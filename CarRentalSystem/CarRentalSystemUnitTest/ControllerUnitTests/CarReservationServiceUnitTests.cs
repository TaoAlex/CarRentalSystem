using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRentalSystem.Context;
using CarRentalSystem.Model;
using Moq;
using System.Data.Entity;
using CarRentalSystem.Controller;
using System.Linq;
using System.Collections.Generic;

namespace CarRentalSystemUnitTest.ControllerUnitTests
{

    [TestClass]
    public class ReservationUnitTests
    {
        TestSetupHelper unitTestHelper = new TestSetupHelper();

        [TestMethod]
        public void FailsToMakeReservationWhenReturnDateIsLessThanRentalDate()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);
            unitTestHelper.setUpEmptyMockReservations(reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    csr.requestReservation(
                    customerMockSet.Object.First().id,
                    carTypeMockSet.Object.First().carTypeName,
                    DateTime.Now,
                    new DateTime(2002, 10, 10)
                    ).Wait();
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new ArgumentException().GetType() || e.InnerException.GetType() == new ArgumentException().GetType());
                }
            }

        }

        [TestMethod]
        public void FailsToMakeReservationWhenInvalidCustomer()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);
            unitTestHelper.setUpEmptyMockReservations(reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var asyncReturnVal = csr.requestReservation(
                        5,
                        carTypeMockSet.Object.First().carTypeName,
                        DateTime.Now,
                        new DateTime(2019, 1, 1)
                        );

                    asyncReturnVal.Wait();
                    var returnVal = asyncReturnVal.Result;


                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new ArgumentException().GetType() || e.InnerException.GetType() == new ArgumentException().GetType());
                }

            }
        }

        [TestMethod]
        public void FailsToMakeReservationWhenInvalidCarType()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);
            unitTestHelper.setUpEmptyMockReservations(reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var asyncReturnVal = csr.requestReservation(
                        customerMockSet.Object.First().id,
                        "a",
                        DateTime.Now, new DateTime(2019, 10, 10));
                    asyncReturnVal.Wait();
                    var returnVal = asyncReturnVal.Result;

                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new ArgumentException().GetType() || e.InnerException.GetType() == new ArgumentException().GetType());
                }
            }
        }

        [TestMethod]
        public void FailsToMakeReservationWhenNoFreeCarsOfGivenType1()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            var carTypes = carTypeMockSet.Object.ToArray();
            var carType1 = carTypes[0];
            var carType2 = carTypes[1];
            var customer = customerMockSet.Object.ToArray()[0];

            var reservation1 = new Reservation(
                customer,
                carType1,
                new DateTime(2018, 11, 12),
                new DateTime(2018, 12, 12));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var asyncReturnVal = csr.requestReservation(
                        customerMockSet.Object.First().id,
                        carTypeMockSet.Object.First().carTypeName,
                        DateTime.Now,
                        new DateTime(2018, 12, 10));

                    asyncReturnVal.Wait();
                    var returnVal = asyncReturnVal.Result;

                    reservationMockSet.Verify(m => m.Add(It.IsAny<Reservation>()), Times.Never());
                    Assert.IsNull(returnVal);
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new ArgumentException().GetType() || e.InnerException is ArgumentException);
                }
            }
        }

        [TestMethod]
        public void SuccessfullyRequestReservationWithDateAfterOtherReservation()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            var carTypes = carTypeMockSet.Object.ToArray();
            var carType1 = carTypes[0];
            var carType2 = carTypes[1];
            var customer = customerMockSet.Object.ToArray()[0];

            var reservation1 = new Reservation(
                customer,
                carType1,
                new DateTime(2018, 11, 12),
                new DateTime(2018, 12, 12));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                var asyncReturnVal = csr.requestReservation(
                    customerMockSet.Object.First().id,
                    carTypeMockSet.Object.First().carTypeName,
                    new DateTime(2018, 12, 13),
                    new DateTime(2018, 12, 14));
                asyncReturnVal.Wait();
                var returnVal = asyncReturnVal.Result;


                reservationMockSet.Verify(m => m.Add(It.IsAny<Reservation>()), Times.Once());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void SuccessfullyRequestsReservationWithDateBeforeOtherReservation()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();
            var reservationMockSet = new Mock<DbSet<Reservation>>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            var carTypes = carTypeMockSet.Object.ToArray();
            var carType1 = carTypes[0];
            var carType2 = carTypes[1];
            var customer = customerMockSet.Object.ToArray()[0];

            var reservation1 = new Reservation(
                customer,
                carType1,
                new DateTime(2018, 11, 12),
                new DateTime(2018, 12, 12));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);


            using (var csr = new CarReservationService(mockContext.Object))
            {
                var asyncReturnVal = csr.requestReservation(
                    customerMockSet.Object.First().id,
                    carTypeMockSet.Object.First().carTypeName,
                    new DateTime(2018, 11, 1),
                    new DateTime(2018, 11, 11));

                asyncReturnVal.Wait();
                var returnVal = asyncReturnVal.Result;


                reservationMockSet.Verify(m => m.Add(It.IsAny<Reservation>()), Times.Once());
                mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
            }

        }
    }
    [TestClass]
    public class ActivatedReservationUnitTests {

        TestSetupHelper unitTestHelper = new TestSetupHelper();

        [TestMethod]
        public void SuccessfullyActivatesReservationWhenCarsAreAvailable()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)),
                DateTime.Now.AddDays(1));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> {reservation1}, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation>(), activatedReservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                var reservation = reservationMockSet.Object.First();

                var asyncActivatedReservation = csr.activateReservation(reservation.id);
                asyncActivatedReservation.Wait();
                var activatedReservation = asyncActivatedReservation.Result;
                
                activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Once());
                mockContext.Verify(a => a.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void FailsToActivateReservationWhenReservationIsCancelled()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)),
                DateTime.Now.AddDays(1));
            reservation1.id = 0;
            reservation1.cancelled = true;

            unitTestHelper.setUpMockReservations(new List<Reservation> {reservation1}, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation>(), activatedReservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var reservation = reservationMockSet.Object.First();

                    var asyncActivatedReservation = csr.activateReservation(reservation.id);
                    asyncActivatedReservation.Wait();
                    var activatedReservation = asyncActivatedReservation.Result;
                
                    activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Never());
                    mockContext.Verify(a => a.SaveChangesAsync(), Times.Never());

                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new InvalidOperationException().GetType() || e.InnerException is InvalidOperationException);
                }
            }
        }

        [TestMethod]
        public void SuccessfullyActivatesReservationAtRentalDate()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(1));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> {reservation1}, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation>(), activatedReservationMockSet, mockContext);


            using (var csr = new CarReservationService(mockContext.Object))
            {
                var reservation = reservationMockSet.Object.First();

                var asyncActivatedReservation = csr.activateReservation(reservation.id);
                asyncActivatedReservation.Wait();
                var activatedReservation = asyncActivatedReservation.Result;


                activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Once());
                mockContext.Verify(a => a.SaveChangesAsync(), Times.Once());
            }
        }

        [TestMethod]
        public void FailsToActivateReservationWhenExpectedReturnDateHasPassed()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType1,
                DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0)),
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation>(), activatedReservationMockSet, mockContext);


            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var reservation = reservationMockSet.Object.First();

                    var asyncActivatedReservation = csr.activateReservation(reservation.id);
                    asyncActivatedReservation.Wait();
                    var activatedReservation = asyncActivatedReservation.Result;

                    activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Never());

                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new InvalidOperationException().GetType() || e.InnerException is InvalidOperationException);

                }

            }
        }
        

        [TestMethod]
        public void FailsToActivateReservationWhenRentalDateHasNotYetBeenMet()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2));

            reservation1.id = 0;

            unitTestHelper.setUpMockReservations(new List<Reservation> {reservation1}, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);
            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation>(), activatedReservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var reservations = reservationMockSet.Object.ToArray();
                    var reservation = reservations[0];

                    var asyncActivatedReservation = csr.activateReservation(reservation.id);
                    asyncActivatedReservation.Wait();
                    var activatedReservation = asyncActivatedReservation.Result;

                    Assert.IsNull(activatedReservation);
                    activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Never());

                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new InvalidOperationException().GetType() || e.InnerException is InvalidOperationException);

                }
            }
        }

        [TestMethod]
        public void FailsToActivateReservationWhenNoCarsAreAvailable()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(2));

            var reservation2 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(2));

            reservation1.id = 0;
            reservation2.id = 1;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1, reservation2 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var reservations = reservationMockSet.Object.ToArray();
                    var rentalcars = rentalCarMockSet.Object.ToArray();
                    var existingActivatedReservation = new ActivatedReservation(reservations[0], rentalcars[1], 0, true);

                    unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation> { existingActivatedReservation }, activatedReservationMockSet, mockContext);

                    var asyncActivatedReservation = csr.activateReservation(reservations[1].id);
                    asyncActivatedReservation.Wait();
                    var activatedReservation = asyncActivatedReservation.Result;

                    Assert.IsNull(activatedReservation);
                    activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Never());

                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new InvalidOperationException().GetType() || e.InnerException is InvalidOperationException);
                }
            }
        }
        [TestMethod]
        public void SuccessfullyActivatesReservationAfterCollidingActivatedReservationIsDeactivated()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(2));

            var reservation2 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(2));

            reservation1.id = 0;
            reservation2.id = 1;

            unitTestHelper.setUpMockReservations(new List<Reservation> { reservation1, reservation2 }, carTypeMockSet, customerMockSet, reservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                var reservations = reservationMockSet.Object.ToArray();
                var rentalcars = rentalCarMockSet.Object.ToArray();
                var existingActivatedReservation = new ActivatedReservation(reservations[0], rentalcars[1], 0, false);

                unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation> { existingActivatedReservation }, activatedReservationMockSet, mockContext);

                var asyncActivatedReservation = csr.activateReservation(reservation1.id);
                asyncActivatedReservation.Wait();
                var activatedReservation = asyncActivatedReservation.Result;


                activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Once());
                mockContext.Verify(a => a.SaveChangesAsync(), Times.Once());

            }
        }
        [TestMethod]
        public void FailsToDeactivateActivatedReservationWhenItIsAlreadyDeactivated()
        {
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

            var reservation1 = new Reservation(
                customer,
                carType2,
                DateTime.Now,
                DateTime.Now.AddDays(2));

            reservation1.id = 0;

            var rentalcars = rentalCarMockSet.Object.ToArray();
            var existingActivatedReservation = new ActivatedReservation( reservation1, rentalcars[1], 0, false);
            existingActivatedReservation.id = 1;

            unitTestHelper.setUpMockActivatedReservations(new List<ActivatedReservation> { existingActivatedReservation }, activatedReservationMockSet, mockContext);

            using (var csr = new CarReservationService(mockContext.Object))
            {
                try
                {
                    var asyncActivatedReservation = csr.deactivateActivatedReservation(existingActivatedReservation.id);
                    asyncActivatedReservation.Wait();
                    var activatedReservation = asyncActivatedReservation.Result;

                    activatedReservationMockSet.Verify(a => a.Add(It.IsAny<ActivatedReservation>()), Times.Never());
                    mockContext.Verify(a => a.SaveChangesAsync(), Times.Never());
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new InvalidOperationException().GetType() || e.InnerException is InvalidOperationException);
                }
            }
        }
    }

    [TestClass]
    public class OtherCarReservationServiceUnitTests
    {
        TestSetupHelper unitTestHelper = new TestSetupHelper();

        [TestMethod]
        public void SuccessfullyUpdateCarMileage()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                var car = rentalCarMockSet.Object.First();
                service.updateCarMileage(car.carNumber, car.currentMileage + 100).Wait();

                mockContext.Verify(rc => rc.SaveChangesAsync(), Times.Once());
            }

        }
        [TestMethod]
        public void FailToUpdateCarMileageWithNewMilageLessThanCurrentMileage()
        {
            var customerMockSet = new Mock<DbSet<BasicCustomer>>();
            var rentalCarMockSet = new Mock<DbSet<RentalCar>>();
            var carTypeMockSet = new Mock<DbSet<RentalCarType>>();
            var mockContext = new Mock<CarRentalContext>();

            unitTestHelper.setUpMockBasicCustomers(customerMockSet, mockContext);
            unitTestHelper.setUpMockCars(rentalCarMockSet, carTypeMockSet, mockContext);

            using (var service = new DatabaseAccessService(mockContext.Object))
            {
                try
                {
                    var car = rentalCarMockSet.Object.First();
                    service.updateCarMileage(car.carNumber, car.currentMileage - 100).Wait();
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.GetType() == new ArgumentException().GetType() || e.InnerException.GetType() == new ArgumentException().GetType());
                }
            }
        }


    }
}
