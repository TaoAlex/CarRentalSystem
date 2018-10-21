using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CarRentalSystem.Model;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using CarRentalSystem.Model.Config;
using CarRentalSystem.Model.ModelCustomizer;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CarRentalSystem.Context
{
    public class CarRentalContext: DbContext
    {
        public virtual DbSet<RentalCar> rentalCars { get; set; }
        public virtual DbSet<Reservation> reservations { get; set; }
        public virtual DbSet<ActivatedReservation> activatedReservations { get; set; }
        public virtual DbSet<BasicCustomer> basicCustomers { get; set; }
        public virtual DbSet<RentalCarType> carTypes { get; set; }
        public virtual DbSet<ConfigItem> configItems { get; set; }

        public void clearTables()
        {
            var listOfTableNames = Database.SqlQuery<string>("SELECT table_name FROM information_schema.tables").ToList();

            foreach (var tableName in listOfTableNames)
            {

                Database.ExecuteSqlCommand("DELETE FROM " + tableName);
            }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        Console.WriteLine(message);
                    }
                }
            }
            return 0;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelCustomizer.ApplyCustomization(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
