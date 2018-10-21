using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalSystem.Model
{
    /// <summary>
    /// This class represents the cars in the system.
    /// </summary>
    public class RentalCar: IEquatable<RentalCar>
    {
        [Key]
        [RegularExpression("[A-Z]*[0-9]*", 
            ErrorMessage="Car registration plate number can only contain alphabethical and numerical characters")]
        [StringLength(20,
            ErrorMessage="The length of the registration plate number cannot exceed 20 characters")]
        public string carNumber { get; set; }

        [Required]
        public double currentMileage { get; set; }

        [Required]
        public RentalCarType rentalCarType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool Equals(RentalCar car)
        {
            return carNumber == car.carNumber;
        }
   }

}
