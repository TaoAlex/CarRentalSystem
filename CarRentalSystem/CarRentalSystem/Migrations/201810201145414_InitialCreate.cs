namespace CarRentalSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivatedReservations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        reservationId = c.Int(nullable: false),
                        startMileage = c.Double(nullable: false),
                        endMileage = c.Double(nullable: false),
                        isCurrentlyActive = c.Boolean(nullable: false),
                        actualReturnDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        rentedCar_carNumber = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.RentalCars", t => t.rentedCar_carNumber)
                .ForeignKey("dbo.Reservations", t => t.reservationId)
                .Index(t => t.reservationId, unique: true)
                .Index(t => t.rentedCar_carNumber);
            
            CreateTable(
                "dbo.RentalCars",
                c => new
                    {
                        carNumber = c.String(nullable: false, maxLength: 20),
                        currentMileage = c.Double(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        rentalCarType_carTypeName = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.carNumber)
                .ForeignKey("dbo.RentalCarTypes", t => t.rentalCarType_carTypeName)
                .Index(t => t.rentalCarType_carTypeName);
            
            CreateTable(
                "dbo.RentalCarTypes",
                c => new
                    {
                        carTypeName = c.String(nullable: false, maxLength: 200),
                        basePriceModifier = c.Double(nullable: false),
                        kmPriceModifier = c.Double(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.carTypeName);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        rentalDate = c.DateTime(nullable: false),
                        expectedReturnDate = c.DateTime(nullable: false),
                        cancelled = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        carType_carTypeName = c.String(nullable: false, maxLength: 200),
                        customer_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.RentalCarTypes", t => t.carType_carTypeName)
                .ForeignKey("dbo.BasicCustomers", t => t.customer_id)
                .Index(t => t.carType_carTypeName)
                .Index(t => t.customer_id);
            
            CreateTable(
                "dbo.BasicCustomers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 200),
                        dateOfBirth = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        someNewValue = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ConfigItems",
                c => new
                    {
                        configName = c.String(nullable: false, maxLength: 128),
                        value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.configName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivatedReservations", "reservationId", "dbo.Reservations");
            DropForeignKey("dbo.Reservations", "customer_id", "dbo.BasicCustomers");
            DropForeignKey("dbo.Reservations", "carType_carTypeName", "dbo.RentalCarTypes");
            DropForeignKey("dbo.ActivatedReservations", "rentedCar_carNumber", "dbo.RentalCars");
            DropForeignKey("dbo.RentalCars", "rentalCarType_carTypeName", "dbo.RentalCarTypes");
            DropIndex("dbo.Reservations", new[] { "customer_id" });
            DropIndex("dbo.Reservations", new[] { "carType_carTypeName" });
            DropIndex("dbo.RentalCars", new[] { "rentalCarType_carTypeName" });
            DropIndex("dbo.ActivatedReservations", new[] { "rentedCar_carNumber" });
            DropIndex("dbo.ActivatedReservations", new[] { "reservationId" });
            DropTable("dbo.ConfigItems");
            DropTable("dbo.BasicCustomers");
            DropTable("dbo.Reservations");
            DropTable("dbo.RentalCarTypes");
            DropTable("dbo.RentalCars");
            DropTable("dbo.ActivatedReservations");
        }
    }
}
