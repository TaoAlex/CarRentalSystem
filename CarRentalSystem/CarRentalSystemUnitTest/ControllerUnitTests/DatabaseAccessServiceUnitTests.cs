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

namespace CarRentalSystemUnitTest.ControllerUnitTests
{

    [TestClass]
    public class GetDataDatabaseAccessServiceUnitTests
    {
        TestSetupHelper unitTestHelper = new TestSetupHelper();

        private void SetUpForDateOverlapTest(
            Mock<DbSet<BasicCustomer>> customerMockSet, 
            Mock<DbSet<RentalCarType>> carTypeMockSet,
            Mock<DbSet<RentalCar>> rentalCarMockSet,
            Mock<DbSet<Reservation>> reservationMockSet,
            Mock<CarRentalContext> mockContext
            )
        {

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);

            // Set up car types
            var carTypes = new List<RentalCarType>
            {
                new RentalCarType{ carTypeName = "Small car", basePriceModifier = 1, kmPriceModifier = 0}
            };


            // Set up cars
            var cars = new List<RentalCar>
            {
                new RentalCar { carNumber = "BLO333", currentMileage = 0, rentalCarType = carTypes.First() },
                new RentalCar { carNumber = "ASD111", currentMileage = 0, rentalCarType = carTypes.First() }
            };

            unitTestHelper.setUpMockCars(cars, carTypes, rentalCarMockSet, carTypeMockSet, mockContext);

            // Set up reservations
            var reservationList = new List<Reservation>
            {
                new Reservation(
                    customerMockSet.Object.First(),
                    carTypeMockSet.Object.First(),
                    new DateTime(2018, 11, 1),
                    new DateTime(2018, 12, 1)),

                new Reservation(
                    customerMockSet.Object.First(),
                    carTypeMockSet.Object.First(),
                    new DateTime(2018, 11, 1),
                    new DateTime(2018, 12, 1))
            };

            unitTestHelper.setUpMockReservations(reservationList, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
        }

        [TestMethod]
        public void CountReservationsRentalDateOverlap()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            SetUpForDateOverlapTest(customerMockSet, carTypeMockSet, rentalCarMockSet, reservationMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var asyncTestValue = service.countReservedCars(
                    "Small car", 
                    new DateTime(2018, 11, 22), 
                    new DateTime(2018, 12, 22)
                    );
                asyncTestValue.Wait();
                var testValue = asyncTestValue.Result;

                Assert.AreEqual(testValue, 2);
            }
        }

