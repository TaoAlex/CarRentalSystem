using CarRentalSystem.Context;
using CarRentalSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarRentalSystem.Controller;
using CarRentalSystem.Model.ModelCustomizer;

namespace CarRentalSystem
{
    class Program
    {
        static void displayCommands()
        {
            Console.WriteLine("__________________________________");
            Console.WriteLine("COMMANDS FOR DISPLAYING DATA");
            Console.WriteLine("_commands_: Show all commands");
            Console.WriteLine("_customers_: Show all customers");
            Console.WriteLine("_carTypes_: Show existing car types");
            Console.WriteLine("_cars_: Show all cars");
            Console.WriteLine("_reservations_: Show existing reservations");
            Console.WriteLine("_activatedReservations_: Show all activated reservations");
            Console.WriteLine("__________________________________");
            Console.WriteLine("COMMANDS FOR WRITING DATA");
            Console.WriteLine("_addCustomer_: Add new customer");
            Console.WriteLine("_addCarType_: Add new car type");
            Console.WriteLine("_addCar_: Add new car");
            Console.WriteLine("__________________________________");
            Console.WriteLine("COMMANDS FOR LOGIC");
            Console.WriteLine("_requestReservation_: Add new reservation");
            Console.WriteLine("_activateReservation_: Activates the reservation, use when car has been retrieved by customer");
            Console.WriteLine("_deactivateReservation_: Deactivates reservation, use when car has been returned");
            Console.WriteLine("_simulateDriving_: Simulates driving a car a few kilometers");
            Console.WriteLine("_calculatePrice_: Calculates the price of the reservation, use after reservation has been deactivated");
        }

        static void showCustomers()
        {
            using (var service = new DatabaseAccessService())
            {
                var customers = service.getCustomers().Result;
                customers.ToList().ForEach(c => Console.WriteLine("id: " + c.id + ", name: " + c.name));
            }
        }

        static void showCarTypes()
        {
            using (var service = new DatabaseAccessService())
            {
                var carTypes = service.getCarTypes().Result;
                carTypes.ToList().ForEach(c => Console.WriteLine("carType: " + c.carTypeName + ", basePriceModifier: " + c.basePriceModifier + ", kmPriceModifier: " + c.kmPriceModifier));
            }
        }

        static void showCars()
        {
            using (var service = new DatabaseAccessService())
            {
                var cars = service.getRentalCars().Result;
                cars.ToList().ForEach(c => Console.WriteLine("carPlate: " + c.carNumber + ", carType: " + c.rentalCarType.carTypeName + ", currentMileage: " + c.currentMileage));
            }
        }

        static void showReservations()
        {
            using (var service = new DatabaseAccessService())
            {
                var res = service.getReservations().Result;
                res.ToList().ForEach(r => Console.WriteLine("id: " + r.id + ", carType: " + r.carType.carTypeName));
            }
        }

        static void showActivatedReservations()
        {
            using (var service = new DatabaseAccessService())
            {
                var ares = service.getActivatedReservations().Result;
                ares.ToList().ForEach(ar => Console.WriteLine("id: " + ar.id + ", car: " + ar.rentedCar + ", isCurrentlyActive: " + ar.isCurrentlyActive));
            }
        }

        static void calculatePrice()
        {
            Console.WriteLine("Calculating price, to go back input '_back_'.");
            Console.WriteLine("Format: 'activatedReservationId'");
            Console.WriteLine("Example: 1");
            Console.Write(">");

            var input = Console.ReadLine();
            if (input == "_back_")
            {
                return;
            }

            try
            {
                var param = int.Parse(input.Trim());

                using (var rrcs = new RentalRateCalculatorService())
                {
                    var asyncCost = rrcs.calculateCost(param);
                    asyncCost.Wait();
                    var cost = asyncCost.Result;
                    Console.WriteLine("The cost is: " + cost);
                }
            }
            catch (Exception e)
            {
                printExceptions(e);
            }
        }
        
        static void simulateDriving()
        {
            Console.WriteLine("Driving a few kilometers, to go back input '_back_'.");
            Console.WriteLine("Format: 'carPlate', 'nrKm'");
            Console.WriteLine("Example: EEE111, 10");
            Console.Write(">");

            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            List<string> untrimmedParams = input.Split(',').ToList();
            List<string> parameters = untrimmedParams.Select(p => p.Trim()).ToList();

            if (parameters.Count() != 2)
            {
                Console.WriteLine("Parameter count is incorrect");
                return;
            }

            try
            {
                var carPlate = parameters[0];
                var mileCount = double.Parse(parameters[1]);

                using (var service = new DatabaseAccessService())
                {
                    service.addCarMileage(carPlate, mileCount).Wait();
                    Console.WriteLine("Driving simulated. Kilometers added to " + carPlate + " : " + mileCount);
                }
            }
            catch (Exception e)
            {
                printExceptions(e);
            }

        }

