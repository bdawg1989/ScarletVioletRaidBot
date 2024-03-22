using PKHeX.Core;
using SysBot.Pokemon.SV.BotRaid;
using System;

namespace SysBot.Pokemon
{
    public class BotFactory9SV : BotFactory<PK9>
    {
        public override PokeRoutineExecutorBase CreateBot(PokeRaidHub<PK9> Hub, PokeBotState cfg) => cfg.NextRoutineType switch
        {
            PokeRoutineType.RotatingRaidBot => new RotatingRaidBotSV(cfg, Hub),
            PokeRoutineType.RemoteControl => new RemoteControlBotSV(cfg),
            _ => throw new ArgumentException(nameof(cfg.NextRoutineType)),
        };

        public override bool SupportsRoutine(PokeRoutineType type) => type switch
        {
            PokeRoutineType.RotatingRaidBot => true,
            PokeRoutineType.RemoteControl => true,
            _ => false,
        };
    }
}