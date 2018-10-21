using CarRentalSystem.Context;
using CarRentalSystem.Model;
using CarRentalSystem.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Controller
{ 
    /// <summary>
    /// This class is used to set rental rates and calculate price of rentals.
    /// </summary>
    public class RentalRateCalculatorService: IDisposable
    {
        private double baseDayRental { get; set; }
        private double kmPrice { get; set; }
        private CarRentalContext context;
        private DatabaseAccessService service;

        public RentalRateCalculatorService(CarRentalContext context)
        {
            this.context = context;
            service = new DatabaseAccessService(context);
        }

        public RentalRateCalculatorService()
        {
            context = new CarRentalContext();
            service = new DatabaseAccessService(context);
        }

        public async Task checkRentalRates()
        {
            if (baseDayRental == 0 || kmPrice == 0)
            {
                baseDayRental = await service.getBaseDayRental();
                kmPrice = await service.getKmPrice();
                if (baseDayRental == 0 || kmPrice == 0)
                {
                    throw new Exception("Base day rental price and km price not defined, use setRentalRate() to set it.");
                }
            }
            
        }
        /// <summary>
        /// The formula for calculating the rental cost is:
        /// baseDayRentalPrice * numberOfDays * baseDayRentalModifier + kmPrice * numberOfKm * kmPriceModifier
        /// Use this to change baseDayRentalPrice and kmPrice variables.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseDayRentalPrice"></param>
        /// <param name="kmPrice"></param>
        /// <returns></returns>
        public async static Task setRentalRate(CarRentalContext context, double baseDayRentalPrice, double kmPrice)
        {
            var service = new DatabaseAccessService(context);
            await service.setBaseDayRentalPrice(baseDayRentalPrice);
            await service.setKmPrice(kmPrice);
        }

        // Calculates the rental price for a reservation when car has been returned
        /// <summary>
        /// The formula for calculating the rental cost is:
        /// baseDayRentalPrice * numberOfDays * baseDayRentalModifier + kmPrice * numberOfKm * kmPriceModifier
        /// Preconditions: The given ActivatedReservation must be deactivated before calling this method.
        /// Calculates the rental price for a reservation.
        /// </summary>
        /// <param name="activatedReservationId"></param>
        /// <returns>The total cost of the rental</returns>
        public async Task<double> calculateCost(int activatedReservationId)
        {
            await checkRentalRates();

            service = new DatabaseAccessService(context);
            
            var activatedReservation = await service.getActivatedReservation(activatedReservationId);

            if (activatedReservation == null)
            {
                throw new ArgumentException("No ActivatedReservation with given id exists");
            }

            var rentedCar = await service.getRentalCar(activatedReservation.rentedCar.carNumber);

            var carType = rentedCar.rentalCarType;
            
            var dayDiff = (activatedReservation.reservation.expectedReturnDate 
                - activatedReservation.reservation.rentalDate).Days;
            var mileage = activatedReservation.endMileage - activatedReservation.startMileage;

            double price = baseDayRental * dayDiff * carType.basePriceModifier 
                + kmPrice * mileage * carType.kmPriceModifier; 

            return price;
        }

        public void Dispose()
        {
            context.Dispose();
            service.Dispose();
        }
    }
}