        static void activateReservation()
        {
            Console.WriteLine("Activating reservation, to go back input '_back_'.");
            Console.WriteLine("Format: 'reservationId'");
            Console.WriteLine("Example: 1");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            try
            {
                var param = int.Parse(input.Trim());
                using (var crs = new CarReservationService())
                {
                    var asyncAr = crs.activateReservation(param);
                    asyncAr.Wait();
                    var ar = asyncAr.Result;
                    Console.WriteLine("Reservation with id " + param + " has been activated.");
                    Console.WriteLine("ActivatedReservation id is " + ar.id + ".");
                }

            }
            catch (Exception e)
            {
                printExceptions(e);
            }
        }

        static void deactivateReservation()
        {
            Console.WriteLine("Deactivating activated reservation, to go back input '_back_'.");
            Console.WriteLine("Format: 'activatedReservationId'");
            Console.WriteLine("Example: 1");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            try
            {
                var param = int.Parse(input.Trim());
                using (var crs = new CarReservationService())
                {
                    crs.deactivateActivatedReservation(param).Wait();
                    Console.WriteLine("Reservation with id " + param + " has been deactivated.");
                }

            } catch(Exception e)
            {
                printExceptions(e);
            }
        }

        static void createCar()
        {
            Console.WriteLine("Creating car, to go back input '_back_'.");
            Console.WriteLine("Format: 'carPlateNr', 'carType', 'currentMileage'");
            Console.WriteLine("Example: AAA111, Small car, 150");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            List<string> untrimmedParams = input.Split(',').ToList();
            List<string> parameters = untrimmedParams.Select(p => p.Trim()).ToList();

            if (parameters.Count() != 3)
            {
                Console.WriteLine("Parameter count is incorrect");
                return;
            }

            try
            {
                var carPlateNr = parameters[0];
                var carType = parameters[1];
                var currentMileage = double.Parse(parameters[2]);

                using (var service = new DatabaseAccessService())
                {
                    service.addRentalCar(carPlateNr, carType, currentMileage).Wait();
                    Console.WriteLine("Car with the registration plate: " + carPlateNr + " has been added.");
                }

            } catch(Exception e)
            {
                printExceptions(e);
            }
        }

        static void createCarType()
        {
            Console.WriteLine("Creating car type, to go back input '_back_'.");
            Console.WriteLine("Format: 'carTypeName', 'kmPrice', 'baseDayRentalPrice'");
            Console.WriteLine("Example: Truck, 12, 55");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            List<string> untrimmedParams = input.Split(',').ToList();
            List<string> parameters = untrimmedParams.Select(p => p.Trim()).ToList();

            if (parameters.Count() != 3)
            {
                Console.WriteLine("Parameter count is incorrect");
                return;
            }

            try
            {
                var carType = parameters[0];
                var kmPrice = double.Parse(parameters[1]);
                var baseDayPrice = double.Parse(parameters[2]);

                using (var service = new DatabaseAccessService())
                {
                    service.addRentalCarType(carType, kmPrice, baseDayPrice).Wait();
                    Console.WriteLine("The following car type: " + carType + " has been added.");
                }

            } catch(Exception e)
            {
                printExceptions(e);
            }
        }

        static void createCustomer()
        {
            Console.WriteLine("Creating customer, to go back input '_back_'.");
            Console.WriteLine("Format: 'name', 'dateOfBirth'");
            Console.WriteLine("Example: Cecil, 2015-12-31");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            List<string> untrimmedParams = input.Split(',').ToList();
            List<string> parameters = untrimmedParams.Select(p => p.Trim()).ToList();

            if (parameters.Count() != 2)
            {
                Console.WriteLine("Parameter count is incorrect");
                return;
            }

            try
            {
                var name = parameters[0];
                var dateOfBirth = DateTime.Parse(parameters[1]);

                using (var service = new DatabaseAccessService())
                {
                    service.addCustomer(name, dateOfBirth).Wait();
                    Console.WriteLine("The following customer: " + name + " has been added.");
                }

            } catch(Exception e)
            {
                printExceptions(e);
            }
        }

