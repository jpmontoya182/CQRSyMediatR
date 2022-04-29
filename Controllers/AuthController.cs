using MediatR;
using Microsoft.AspNetCore.Mvc;
using CQRSyMediatR.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using CQRSyMediatR.Services;

namespace CQRSyMediatR.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthContoller : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthContoller(IMediator mediator)
    {
        _mediator = mediator;
    }    

    [HttpPost]
    public Task<TokenCommandResponse> Token([FromBody] TokenCommand command) 
    {
        return _mediator.Send(command);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me([FromServices] ICurrentUserService currentUser)
    {
        return Ok(new
        {
            currentUser.User,
            IsAdmin = currentUser.IsInRole("Admin")
        });
    }
}