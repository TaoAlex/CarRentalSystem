using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Model
{
    /*
     * This class is used to demonstrate how 
     * the model can be extended.
     */
    public class ExtendedCustomer: BasicCustomer
    {
        [Required]
        public string someNewValue { get; set; }
    }
}
