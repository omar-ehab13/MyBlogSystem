namespace BlogSystem.Application.DTOs;

public record BlogDto(
    Guid Id,
    string Title,
    string Body,
    Guid AuthorId,
    string AuthorName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateBlogDto(
    string Title,
    string Body,
    Guid AuthorId
);

public record UpdateBlogDto(
    string Title,
    string Body
);
