using SysBot.Base;
using SysBot.Pokemon.SV;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SysBot.Pokemon
{
    public class RemoteControlBotSV(PokeBotState cfg) : PokeRoutineExecutor9SV(cfg)
    {
        public override async Task MainLoop(CancellationToken token)
        {
            try
            {
                Log("Identifying trainer data of the host console.");
                await IdentifyTrainer(token).ConfigureAwait(false);

                Log("Starting main loop, then waiting for commands.");
                Config.IterateNextRoutine();
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1_000, token).ConfigureAwait(false);
                    ReportStatus();
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }

            Log($"Ending {nameof(RemoteControlBotSV)} loop.");
            await HardStop().ConfigureAwait(false);
        }

        public override async Task HardStop()
        {
            await SetStick(SwitchStick.LEFT, 0, 0, 0_500, CancellationToken.None).ConfigureAwait(false); // reset
            await CleanExit(CancellationToken.None).ConfigureAwait(false);
        }

        public override async Task RebootReset(CancellationToken t)
        {
            await ReOpenGame(new PokeRaidHubConfig(), t).ConfigureAwait(false);
            await HardStop().ConfigureAwait(false);

            await Task.Delay(2_000, t).ConfigureAwait(false);
            if (!t.IsCancellationRequested)
            {
                Log("Restarting the main loop.");
                await MainLoop(t).ConfigureAwait(false);
            }
        }

        public override async Task RefreshMap(CancellationToken t)
        {
            await ReOpenGame(new PokeRaidHubConfig(), t).ConfigureAwait(false);
        }

        private class DummyReset : IBotStateSettings
        {
            public bool ScreenOff => true;
        }
    }
}