using MelonLoader;
using System.Diagnostics.CodeAnalysis;

namespace BloomEngine.Core;

/// <summary>
/// Internal static class for logging messages to the MelonLoader console with optional prefixes and severity levels.
/// </summary>
internal static class BloomLogger
{
    /// <summary>
    /// Gets the logger instance, which is set on load.
    /// </summary>
    internal static MelonLogger.Instance Logger { get; set; } = null!;

    /// <summary>
    /// Logs a debug message to the MelonLoader console if running in DEBUG mode.
    /// The message is prefixed with an optional string and displayed in a gray color.
    /// </summary>
    /// <param name="msg">The string to log.</param>
    /// <param name="prefix">The prefix for the log message. Prepended directly before the message.</param>
    public static void Debug(string msg, string prefix = "")
    {
#if DEBUG
        Logger?.Msg(ColorARGB.Gray, prefix + msg);
#endif
    }

    /// <summary>
    /// Logs a standard info message to the MelonLoader console.
    /// The message is prefixed with an optional string.
    /// </summary>
    /// <param name="msg">The string to log.</param>
    /// <param name="prefix">The prefix for the log message. Prepended directly before the message.</param>
    public static void Info(string msg, string prefix = "") => Logger?.Msg(prefix + msg);

    /// <summary>
    /// Logs a warning message to the MelonLoader console.
    /// The message is prefixed with an optional string.
    /// </summary>
    /// <param name="msg">The string to log.</param>
    /// <param name="prefix">The prefix for the log message. Prepended directly before the message.</param>
    public static void Warn(string msg, string prefix = "") => Logger?.Warning(prefix + msg);

    /// <summary>
    /// Logs an error message to the MelonLoader console.
    /// The message is prefixed with an optional string.
    /// </summary>
    /// <param name="msg">The string to log.</param>
    /// <param name="prefix">The prefix for the log message. Prepended directly before the message.</param>
    public static void Error(string msg, string prefix = "") => Logger?.Error(prefix + msg);

    /// <summary>
    /// Checks whether a condition is true, returning true or logging the provided message as either a warning or an error otherwise.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="msg">The message to log if the condition is false.</param>
    /// <param name="prefix">The prefix for the log message. Prepended directly before the message.</param>
    /// <param name="warnInsteadOfError">Indicates whether to log as a warning instead of an error.</param>
    /// <returns>true if the condition is true, false otherwise.</returns>
    public static bool Assert([NotNullWhen(true)] bool condition, string msg, string prefix = "", bool warnInsteadOfError = false)
    {
        if (condition)
            return true;

        if (warnInsteadOfError)
            BloomLogger.Warn(msg, prefix);
        else BloomLogger.Error(msg, prefix);

        return false;
    }
}