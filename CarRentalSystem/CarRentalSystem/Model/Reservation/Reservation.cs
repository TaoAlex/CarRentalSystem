using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Model
{
    /// <summary>
    /// This class represents a reservation that has not yet been activated.
    /// No car has been rented out to the customer yet.
    /// </summary>
    public class Reservation: IEquatable<Reservation>
    {
        [Key]
        public int id { get; set; }
        [Required]
        public DateTime rentalDate { get; set; }
        [Required]
        public DateTime expectedReturnDate { get; set; }
        [Required]
        public RentalCarType carType { get; set; }
        [Required]
        public BasicCustomer customer { get; set; }
        public bool cancelled { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        private Reservation()
        {

        }

        public Reservation(
            BasicCustomer customer,
            RentalCarType carType,
            DateTime rentalDate,
            DateTime expectedReturnDate
        )
        {
            this.customer = customer;
            this.rentalDate = rentalDate;
            this.expectedReturnDate = expectedReturnDate;
            this.carType = carType;
            cancelled = false;
        }

        public bool Equals(Reservation other)
        {
            return id == other.id;
        }
    }
}
