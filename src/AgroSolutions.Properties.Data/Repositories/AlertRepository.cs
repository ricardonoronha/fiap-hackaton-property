using AgroSolutions.Properties.Domain.Entities;
using AgroSolutions.Properties.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Data.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly PropertiesContext _ctx;
        public AlertRepository(PropertiesContext ctx)
        {
            _ctx = ctx;
        }
        public async Task Add(Alert alert)
        {
            if(alert == null) 
                throw new ArgumentNullException("Alert must be provided.");

            _ctx.Alerts.Add(alert);
        }

        public async Task<List<Alert>> GetByFieldIdAsync(Guid fieldId)
        {
            var field = _ctx.Fields.Include(f => f.Alerts).FirstOrDefault(f => f.Id == fieldId);
            if (field == null)
                throw new KeyNotFoundException("Field not found.");

            return field.Alerts.ToList();
        }

        public async Task<Alert> GetById(Guid id)
        {
            var alert = await _ctx.Alerts.FirstOrDefaultAsync(x => x.Id == id);
            return alert;
        }

        public async Task Update(Guid id, Alert alert)
        {
            var alertEntity = await _ctx.Alerts.FirstOrDefaultAsync(x => x.Id == id);

            if (alertEntity == null)
                throw new KeyNotFoundException("Alert not found.");

            alertEntity.Type = alert.Type;
            alertEntity.FieldId = alert.FieldId;
            alertEntity.StartDate = alert.StartDate;
            alertEntity.EndDate = alert.EndDate;
            alertEntity.Active  = alert.Active;

            _ctx.Alerts.Add(alertEntity);
            await _ctx.SaveChangesAsync();
        }
    }
}
