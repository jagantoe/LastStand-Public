using LastStand.Grains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LastStand.APIBase.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayController : Controller
{
    private readonly byte[] _key;
    private readonly IGrainFactory _grainFactory;
    public PlayController([FromKeyedServices("token-key")] byte[] key, IGrainFactory grainFactory)
    {
        _key = key;
        _grainFactory = grainFactory;
    }

    [HttpGet]
    [Route("Token/{name}")]
    public async Task<ActionResult> GetToken([FromRoute] string name)
    {
        var token = GenerateToken(name);
        await _grainFactory.GetGrain<IGameGrain>(name).Ping();
        return Ok(token);
    }

    [Authorize]
    [HttpPost]
    [Route("Command")]
    public async Task<ActionResult> Command([FromBody] CommandRequest request)
    {
        var name = User.GetUserName();
        var response = await _grainFactory.GetGrain<IGameGrain>(name).Command(request.Command);
        return Ok(response);
    }
    public record CommandRequest(string Command);

    [Authorize]
    [HttpPost]
    [Route("Send")]
    public async Task<ActionResult> Send([FromBody] SendRequest request)
    {
        var name = User.GetUserName();
        var response = await _grainFactory.GetGrain<IGameGrain>(name).SendOut(request);
        return Ok(response);
    }

    private string GenerateToken(string name)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name)
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
public static class UserExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.Claims.First(x => x.Type == ClaimTypes.Name).Value;
    }
}