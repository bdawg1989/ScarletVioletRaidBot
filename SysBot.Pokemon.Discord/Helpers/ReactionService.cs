using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReactionService
{
    private readonly DiscordSocketClient _client;
    private readonly Dictionary<ulong, Func<SocketReaction, Task>> _reactionActions;

    public ReactionService(DiscordSocketClient client)
    {
        _client = client;
        _reactionActions = new Dictionary<ulong, Func<SocketReaction, Task>>();

        // Subscribe to the reaction added event
        _client.ReactionAdded += OnReactionAddedAsync;
    }

    public void AddReactionHandler(ulong messageId, Func<SocketReaction, Task> handler)
    {
        _reactionActions[messageId] = handler;
    }

    public void RemoveReactionHandler(ulong messageId)
    {
        _reactionActions.Remove(messageId);
    }

    private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
    {
        if (_reactionActions.TryGetValue(reaction.MessageId, out var handler))
        {
            await handler(reaction);
        }
    }
}