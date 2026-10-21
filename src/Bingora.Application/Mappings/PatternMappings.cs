using Bingora.Application.DTOs.Pattern;
using Bingora.Domain.Entities;

namespace Bingora.Application.Mappings;

public static class PatternMappings
{
    public static PatternResponseDto ToResponseDto(this Pattern p)
        => new PatternResponseDto(
            p.Id,
            p.Name,
            p.GridRepresentation,
            p.Bitmask,
            p.CreatedAt,
            p.IsActive
        );
}