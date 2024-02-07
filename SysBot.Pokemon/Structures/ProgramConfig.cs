using SysBot.Base;

namespace SysBot.Pokemon
{
    public class ProgramConfig : BotList<PokeBotState>
    {
        public ProgramMode Mode { get; set; } = ProgramMode.SV;
        public PokeRaidHubConfig Hub { get; set; } = new();
    }

    public enum ProgramMode
    {
        None = 0, // invalid
        SV = 4,
    }
}
