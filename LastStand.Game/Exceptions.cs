namespace LastStand.Game;

public class LastStandException(string message) : Exception(message) { }

public class InvalidCommandException(string message) : Exception(message) { }