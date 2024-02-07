namespace SysBot.Pokemon
{
    /// <summary>
    /// Type of routine the Bot carries out.
    /// </summary>
    public enum PokeRoutineType
    {
        /// <summary> Sits idle waiting to be re-tasked. </summary>
        Idle = 0,

        /// <summary> Similar to idle, but identifies the bot as available for Remote input (Twitch Plays, etc). </summary>
        RemoteControl = 1_000,

        /// <summary> Performs and rotates group battles as a host. </summary>
        RotatingRaidBot = 2_000,
    }

    public static class PokeRoutineTypeExtensions
    {
        public static bool IsRaidBot(this PokeRoutineType type) => type is PokeRoutineType.RotatingRaidBot;
    }
}
