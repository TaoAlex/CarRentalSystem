using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalSystem.Model;
using CarRentalSystem.Context;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CarRentalSystem.Controller
{
    /// <summary>
    /// This class is used to handle reservations.
    /// </summary>
    public class CarReservationService: IDisposable
    {

        private CarRentalContext context;
        private DatabaseAccessService db;

        public CarReservationService(CarRentalContext context)
        {
            this.context = context;
            db = new DatabaseAccessService(context);
        }

        public CarReservationService()
        {
            context = new CarRentalContext();
            db = new DatabaseAccessService(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        /// <summary>
        /// Precondition: The given reservation must exist in the database
        /// Activates the given reservation. This means the car has been rented to a customer.
        /// A car is assigned and the car mileage is tracked. 
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns>Returns the ActivatedReservation class that was created.</returns>
        /// 

        public async Task<ActivatedReservation> activateReservation(int reservationId)
        {
           
            RentalCar car = await db.getAvailableRentalCar(reservationId);

            var res = await db.getReservation(reservationId);
                
            if (res.cancelled)
            {
                throw new InvalidOperationException("A cancelled reservation cannot be activated.");
            }

            ActivatedReservation activatedReservation = new ActivatedReservation(res, car, car.currentMileage, true);

            if (activatedReservation.reservation.expectedReturnDate < DateTime.Now ||
                activatedReservation.reservation.rentalDate > DateTime.Now)
            {
                throw new InvalidOperationException("The rental date has not yet been passed. " +
                    "Cannot activate reservation. " +
                    "\nGiven startDate: " + activatedReservation.reservation.rentalDate +
                    "\nGiven endDate: " + activatedReservation.reservation.rentalDate);
            }

            context.activatedReservations.Add(activatedReservation);
            await context.SaveChangesAsync();
            return activatedReservation;

            
        }

        /// <summary>
        /// Precondition: The given ActivatedReservation must be currently active. 
        /// Deactivates the given ActivatedReservation. This means the rented car has been 
        /// returned. 
        /// </summary>
        /// <param name="activatedReservationId"></param>
        /// <returns>Returns the deactivated ActivatedReservation.</returns>
        public async Task<ActivatedReservation> deactivateActivatedReservation(int activatedReservationId)
        {
            var activatedReservation = await db.getActivatedReservation(activatedReservationId);
            if (!activatedReservation.isCurrentlyActive)
            {
                throw new InvalidOperationException("Cannot deactivate an already deactivated ActivatedReservation");
            }
            activatedReservation.endMileage = activatedReservation.rentedCar.currentMileage;
            activatedReservation.isCurrentlyActive = false;
            activatedReservation.actualReturnDate = DateTime.Now;
            await context.SaveChangesAsync();

            return activatedReservation;
            
        }
        
        /// <summary>
        /// This method is used to request a reservation. If there are no available cars of given 
        /// car type for the given date period, this method will throw an exception. 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="carTypeName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns the reservation that was made.</returns>
        public async Task<Reservation> requestReservation(int customerId, string carTypeName, DateTime startDate, DateTime endDate)
        {
            
            var rentalCars = await db.getRentalCarsByType(carTypeName);
            var allRentalCars = rentalCars.Length;

            var reservedCars = await db.countReservedCars(carTypeName, startDate, endDate);
               
            var availableCars = allRentalCars - reservedCars;

            if (allRentalCars == 0)
            {
                throw new ArgumentException("No such car type exists in the database");
            }

            if (availableCars > 0)
            {
                var res = await db.makeReservation(customerId, carTypeName, startDate, endDate);   
                return res;
            }

            throw new ArgumentException("No available cars for this period");
            

        }
    }
}
