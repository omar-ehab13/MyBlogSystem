using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogSystem.Application.Features.Blogs.Queries;
using BlogSystem.Application.Features.Blogs.Commands;
using BlogSystem.Application.DTOs;

namespace BlogSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBlogs(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllBlogsQuery(), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBlogById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBlogByIdQuery(id), cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    //[HttpPost]
    //public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto blogDto, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(new CreateBlogCommand(blogDto), cancellationToken);
    //    return StatusCode(result.StatusCode, result);
    //}

    //[HttpPut("{id:guid}")]
    //public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] UpdateBlogDto blogDto, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(new UpdateBlogCommand(id, blogDto), cancellationToken);
    //    return StatusCode(result.StatusCode, result);
    //}

    //[HttpDelete("{id:guid}")]
    //public async Task<IActionResult> DeleteBlog(Guid id, CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(new DeleteBlogCommand(id), cancellationToken);
    //    return StatusCode(result.StatusCode, result);
    //}
}