﻿using BlogSystem.Application.Features.Authors.Commands.CreateAuthorCommand;
using BlogSystem.Application.Features.Authors.Commands.DeleteAuthorCommand;
using BlogSystem.Application.Features.Authors.Commands.UpdateAuthorCommand;
using BlogSystem.Application.Features.Authors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorsController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var result = await _mediator.Send(new GetAllAuthorsQuery());

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var result = await _mediator.Send(new GetAuthorByIdQuery(id));

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromForm] CreateAuthorDto authorDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateAuthorCommand(authorDto), cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, [FromForm]UpdateAuthorDto authorDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateAuthorCommand(id, authorDto), cancellationToken);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteAuthorCommand(id), cancellationToken);

            return StatusCode(result.StatusCode, result);
        }
    }
}
