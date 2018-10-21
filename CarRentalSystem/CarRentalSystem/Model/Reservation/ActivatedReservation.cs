using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Model
{
    /// <summary>
    /// This class represents a reservation that has been
    /// activated. In other words, the car in this class is either:
    /// A: Currently being rented out, or B: Has been rented out and returned back
    /// </summary>
    public class ActivatedReservation: IEquatable<ActivatedReservation>
    {
        [Key]
        public int id { get; set; }
        [Required]
        public RentalCar rentedCar { get; set; }

        [Required]
        [Index(IsUnique = true)]    
        public int reservationId { get; set; }
        public Reservation reservation { get; set; }

        [Required]
        public double startMileage { get; set; }
        public double endMileage { get; set; }

        [Required]
        public bool isCurrentlyActive { get; set; }
        public DateTime? actualReturnDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        private ActivatedReservation()
        {

        }

        public ActivatedReservation(Reservation r, RentalCar rc, double startMileage, bool isCurrentlyActive)
        {
            reservation = r;
            rentedCar = rc;
            this.startMileage = startMileage;
            this.isCurrentlyActive = isCurrentlyActive;
        }

        public void returnCar()
        {
            isCurrentlyActive = false;
            actualReturnDate = DateTime.Now;
            endMileage = rentedCar.currentMileage;
        }

        public bool Equals(ActivatedReservation other)
        {
            return id == other.id;
        }
    }
}
