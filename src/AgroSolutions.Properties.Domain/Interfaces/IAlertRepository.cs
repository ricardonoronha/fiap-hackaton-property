using AgroSolutions.Properties.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Domain.Interfaces
{
    public interface IAlertRepository
    {
        Task<Alert> GetById(Guid id);
        Task<List<Alert>> GetByFieldIdAsync(Guid fieldId);
        Task Add(Alert alert);
        Task Update(Guid id, Alert alert);

    }
}
