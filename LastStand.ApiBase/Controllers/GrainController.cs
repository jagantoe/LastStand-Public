using LastStand.Grains;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LastStand.APIBase.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class GrainController : Controller
{
    private readonly IGrainFactory grainFactory;
    public GrainController(IGrainFactory grainFactory)
    {
        this.grainFactory = grainFactory;
    }

    [HttpGet]
    [Route("GetGames")]
    public async Task<ActionResult> GetActiveGames()
    {
        return Ok(await grainFactory.GetGrain<IGrainCollector>(GrainCollectorGrain.Name).Get());
    }

    [HttpGet]
    [Route("GetState/{name}")]
    public async Task<ActionResult> GetGameState([FromRoute] string name)
    {
        var serializedState = await grainFactory.GetGrain<IGameGrain>(name).GetSerializedGameState();
        var state = JsonSerializer.Deserialize<VisualGameState>(serializedState);
        return Ok(state);
    }
}