        [TestMethod]
        public void CountReservationsReturnDateOverlap()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            SetUpForDateOverlapTest(customerMockSet, carTypeMockSet, rentalCarMockSet, reservationMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var asyncTestValue = service.countReservedCars(
                    "Small car",
                    new DateTime(2018, 10, 1),
                    new DateTime(2018, 11, 13)
                    );
                asyncTestValue.Wait();
                var testValue = asyncTestValue.Result;

                Assert.AreEqual(testValue, 2);
            }
        }

        [TestMethod]
        public void CountReservationsDateOuterEncapsulation()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            SetUpForDateOverlapTest(customerMockSet, carTypeMockSet, rentalCarMockSet, reservationMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var asyncTestValue = service.countReservedCars(
                    "Small car",
                    new DateTime(2018, 10, 1),
                    new DateTime(2018, 12, 30)
                    );
                asyncTestValue.Wait();
                var testValue = asyncTestValue.Result;

                Assert.AreEqual(testValue, 2);
            }
        }

        [TestMethod]
        public void CountReservationsDateEncapsulated()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            SetUpForDateOverlapTest(customerMockSet, carTypeMockSet, rentalCarMockSet, reservationMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var asyncTestValue = service.countReservedCars(
                    "Small car",
                    new DateTime(2018, 11, 22),
                    new DateTime(2018, 11, 23)
                    );
                asyncTestValue.Wait();
                var testValue = asyncTestValue.Result;

                Assert.AreEqual(testValue, 2);
            }
        }
    }
    [TestClass]
    public class AddDataDatabaseAccessServiceUnitTests
    {

        TestSetupHelper unitTestHelper = new TestSetupHelper();

        [TestMethod]
        public void AddsCustomer()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var mockContext = new Mock<CarRentalContext>();
            mockContext.Setup(m => m.basicCustomers).Returns(customerMockSet.Object);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.addCustomer("Alex", DateTime.Now).Wait();
                customerMockSet.Verify(m => m.Add(It.IsAny<BasicCustomer>()), Times.Once());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void AddsRentalCarType()
        {
            var rentalCarMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            mockContext.Setup(m => m.carTypes).Returns(rentalCarMockSet.Object);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.addRentalCarType("Small car", 1, 0).Wait();
                rentalCarMockSet.Verify(m => m.Add(It.IsAny<RentalCarType>()), Times.Once());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void AddsRentalCarWithExistingCarType()
        {
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();

            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            // Test adding rental car
            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.addRentalCar("ABC123", "Small Car", 0).Wait();
                rentalCarMockSet.Verify(m => m.Add(It.IsAny<RentalCar>()), Times.Once());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void FailsToAddCarWhenGivenCarTypeDoesNotExist()
        {
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();

            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            // Attempt to add rental car of type which does not exist in the database
            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var asyncReturnVal = service.addRentalCar("ABC123", "Mini Car", 0);
                asyncReturnVal.Wait();
                var returnVal = asyncReturnVal.Result;

                rentalCarMockSet.Verify(m => m.Add(It.IsAny<RentalCar>()), Times.Never());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Never());
                Assert.IsNull(returnVal);
            }
        }

        [TestMethod]
        public void AddsBaseDayRentalPriceConfigWhenNoPreviousConfigExists()
        {
            var configMockSet = new Mock<DbSet<ConfigItem>>();
            var mockContext = new Mock<CarRentalContext>();

            var configList = new List<ConfigItem>();

            unitTestHelper.setUpMockConfigItems(configList, configMockSet, mockContext);
            
            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.setBaseDayRentalPrice(15).Wait();
                configMockSet.Verify(c => c.Add(It.IsAny<ConfigItem>()), Times.Once());
                mockContext.Verify(c => c.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void AddsKmPriceConfigWhenNoPreviousConfigExists()
        {
            var configMockSet = new Mock<DbSet<ConfigItem>>();
            var mockContext = new Mock<CarRentalContext>();

            var configList = new List<ConfigItem>();

            unitTestHelper.setUpMockConfigItems(configList, configMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.setKmPrice(15).Wait();
                configMockSet.Verify(c => c.Add(It.IsAny<ConfigItem>()), Times.Once());
                mockContext.Verify(c => c.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void UpdatesButDoesNotAddBaseDayRentalPriceConfigWhenConfigExists()
        {
            var configMockSet = new Mock<DbSet<ConfigItem>>();
            var mockContext = new Mock<CarRentalContext>();

            var configList = new List<ConfigItem> {
                new ConfigItem{ configName = "baseDayRentalPrice", value = "15"},
            };
            
            unitTestHelper.setUpMockConfigItems(configList, configMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.setBaseDayRentalPrice(15).Wait();
                configMockSet.Verify(c => c.Add(It.IsAny<ConfigItem>()), Times.Never());
                mockContext.Verify(c => c.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void UpdatesButDoesNotAddKmPriceConfigWhenConfigExists()
        {
            var mockContext = new Mock<CarRentalContext>();
            var configMockSet = new Mock<DbSet<ConfigItem>>();

            var configList = new List<ConfigItem> {
                new ConfigItem{ configName = "kmPrice", value = "15"}
            };
            
            unitTestHelper.setUpMockConfigItems(configList, configMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                service.setKmPrice(15).Wait();
                configMockSet.Verify(c => c.Add(It.IsAny<ConfigItem>()), Times.Never());
                mockContext.Verify(c => c.SaveChangesAsync(), Times.Once());
            }
        }




    }
    
}
