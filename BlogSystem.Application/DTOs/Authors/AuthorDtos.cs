public record AuthorDto(
    Guid Id,
    string Name,
    string Email,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateAuthorDto(
    string Name,
    string Email,
    string? ImageUrl = null
);

public record UpdateAuthorDto(
    string Name,
    string Email,
    string? ImageUrl = null
);
