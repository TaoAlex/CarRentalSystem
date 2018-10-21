using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Model
{
    public class BasicCustomer: IEquatable<BasicCustomer>
    {
        public int id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage= "Length of name cannot exceed 200 characters")]
        public string name { get; set; }

        [Required]
        public DateTime dateOfBirth { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool Equals(BasicCustomer other)
        {
            return id == other.id;
        }
    }
}
