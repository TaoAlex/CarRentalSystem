using System;
using CarRentalSystem.Context;
using CarRentalSystem.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using CarRentalSystem.Controller;
using System.Linq;
using System.Collections.Generic;
using CarRentalSystem.Model.Config;
using System.Data.Entity.Infrastructure;
using TestingDemo;

namespace CarRentalSystemUnitTest.ControllerUnitTests
{
    class TestSetupHelper
    {


        #region Setup methods

        public void setUpMockBasicCustomers(Mock<DbSet<BasicCustomer>> customerMockSet, Mock<CarRentalContext> mockContext)
        {
            // Set up customers
            var customers = new List<BasicCustomer>
            {
                new BasicCustomer{id = 0, name = "Klas", dateOfBirth = new DateTime(1990, 7, 22)},
                new BasicCustomer{id = 1, name = "Joel", dateOfBirth = new DateTime(1993, 2, 12)},
                new BasicCustomer{id = 2, name = "Emma", dateOfBirth = new DateTime(1991, 1, 1)},

            }.AsQueryable();

            // Set up Queryable for customers
            customerMockSet.As<IQueryable<BasicCustomer>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<BasicCustomer>(customers.Provider));
            customerMockSet.As<IQueryable<BasicCustomer>>().Setup(m => m.Expression).Returns(customers.Expression);
            customerMockSet.As<IQueryable<BasicCustomer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            customerMockSet.As<IQueryable<BasicCustomer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());
            customerMockSet.As<IDbAsyncEnumerable<BasicCustomer>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<BasicCustomer>(customers.GetEnumerator()));


            mockContext.Setup(m => m.basicCustomers).Returns(customerMockSet.Object);
        }

        public void setUpEmptyMockReservations(Mock<DbSet<Reservation>> reservationMockSet, Mock<CarRentalContext> mockContext)
        {
            var reservationList = new List<Reservation>().AsQueryable();
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Reservation>(reservationList.Provider));
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.Expression).Returns(reservationList.Expression);
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.ElementType).Returns(reservationList.ElementType);
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.GetEnumerator()).Returns(reservationList.GetEnumerator());
            reservationMockSet.As<IDbAsyncEnumerable<Reservation>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Reservation>(reservationList.GetEnumerator()));
            mockContext.Setup(m => m.reservations).Returns(reservationMockSet.Object);
        }

        public void setUpMockReservations(List<Reservation> listOfReservations, Mock<DbSet<RentalCarType>> carTypeMockSet, Mock<DbSet<BasicCustomer>> customerMockSet, Mock<DbSet<Reservation>> reservationMockSet, Mock<CarRentalContext> mockContext)
        {

            var reservationList = listOfReservations.AsQueryable();

            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Reservation>(reservationList.Provider));
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.Expression).Returns(reservationList.Expression);
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.ElementType).Returns(reservationList.ElementType);
            reservationMockSet.As<IQueryable<Reservation>>().Setup(m => m.GetEnumerator()).Returns(reservationList.GetEnumerator());
            reservationMockSet.As<IDbAsyncEnumerable<Reservation>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Reservation>(reservationList.GetEnumerator()));


            mockContext.Setup(m => m.reservations).Returns(reservationMockSet.Object);
        }

        public void setUpMockCars(List<RentalCar> carList, List<RentalCarType> carTypeList, Mock<DbSet<RentalCar>> rentalCarMockSet, Mock<DbSet<RentalCarType>> carTypeMockSet, Mock<CarRentalContext> mockContext)
        {
            var cars = carList.AsQueryable();
            var carTypes = carTypeList.AsQueryable();

            // Set up Queryable for car types
            carTypeMockSet.As<IQueryable<RentalCarType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<RentalCarType>(carTypes.Provider));
            carTypeMockSet.As<IQueryable<RentalCarType>>().Setup(m => m.Expression).Returns(carTypes.Expression);
            carTypeMockSet.As<IQueryable<RentalCarType>>().Setup(m => m.ElementType).Returns(carTypes.ElementType);
            carTypeMockSet.As<IQueryable<RentalCarType>>().Setup(m => m.GetEnumerator()).Returns(carTypes.GetEnumerator());
            carTypeMockSet.As<IDbAsyncEnumerable<RentalCarType>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<RentalCarType>(carTypes.GetEnumerator()));

            
            // Set up Queryable for cars
            rentalCarMockSet.As<IQueryable<RentalCar>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<RentalCar>(cars.Provider));
            rentalCarMockSet.As<IQueryable<RentalCar>>().Setup(m => m.Expression).Returns(cars.Expression);
            rentalCarMockSet.As<IQueryable<RentalCar>>().Setup(m => m.ElementType).Returns(cars.ElementType);
            rentalCarMockSet.As<IQueryable<RentalCar>>().Setup(m => m.GetEnumerator()).Returns(cars.GetEnumerator());

            // Set up mock context
            mockContext.Setup(m => m.carTypes).Returns(carTypeMockSet.Object);
            mockContext.Setup(m => m.rentalCars).Returns(rentalCarMockSet.Object);
        }

        public void setUpMockCars(Mock<DbSet<RentalCar>> rentalCarMockSet, Mock<DbSet<RentalCarType>> carTypeMockSet, Mock<CarRentalContext> mockContext)
        {

            // Set up car types
            var carTypes = new List<RentalCarType>
            {
                new RentalCarType{ carTypeName = "Small Car", basePriceModifier = 1, kmPriceModifier = 0},
                new RentalCarType{ carTypeName = "Van", basePriceModifier = 1.2, kmPriceModifier = 1},
                new RentalCarType{ carTypeName = "Mini Bus", basePriceModifier = 1.7, kmPriceModifier = 1.5},
                new RentalCarType{ carTypeName = "Truck", basePriceModifier = 2, kmPriceModifier = 3}
            };

            // Set up cars
            var cars = new List<RentalCar>
            {
                new RentalCar { carNumber = "BLO333", currentMileage = 0, rentalCarType = carTypes.ToArray()[0] },
                new RentalCar { carNumber = "LEM222", currentMileage = 0, rentalCarType = carTypes.ToArray()[1] },
                new RentalCar { carNumber = "KDD432", currentMileage = 0, rentalCarType = carTypes.ToArray()[2] },
                new RentalCar { carNumber = "TKI993", currentMileage = 0, rentalCarType = carTypes.ToArray()[3] },
            };

            setUpMockCars(cars, carTypes, rentalCarMockSet, carTypeMockSet, mockContext);
        }

        public void setUpMockActivatedReservations(List<ActivatedReservation> activeRes, Mock<DbSet<ActivatedReservation>> activatedReservationMockSet, Mock<CarRentalContext> mockContext)
        {
            var activatedReservationsList = activeRes.AsQueryable();

            activatedReservationMockSet.As<IQueryable<ActivatedReservation>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ActivatedReservation>(activatedReservationsList.Provider));
            activatedReservationMockSet.As<IQueryable<ActivatedReservation>>().Setup(m => m.Expression).Returns(activatedReservationsList.Expression);
            activatedReservationMockSet.As<IQueryable<ActivatedReservation>>().Setup(m => m.ElementType).Returns(activatedReservationsList.ElementType);
            activatedReservationMockSet.As<IQueryable<ActivatedReservation>>().Setup(m => m.GetEnumerator()).Returns(activatedReservationsList.GetEnumerator());
            activatedReservationMockSet.As<IDbAsyncEnumerable<ActivatedReservation>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ActivatedReservation>(activatedReservationsList.GetEnumerator()));


            mockContext.Setup(m => m.activatedReservations).Returns(activatedReservationMockSet.Object);
        }

        public void setUpMockConfigItems(List<ConfigItem> configItems, Mock<DbSet<ConfigItem>> configMockSet, Mock<CarRentalContext> mockContext) {
            var configItemsList = configItems.AsQueryable();

            configMockSet.As<IQueryable<ConfigItem>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ConfigItem>(configItemsList.Provider));
            configMockSet.As<IQueryable<ConfigItem>>().Setup(m => m.Expression).Returns(configItemsList.Expression);
            configMockSet.As<IQueryable<ConfigItem>>().Setup(m => m.ElementType).Returns(configItemsList.ElementType);
            configMockSet.As<IQueryable<ConfigItem>>().Setup(m => m.GetEnumerator()).Returns(configItemsList.GetEnumerator());
            configMockSet.As<IDbAsyncEnumerable<ConfigItem>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<ConfigItem>(configItemsList.GetEnumerator()));


            mockContext.Setup(m => m.configItems).Returns(configMockSet.Object);
        }
        #endregion

    }
}