        static void requestReservation()
        {
            Console.WriteLine("Adding reservation, to go back input '_back_'.");
            Console.WriteLine("Format: 'customerId', 'carTypeName', 'startDate', 'endDate'");
            Console.WriteLine("Example: 1, Small car, 2018-10-19, 2018-11-19");
            Console.Write(">");
            var input = Console.ReadLine();

            if (input == "_back_")
            {
                return;
            }

            List<string> untrimmedParams = input.Split(',').ToList();
            List<string> parameters = untrimmedParams.Select(x => x.Trim()).ToList();

            if (parameters.Count() != 4)
            {
                Console.WriteLine("Input format is incorrect");
                return;
            }

            try
            {
                var id = int.Parse(parameters[0]);
                var carType = parameters[1];
                var startDate = DateTime.Parse(parameters[2]);
                var endDate = DateTime.Parse(parameters[3]);


                using (var service = new CarReservationService())
                {
                    Console.WriteLine("Creating reservation...");
                    var asyncR = service.requestReservation(id, carType, startDate, endDate);
                    asyncR.Wait();
                    var r = asyncR.Result;

                    Console.WriteLine("Reservation successful. The reservation id is:" + r.id);
                }

            }
            catch(Exception e)
            {
                printExceptions(e);
            }
            
        }


        static void printExceptions(Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                printExceptions(e.InnerException);
            }
        }

        static void initialize()
        {
            using (var service = new DatabaseAccessService())
            {
                var customer1 = new BasicCustomer { name = "Linda", dateOfBirth = new DateTime(1990, 1, 1) };
                var customer2 = new BasicCustomer { name = "Klas", dateOfBirth = new DateTime(1992, 1, 1) };
                service.addCustomer(customer1).Wait();
                service.addCustomer(customer2).Wait();

                var smallCar = new RentalCarType { carTypeName = "Small car", basePriceModifier = 1, kmPriceModifier = 0 };
                var van = new RentalCarType { carTypeName = "Van", basePriceModifier = 1.2, kmPriceModifier = 1 };
                var miniBus = new RentalCarType { carTypeName = "Mini bus", basePriceModifier = 1.7, kmPriceModifier = 1.5 };

                service.addRentalCarType(smallCar).Wait();
                service.addRentalCarType(van).Wait();
                service.addRentalCarType(miniBus).Wait();

                service.addRentalCar("DDD111", "Small car", 0).Wait();
                service.addRentalCar("DDD222", "Small car", 0).Wait();
                service.addRentalCar("EEE111", "Van", 0).Wait();
                service.addRentalCar("EEE222", "Van", 0).Wait();
                service.addRentalCar("FFF111", "Mini bus", 0).Wait();
                service.addRentalCar("FFF222", "Mini bus", 0).Wait();

            }

            using (var context = new CarRentalContext())
            {
                RentalRateCalculatorService.setRentalRate(context, 15, 20).Wait();
            }

            Console.WriteLine("Initialize complete");

        }

        static void Main()
        {
            Console.WriteLine("Welcome to the car rental system.");
            Console.WriteLine("If this is your first time using this app, please run _initialize_.");
            displayCommands();

            var input = "";
            while (input != "_exit_") {
                Console.Write(">");
                input = Console.ReadLine();
                switch (input) {
                    //Get
                    case "_commands_":
                        displayCommands();
                        break;
                    case "_customers_":
                        showCustomers();
                        break;
                    case "_carTypes_":
                        showCarTypes();
                        break;
                    case "_cars_":
                        showCars();
                        break;
                    case "_reservations_":
                        showReservations();
                        break;
                    case "_activatedReservations_":
                        showActivatedReservations();
                        break;
                    //Add
                    case "_addCustomer_":
                        createCustomer();
                        break;
                    case "_addCarType_":
                        createCarType();
                        break;
                    case "_addCar_":
                        createCar();
                        break;
                    //Logic
                    case "_requestReservation_":
                        requestReservation();
                        break;
                    case "_activateReservation_":
                        activateReservation();
                        break;
                    case "_deactivateReservation_":
                        deactivateReservation();
                        break;
                    case "_calculatePrice_":
                        calculatePrice();
                        break;
                    case "_simulateDriving_":
                        simulateDriving();
                        break;
                    case "_initialize_":
                        initialize();
                        break;
                    default:
                        Console.WriteLine("Invalid input, see _commands_ for a list of commands");
                        break;
                    
                }
            }
        }

        // Example of how the model can be extended
        static void extendingClass()
        {
            using (var databaseAccess = new DatabaseAccessService(new CarRentalContext()))
            {
                ModelCustomizer.RegisterModelCustomization(
                        test =>
                        {
                            test.Entity<ExtendedCustomer>();
                        });

                var customer = new BasicCustomer { name = "Christie", dateOfBirth = new DateTime(1990, 1, 1) };
                var extendedCustomer = new ExtendedCustomer { name = "Medda", dateOfBirth = new DateTime(1990, 1, 1), someNewValue = "hej" };

                databaseAccess.addCustomer(customer).Wait();
                databaseAccess.addCustomer(extendedCustomer).Wait();
            }
        }
    }
}
