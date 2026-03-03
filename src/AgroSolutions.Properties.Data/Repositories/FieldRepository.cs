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
    public class FieldRepository(PropertiesContext ctx) : IFieldRepository
    {
        public async Task AddAsync(Field field)
        {
            if(field is null)
                throw new ArgumentNullException("Field must be valid");

            await ctx.Fields.AddAsync(field);
            await ctx.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var field = await ctx.Fields.FirstOrDefaultAsync(x => x.Id == id);

            if (field is null)
                return;

            ctx.Fields.Remove(field);
            await ctx.SaveChangesAsync();
        }

        public Task<List<Field>> GetAllAsync()
            => ctx
            .Fields
            .Include(x=> x.Culture)
            .Include(x=> x.Alerts.Where(x=> x.Active))
            .ToListAsync();

        public Task<Field?> GetByIdAsync(Guid id)
            => ctx.Fields
            .Include(x => x.Culture)
            .Include(x => x.Alerts).FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateAsync(Guid id, Field entity)
        {
            var field = await ctx.Fields.FirstOrDefaultAsync(x => x.Id == id);
            
            if (field is null)
                return;
            
            field.Name = entity.Name;
            field.CultureId = entity.CultureId;
            field.PropertyId = entity.PropertyId;
            
            ctx.Fields.Update(field);
            await ctx.SaveChangesAsync();

        }
    }
}
