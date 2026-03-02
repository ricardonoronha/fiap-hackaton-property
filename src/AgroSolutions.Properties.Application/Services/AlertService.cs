using AgroSolutions.Properties.Application.DTOs;
using AgroSolutions.Properties.Application.Interfaces;
using AgroSolutions.Properties.Domain.Interfaces;

namespace AgroSolutions.Properties.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;

        public AlertService(IAlertRepository alertRepository) 
        {
            _alertRepository = alertRepository;
        }

        public async Task<AlertDto> GetById(Guid id)
        {
            var alert = await _alertRepository.GetById(id);

            
            if (alert == null)
                return null;

            return new AlertDto(alert.Id, alert.FieldId, alert.Type, alert.StartDate, alert.EndDate, alert.Active);
        }

        public async Task<List<AlertDto>> GetAll(Guid fieldId)
        {
            var alerts = await _alertRepository.GetByFieldIdAsync(fieldId);

            if (alerts == null || !alerts.Any())
                return new List<AlertDto>();

            return alerts.Select(alert => new AlertDto(alert.Id, alert.FieldId, alert.Type, alert.StartDate, alert.EndDate, alert.Active)).ToList();

        }
    }
}
