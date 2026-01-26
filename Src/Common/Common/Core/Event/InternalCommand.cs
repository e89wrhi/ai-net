namespace AI.Common.Core;

/// <summary>
/// A base structure for internal commands.
/// It combines the properties of an event (for tracking) with the intent of a command (to do work).
/// </summary>
public record InternalCommand : IInternalCommand, ICommand;