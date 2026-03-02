using AgroSolutions.Properties.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Application.DTOs
{
    public record AlertDto(
        Guid Id,
        Guid FieldId,
        AlertType Type,
        DateTime StartDate,
        DateTime? EndDate,
        bool Active
    );
}
