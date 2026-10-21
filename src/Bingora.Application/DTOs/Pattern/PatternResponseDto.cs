namespace Bingora.Application.DTOs.Pattern;

public record PatternResponseDto(
    Guid Id,
    string Name,
    string GridRepresentation,
    int Bitmask,
    DateTime CreatedAt,
    bool IsActive
);