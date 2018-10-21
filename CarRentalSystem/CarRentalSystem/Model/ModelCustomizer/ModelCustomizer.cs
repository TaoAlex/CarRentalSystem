using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Model.ModelCustomizer
{
    /// <summary>
    /// This class is used to make database tables easy to extend.
    /// </summary>
    public class ModelCustomizer
    {
        private static Action<DbModelBuilder> _modelCustomization;

        public static void RegisterModelCustomization(Action<DbModelBuilder> modelCustomization)
        {
            _modelCustomization = modelCustomization;
        }

        internal static void ApplyCustomization(DbModelBuilder modelBuilder)
        {
            if (_modelCustomization != null)
            {
                _modelCustomization(modelBuilder);
            }
        }
    }
}
