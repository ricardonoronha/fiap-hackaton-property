using AgroSolutions.Properties.Application.DTOs;

namespace AgroSolutions.Properties.Application.Interfaces
{
    public interface IAlertService
    {
        Task<AlertDto> GetById(Guid id);
        Task<List<AlertDto>> GetAll(Guid fieldId);
    }
}
