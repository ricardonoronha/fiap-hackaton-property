using AgroSolutions.Properties.Application.DTOs;
using AgroSolutions.Properties.Application.Interfaces;
using AgroSolutions.Properties.Domain.Entities;
using AgroSolutions.Properties.Domain.Interfaces;

namespace AgroSolutions.Properties.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }
        public async Task AddAsync(PropertyInputDto property)
        {
            if(property is null)
                throw new ArgumentException("Property input cannot be null.");

            var propertyEntity = new Property()
            {
                Name = property.Name,
                Location = property.Location,
                CreatedAt = DateTime.UtcNow,
                FarmerId = property.FarmerId!
            };

            await _propertyRepository.AddAsync(propertyEntity);
        }

        public async Task<IEnumerable<PropertyOutputDto>> GetAllAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();
            if (properties is null || !properties.Any())
                return Enumerable.Empty<PropertyOutputDto>();

            return properties.Select(p => new PropertyOutputDto
            {
                Id = p.Id,
                Name = p.Name,
                Location = p.Location,
                CreatedAt = p.CreatedAt,
                FarmerId = p.FarmerId
            });

        }

        public async Task<PropertyOutputDto> GetByIdAsync(Guid id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property is null)
                return null;

            return new PropertyOutputDto
            {
                Id = property.Id,
                Name = property.Name,
                Location = property.Location,
                CreatedAt = property.CreatedAt,
                FarmerId = property.FarmerId
            };
        }

        public async Task RemoveAsync(Guid id)
        {   
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property is null)
                return;

            await _propertyRepository.DeleteAsync(property.Id);
        }

        public async Task<PropertyOutputDto> UpdateAsync(Guid id, PropertyInputDto property)
        {   
            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty is null)
                return null;

            existingProperty.Name = property.Name;
            existingProperty.Location = property.Location;
            
            await _propertyRepository.UpdateAsync(existingProperty.Id, existingProperty);
            return new PropertyOutputDto
            {
                Id = existingProperty.Id,
                Name = existingProperty.Name,
                Location = existingProperty.Location,
                CreatedAt = existingProperty.CreatedAt,
                FarmerId = existingProperty.FarmerId

            };
        }
    }
}
