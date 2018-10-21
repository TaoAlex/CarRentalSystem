using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalSystem.Context;
using CarRentalSystem.Model;
using Moq;
using System.Data.Entity;
using CarRentalSystem.Controller;
using System.Linq;
using System.Collections.Generic;
//using CarRentalSystem.Controller.Services;
using CarRentalSystem.Model.Config;

namespace CarRentalSystemUnitTest.ControllerUnitTests
{
    [TestClass]
    public class RentalRateCalculatorServiceUnitTests
    {
        TestSetupHelper unitTestHelper = new TestSetupHelper();

        [TestMethod]
        public void SuccessfullyCalculateRentalRate()
        {
            double kmPrice = 15;
            double baseDayRentalPrice = 20;
            double mileage = 100;

            var configMockSet = new Mock<DbSet<ConfigItem>>();
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();
            var activatedReservationMockSet = new Mock<DbSet<ActivatedReservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            var carTypes = carTypeMockSet.Object.ToArray();
            var carType1 = carTypes[0];
            var carType2 = carTypes[1];
            var customer = customerMockSet.Object.ToArray()[0];

            var reservation = new Reservation(
                customer,
                carType2,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2));
            reservation.id = 0;

            var rentalCar = rentalCarMockSet.Object.FirstOrDefault(car => car.rentalCarType.carTypeName == carType2.carTypeName);
            var activatedReservation = new ActivatedReservation(reservation, rentalCar, 0, true );
            activatedReservation.endMileage = 100;
            activatedReservation.actualReturnDate = DateTime.Now;
            activatedReservation.isCurrentlyActive = false;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation> { activatedReservation }, activatedReservationMockSet, mockContext);

            var configList = new List<ConfigItem> {
                new ConfigItem{ configName = "kmPrice", value = kmPrice.ToString()},
                new ConfigItem{ configName = "baseDayRentalPrice", value = baseDayRentalPrice.ToString()}
            };

            unitTestHelper.setUpMockConfigItems(configList, configMockSet, mockContext);
            
            double kmPriceModifier = carType2.kmPriceModifier;
            double basePriceModifier = carType2.basePriceModifier;
            double dayDiff = (reservation.expectedReturnDate - reservation.rentalDate).Days;

            double expectedCost = baseDayRentalPrice * dayDiff * basePriceModifier + kmPrice * mileage * kmPriceModifier;

            using (var rrcs = new RentalRateCalculatorService(mockContext.Object))
            {
                var asyncCost = rrcs.calculateCost(activatedReservation.id);
                asyncCost.Wait();
                var cost = asyncCost.Result;
                Assert.AreEqual(expectedCost, cost);
            }
        }
    }
}
