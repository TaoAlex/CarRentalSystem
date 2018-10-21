using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Model
{
    /// <summary>
    /// This class represents the car types and their associated price rates.
    /// </summary>
    public class RentalCarType: IEquatable<RentalCarType>
    {
        [Key]
        [StringLength(200, ErrorMessage="Length of car type name cannot exceed 200 characters.")]
        public string carTypeName { get; set; }

        [Required]
        public double basePriceModifier { get; set; }

        [Required]
        public double kmPriceModifier { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool Equals(RentalCarType other)
        {
            return carTypeName == other.carTypeName;
        }
    }
}
