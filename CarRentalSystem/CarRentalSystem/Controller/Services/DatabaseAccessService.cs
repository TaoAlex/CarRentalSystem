using System;
using System.Collections.Generic;
using System.Linq;
using CarRentalSystem.Model;
using CarRentalSystem.Context;
using System.Data.Entity;
using CarRentalSystem.Model.Config;
using System.Threading.Tasks;

namespace CarRentalSystem.Controller
{
    /// <summary>
    /// This class provides simple methods for adding and getting data from the database.
    /// </summary>
    public class DatabaseAccessService: IDisposable
    {
        private CarRentalContext context;

        public DatabaseAccessService(CarRentalContext context)
        {
            this.context = context;
        }

        public DatabaseAccessService()
        {
            context = new CarRentalContext();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        #region Database READ
        // ----------------------------------
        // DATABASE READ
        // ----------------------------------

        // Rental rates

        public async Task<double> getBaseDayRental()
        {
            var baseDayRentalPrice = await (from c in context.configItems
                                                      where c.configName == "baseDayRentalPrice"
                                                      select c.value).SingleOrDefaultAsync();
            return double.Parse(baseDayRentalPrice);
        }

        public async Task<double> AwaitTest()
        {
            var baseDayRentalPrice = await (from c in context.configItems
                                                      where c.configName == "baseDayRentalPrice"
                                                      select c.value).SingleOrDefaultAsync();
            return double.Parse(baseDayRentalPrice);
        }

        public async Task<double> getKmPrice()
        {
            var kmPrice = await (from c in context.configItems
                                           where c.configName == "kmPrice"
                                           select c.value).SingleOrDefaultAsync();
            return double.Parse(kmPrice);
        }


        // Customers

        public async Task<BasicCustomer[]> getCustomers()
        {
            var customers = await (from c in context.basicCustomers
                                orderby c.name
                                select c).ToArrayAsync();
            return customers;
        }

        public async Task<BasicCustomer> findCustomers(string name, DateTime dateOfBirth)
        {
            var customer = await (from c in context.basicCustomers
                            where c.name == name && c.dateOfBirth == dateOfBirth
                            select c).FirstOrDefaultAsync();
            return customer;
        }

        public async Task<BasicCustomer> getCustomer(int id)
        {
            var customer = await (from c in context.basicCustomers
                            where c.id == id
                            select c).FirstOrDefaultAsync();
            return customer;
        }

        // Rental cars 

        public async Task<RentalCar[]> getRentalCars()
        {
                var cars = await (from rc in context.rentalCars
                            orderby rc.carNumber
                            select rc).Include(car => car.rentalCarType).ToArrayAsync();
                return cars;
        }

        public async Task<RentalCar> getRentalCar(string carNumber)
        {
            var car = await (from rc in context.rentalCars
                       where rc.carNumber == carNumber
                       select rc).Include(c => c.rentalCarType).FirstOrDefaultAsync();
            return car;
        }

        public async Task<RentalCar[]> getRentalCarsByType(string carType)
        {
            var cars = await (from rc in context.rentalCars
                        where rc.rentalCarType.carTypeName == carType
                        select rc)
                        .Include(car => car.rentalCarType)
                        .ToArrayAsync();
            return cars;
        }

        public async Task<ActivatedReservation[]> getCurrentlyActiveReservations(string carTypeName)
        {
            var currentlyActive = await (from active in context.activatedReservations
                                  where active.reservation.carType.carTypeName == carTypeName && active.isCurrentlyActive
                                  select active)
                                  .Include(a => a.reservation)
                                  .Include(a => a.rentedCar)
                                  .ToArrayAsync();
            return currentlyActive;
        }

        public async Task<ActivatedReservation> getActivatedReservation(int id)
        {
            var currentlyActive = await (from active in context.activatedReservations
                                  where active.id == id
                                  select active)
                                  .Include(a => a.reservation)
                                  .Include(a => a.rentedCar)
                                  .FirstOrDefaultAsync();
            return currentlyActive;
        }

        public async Task<ActivatedReservation[]> getActivatedReservations()
        {
            var currentlyActive = await (from active in context.activatedReservations
                                   select active)
                                  .Include(a => a.reservation)
                                  .Include(a => a.rentedCar)
                                  .ToArrayAsync();
            return currentlyActive;
        }
        
        /// <summary>
        /// Gets a rental car for a given reservation. 
        /// Assumes that the reservation is already registered in the database.
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns>A rental car</returns>
        public async Task<RentalCar> getAvailableRentalCar(int reservationId)
        {
            var r = await getReservation(reservationId);

            var allCars = await getRentalCarsByType(r.carType.carTypeName);
            var currentlyActiveReservations = await getCurrentlyActiveReservations(r.carType.carTypeName);
           
            var currentlyActiveCars = (from active in currentlyActiveReservations
                                      select active.rentedCar).ToList();
            
            var car = allCars.Where(c => !currentlyActiveCars.Contains(c)).FirstOrDefault();

            if (car == null)
            {
                throw new InvalidOperationException("There are no available cars of the given type.");
            }

            return car;
        }


        // Rental car types

        public async Task<RentalCarType[]> getCarTypes()
        {
            var carTypes = await (from ct in context.carTypes
                            orderby ct.carTypeName
                            select ct).ToArrayAsync();
            return carTypes;
        }

        public async Task<RentalCarType> getCarType(string name)
        {
            return await context.carTypes.SingleOrDefaultAsync(x => x.carTypeName == name);
        }
       
        // Reservations

        public async Task<Reservation[]> getReservations()
        {
            Reservation[] reservations = await (from r in context.reservations
                                          where !r.cancelled
                                          orderby r.id
                                          select r)
                                          .Include(res => res.carType)
                                          .Include(res => res.customer)
                                          .ToArrayAsync();
            return reservations;
        }

        public async Task<Reservation[]> getReservationsByCarType(string carTypeName)
        {
            var reservations = await (from r in context.reservations
                                where r.carType.carTypeName == carTypeName && !r.cancelled
                                select r)
                                .Include(res => res.carType)
                                .Include(res => res.customer)
                                .ToArrayAsync();
            return reservations;
        }

        public async Task<Reservation[]> getReservationsByCustomer(BasicCustomer customer)
        {
            var reservations = await (from r in context.reservations
                                where r.customer.id == customer.id && !r.cancelled
                                select r)
                                .Include(res => res.carType)
                                .Include(res => res.customer)
                                .ToArrayAsync();
            return reservations;
        }

        public async Task<Reservation> getReservation(int id)
        {
            var reservations =  await (from r in context.reservations
                                where r.id == id
                                select r)
                                .Include(res => res.carType)
                                .Include(res => res.customer)
                                .FirstOrDefaultAsync();
            return reservations;
        }

        public async Task<List<Reservation>> getReservationsByDate(string carTypeName, DateTime rentalDate, DateTime returnDate)
        {
            var reservations = await context.reservations
                               .Where( r =>

                               ((rentalDate >= r.rentalDate &&
                               rentalDate <= r.expectedReturnDate) ||

                               (returnDate >= r.rentalDate &&
                               returnDate <= r.expectedReturnDate) ||

                               (rentalDate < r.rentalDate &&
                               returnDate > r.expectedReturnDate)) &&

                               r.carType.carTypeName == carTypeName &&
                               !r.cancelled)

                               .Include(res => res.carType)
                               .Include(res => res.customer)
                               .ToListAsync();
            return reservations;
        }

        // Count reservations 
        public async Task<int> countReservedCars(string carTypeName, DateTime rentalStart, DateTime rentalEnd)
        {
            var reservations = await getReservationsByDate(carTypeName, rentalStart, rentalEnd);
            
            return reservations.Count();
        }
        #endregion

        #region DATABASE WRITE

        // --------------------------------
        // DATABASE WRITE
        // --------------------------------
        
        public async Task setBaseDayRentalPrice(double baseDayRentalPrice)
        {
            var currentBaseDayRentalPriceConfig = await (from c in context.configItems
                                        where c.configName == "baseDayRentalPrice"
                                        select c).SingleOrDefaultAsync();

            // If baseDayRentalPrice does not exist in config, create it and assign given value
            if (currentBaseDayRentalPriceConfig == null)
            {
                currentBaseDayRentalPriceConfig = new ConfigItem { configName = "baseDayRentalPrice", value = baseDayRentalPrice.ToString() };
                context.configItems.Add(currentBaseDayRentalPriceConfig);
            }

            currentBaseDayRentalPriceConfig.value = baseDayRentalPrice.ToString();
            await context.SaveChangesAsync();
        }

        public async Task setKmPrice(double kmPrice)
        {
            var currentKmPriceConfig = await (from c in context.configItems
                                        where c.configName == "kmPrice"
                                        select c).SingleOrDefaultAsync();
            
            // If kmPrice does not exist in config, create it and assign given value
            if (currentKmPriceConfig == null)
            {
                currentKmPriceConfig = new ConfigItem { configName = "kmPrice", value = kmPrice.ToString()};
                context.configItems.Add(currentKmPriceConfig);
            }

            currentKmPriceConfig.value = kmPrice.ToString();
            await context.SaveChangesAsync();
        }

        
        public async Task<BasicCustomer> addCustomer(string name, DateTime dateOfBirth)
        {
            var customer = new BasicCustomer { name = name, dateOfBirth = dateOfBirth };
            context.basicCustomers.Add(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        public async Task<BasicCustomer> addCustomer(BasicCustomer customer)
        {
            context.basicCustomers.Add(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        public async Task<RentalCarType> addRentalCarType(RentalCarType carType)
        {
            context.carTypes.Add(carType);
            await context.SaveChangesAsync();
            return carType;
        }

        public async Task<RentalCarType> addRentalCarType(string name, double kmPriceModifier, double basePriceModifier)
        {
            RentalCarType carType = new RentalCarType { carTypeName = name, kmPriceModifier = kmPriceModifier, basePriceModifier = basePriceModifier };
            context.carTypes.Add(carType);
            await context.SaveChangesAsync();
            return carType;
        }

        public async Task<RentalCar> addRentalCar(string carPlateNumber, string carType, double mileage)
        {
            var carTypeObj = await getCarType(carType);
            

            RentalCar rentalCar = new RentalCar { carNumber = carPlateNumber, rentalCarType = carTypeObj, currentMileage = mileage };

            if (carTypeObj != null)
            {
                context.rentalCars.Add(rentalCar);
                context.carTypes.Attach(carTypeObj);
                await context.SaveChangesAsync();
                return rentalCar;
            }

            return null;
        }

        /// <summary>
        /// Updates the mileage of a given car to the new given value.
        /// </summary>
        /// <param name="rentalCarNr"></param>
        /// <param name="newMileage"></param>
        /// <returns></returns>
        public async Task updateCarMileage(string rentalCarNr, double newMileage)
        {
            var rentalCar = await getRentalCar(rentalCarNr);

            if (newMileage < rentalCar.currentMileage)
            {
                throw new ArgumentException("New mileage cannot be less than current mileage");
            }

            rentalCar.currentMileage = newMileage;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds given mileage to given car.
        /// </summary>
        /// <param name="rentalCarNr"></param>
        /// <param name="newMileage"></param>
        /// <returns></returns>
        public async Task addCarMileage(string rentalCarNr, double newMileage)
        {
            var rentalCar = await getRentalCar(rentalCarNr);

            if (newMileage < 0)
            {
                throw new ArgumentException("New mileage cannot be negative");
            }

            rentalCar.currentMileage = rentalCar.currentMileage + newMileage;
            await context.SaveChangesAsync();

        }

        /// <summary>
        /// Creates a reservation without regards to number of
        /// available cars. To properly request a reservation, refer to 
        /// CarReservationService.requestReservation()
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="carTypeName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns a reservation</returns>
        internal async Task<Reservation> makeReservation(int customerId, string carTypeName, DateTime startDate, DateTime endDate)
        {
            var existingCustomer = await getCustomer(customerId);
            var existingCarType = await getCarType(carTypeName);
            
            if (existingCustomer == null)
            {
                throw new ArgumentException("Given Customer does not exist in the database" + existingCustomer);
            }

            if (existingCarType == null)
            {
                throw new ArgumentException("Given Car Type does not exist in the database");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("The parameter startDate cannot be later than end date");
            }

            Reservation reservation = new Reservation(existingCustomer, existingCarType, startDate, endDate);
            context.reservations.Add(reservation);
            context.carTypes.Attach(existingCarType);
            context.basicCustomers.Attach(existingCustomer);
            await context.SaveChangesAsync();

            return reservation;
        }
        #endregion

    }
}
