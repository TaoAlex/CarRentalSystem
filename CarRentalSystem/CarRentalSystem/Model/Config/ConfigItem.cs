using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Model.Config
{
    /// <summary>
    /// This class is used to store global configurations for the system.
    /// </summary>
    public class ConfigItem
    {
        [Key]
        public string configName { get; set; }
        [Required]
        public string value { get; set; }
    }
}
