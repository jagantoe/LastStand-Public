using LastStand.Game;
using Orleans.Runtime;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace LastStand.Grains;

public interface IGameGrain : IGrainWithStringKey
{
    Task Ping();

    Task<string> GetSerializedGameState();
    Task<string> Command(string command);
    Task<string> SendOut(SendRequest request);
}

public sealed class GameGrain : Grain, IGameGrain
{
    private readonly IPersistentState<GameContainer> _state;
    private readonly IDatabase _cache;
    private readonly ISubscriber _subscriber;

    private readonly string _key;
    private Game.Game Game => _state.State.game;

    private IGrainCollector grainCollector;
    private IDisposable _gameTimer;
    private IDisposable _saveTimer;

    private uint Timer = 3;

    public GameGrain([PersistentState(stateName: "game", storageName: "laststandstore")] IPersistentState<GameContainer> state, ConnectionMultiplexer connectionMultiplexer)
    {
        _state = state;
        _cache = connectionMultiplexer.GetDatabase();
        _subscriber = connectionMultiplexer.GetSubscriber();
        _key = this.GetPrimaryKeyString();
    }

    private async Task StartSaveTimer()
    {
        _saveTimer = RegisterTimer(
            asyncCallback: async state =>
            {
                await WriteState();
            },
            state: this,
            dueTime: TimeSpan.FromMinutes(1),
            period: TimeSpan.FromMinutes(1));
    }
    private async Task StartTimer()
    {
        _gameTimer = RegisterTimer(
            asyncCallback: async state =>
            {
                await RunRound();
            },
            state: this,
            dueTime: TimeSpan.FromSeconds(0),
            period: TimeSpan.FromSeconds(Timer));
    }
    public async Task RestartTimer()
    {
        Timer = Game.Timer;
        _gameTimer?.Dispose();
        await StartTimer();
    }

    public async Task RunRound()
    {
        try
        {
            var timer = Stopwatch.StartNew();
            if (Game.GameActive)
            {
                Game.RunRound();
                SaveGame();
                if (Game.Events.Count > 0) await ProcessEvents();
            }
            timer.Stop();
            Console.WriteLine($"{_key} Round {Game.Round} time: {timer.ElapsedMilliseconds}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{_key} Run round Exception: " + e);
        }
    }
    private async Task ProcessEvents()
    {
        foreach (var @event in Game.Events)
        {
            if (@event is Constants.TimerChanged)
            {
                await RestartTimer();
            }
            else
            {
                // Send event through cache
                _subscriber.Publish($"{_key}-{@event}", DateTime.Now.ToString(), CommandFlags.FireAndForget);
            }
        }
        Game.Events.Clear();
    }

    // Write game state to cache for players to read
    private void SaveGame()
    {
        // Write state to cache for players to read
        _cache.StringSet(_key, JsonSerializer.Serialize(new PlayGameState(Game)), flags: CommandFlags.FireAndForget);
    }
    // Write state to storage for persistance
    private async Task WriteState()
    {
        // Write state to storage
        var timer = Stopwatch.StartNew();
        await _state.WriteStateAsync();
        timer.Stop();
        Console.WriteLine($"Save time: {timer.ElapsedMilliseconds}");
    }

    public Task Ping()
    {
        return Task.CompletedTask;
    }
    public Task<string> GetSerializedGameState()
    {
        return Task.FromResult(JsonSerializer.Serialize(new VisualGameState(Game)));
    }
    public Task<string?> Command(string command)
    {
        try
        {
            Game.HandleCommand(command);
            SaveGame();
            return Task.FromResult<string?>(null);
        }
        catch (InvalidCommandException invalidCommandException)
        {
            return Task.FromResult(invalidCommandException.Message);
        }
        catch (LastStandException lastStandException)
        {
            Console.WriteLine($"{_key } Command {command} Exception: " + lastStandException.Message);
            return Task.FromResult(lastStandException.Message);
        }
    }
    public Task<string?> SendOut(SendRequest request)
    {
        try
        {
            Game.SendOutPlayer(request.PlayerName, request.CombatBehaviour, request.Commands, request.Items);
            SaveGame();
            return Task.FromResult<string?>(null);
        }
        catch (InvalidCommandException invalidCommandException)
        {
            return Task.FromResult(invalidCommandException.Message);
        }
        catch (LastStandException lastStandException)
        {
            Console.WriteLine($"{_key } SendOut {request.PlayerName} Exception: " + lastStandException.Message);
            return Task.FromResult(lastStandException.Message);
        }
    }

    // Register all active grains so we can read and get their state for the frontend
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        grainCollector = GrainFactory.GetGrain<IGrainCollector>(GrainCollectorGrain.Name);
        await grainCollector.Register(this.GetPrimaryKeyString());
        await _state.ReadStateAsync();
        if (_state.State.game == null)
        {
            _state.State.game = new() { Name = _key };
            SaveGame();
        }
        RestartTimer();
        StartSaveTimer();
    }
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _saveTimer?.Dispose();
        _gameTimer?.Dispose();
        await grainCollector.Deregister(this.GetPrimaryKeyString());
        await _state.WriteStateAsync();
    }
}

[GenerateSerializer]
public class SendRequest
{
    public required string PlayerName { get; set; }
    public required CombatBehaviour CombatBehaviour { get; set; }
    public required List<string> Commands { get; set; }
    public required List<string> Items { get; set; }
}