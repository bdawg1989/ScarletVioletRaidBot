using Discord.WebSocket;
using SysBot.Base;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public static class ReusableActions
    {
        public static async Task EchoAndReply(this ISocketMessageChannel channel, string msg)
        {
            // Announce it in the channel the command was entered only if it's not already an echo channel.
            EchoUtil.Echo(msg);
            if (!EchoModule.IsEchoChannel(channel) || !EchoModule.IsEmbedEchoChannel(channel))
                await channel.SendMessageAsync(msg).ConfigureAwait(false);
        }

        public static string StripCodeBlock(string str) => str.Replace("`\n", "").Replace("\n`", "").Replace("`", "").Trim();
    }
}