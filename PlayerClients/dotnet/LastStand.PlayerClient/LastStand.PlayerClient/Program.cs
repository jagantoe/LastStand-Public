// See https://aka.ms/new-console-template for more information
using LastStand.Helpers;

var client = new LastStandClient();
var cache = new LastStandCache();
var sub = cache._subscriber;
// A player has returned and is available again
sub.Subscribe($"{Setup.User}-{Constants.PlayerReturned}").OnMessage(x =>
{
    Console.WriteLine(x);
});
// Attackers have appeared
sub.Subscribe($"{Setup.User}-{Constants.AttackerAppeared}").OnMessage(x =>
{
    Console.WriteLine(x);
});
// Your base is being attacked
sub.Subscribe($"{Setup.User}-{Constants.BaseAttacked}").OnMessage(x =>
{
    Console.WriteLine(x);
});
// Game over
sub.Subscribe($"{Setup.User}-{Constants.GameEnded}").OnMessage(x =>
{
    Console.WriteLine(x);
});

Console.ReadLine();