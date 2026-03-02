using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Domain.Entities
{
    public class Alert
    {
        public Guid Id { get; set; }
        public string SensorType { get; set; }
        public Guid FieldId { get; set; }
        public Field Field { get; set; }

        public AlertType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Active { get; set; }
    }

    public enum AlertType 
    {
        Drought,
        Pest
    }
}
