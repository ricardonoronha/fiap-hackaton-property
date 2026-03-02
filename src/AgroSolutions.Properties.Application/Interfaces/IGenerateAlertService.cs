using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Application.Interfaces
{
    public interface IGenerateAlertService
    {
        Task UpdateAlertsByReadings(List<SensorReadingDto> readings);
    }
}
