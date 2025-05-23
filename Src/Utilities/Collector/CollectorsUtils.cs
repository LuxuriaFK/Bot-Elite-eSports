﻿/*
 * Arquivo: CollectorsUtils.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 4-10-2021
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#pragma warning disable 1998

namespace BOTDiscord.Utilities.Collector
{
    public class CollectorsUtils
    {
        private DiscordSocketClient _client;

        public CollectorsUtils(DiscordSocketClient client)
        {
            _client = client;
            //if (Program.CmdOptions.Observer) 
            //  return;
            _client.ReactionAdded += ClientOnReactionAdded;
            _client.MessageReceived += ClientOnMessageReceived;
        }

        private async Task ClientOnMessageReceived(SocketMessage arg)
        {
            new Thread(o =>
            {
                if (MessageByChannel.TryGetValue(arg.Channel.Id, out var messageChannels))
                    ProcessMessage(arg, messageChannels.ToList());

                if (MessageByUser.TryGetValue(arg.Author.Id, out var messageUsers))
                    ProcessMessage(arg, messageUsers.ToList());
            }).Start();
        }

        private void ProcessMessage(IMessage message, IEnumerable<KeyValuePair<Guid, (Predicate<IMessage>, Action<IMessage>)>> dictionary)
        {
            foreach (var i in dictionary.Where(pair => pair.Value.Item1(message)))
            {
                i.Value.Item2(message);
                //logger.Swallow(() =>);
            }
        }


        private async Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            new Thread(o =>
            {
                if (ReactionByChannel.TryGetValue(arg3.Channel.Id, out var reactionChannels))
                    ProcessReactions(arg3, reactionChannels.ToList());

                if (ReactionByEmote.TryGetValue(arg3.Emote, out var reactionEmotes))
                    ProcessReactions(arg3, reactionEmotes.ToList());

                if (ReactionByMessage.TryGetValue(arg3.MessageId, out var reactionMessages))
                    ProcessReactions(arg3, reactionMessages.ToList());

                if (ReactionByUser.TryGetValue(arg3.UserId, out var reactionUsers))
                    ProcessReactions(arg3, reactionUsers.ToList());
            }).Start();
        }

        private void ProcessReactions(SocketReaction reaction,
                                             IEnumerable<KeyValuePair<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>> dictionary)
        {
            foreach (var i in dictionary.Where(pair => pair.Value.Item1(reaction)))
            {
                i.Value.Item2(reaction);
                //logger.Swallow(() => );
            }
        }

        #region Collect reaction by channel

        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>> ReactionByChannel =
            new();

        public CollectorController CollectReaction(IChannel channel, Predicate<SocketReaction> predicate,
                                                          Action<EmoteCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!ReactionByChannel.TryGetValue(channel.Id, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    ReactionByChannel.TryRemove(channel.Id, out _);
                }
            };
            var concurrentDictionary = ReactionByChannel.GetOrAdd(channel.Id,
                arg => new ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>());
            concurrentDictionary.TryAdd(key, (ApplyFilters(predicate, filter), reaction => action(new EmoteCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect reaction by emote

        private readonly ConcurrentDictionary<IEmote, ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>> ReactionByEmote =
            new();

        public CollectorController CollectReaction(IEmote emote, Predicate<SocketReaction> predicate,
                                                          Action<EmoteCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!ReactionByEmote.TryGetValue(emote, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    ReactionByEmote.TryRemove(emote, out _);
                }
            };
            var concurrentDictionary = ReactionByEmote.GetOrAdd(emote,
                arg => new ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>());
            concurrentDictionary.TryAdd(key, (ApplyFilters(predicate, filter), reaction => action(new EmoteCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect reaction by command

        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>> ReactionByMessage =
            new();

        public CollectorController CollectReaction(IMessage message, Predicate<SocketReaction> predicate,
                                                          Action<EmoteCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!ReactionByMessage.TryGetValue(message.Id, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    ReactionByMessage.TryRemove(message.Id, out _);
                }
            };
            var concurrentDictionary = ReactionByMessage.GetOrAdd(message.Id,
                arg => new ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>());
            concurrentDictionary.TryAdd(key, (ApplyFilters(predicate, filter), reaction => action(new EmoteCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect reaction by user

        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>> ReactionByUser =
            new();

        public CollectorController CollectReaction(IUser user, Predicate<SocketReaction> predicate,
                                                          Action<EmoteCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!ReactionByUser.TryGetValue(user.Id, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    ReactionByUser.TryRemove(user.Id, out _);
                }
            };
            var concurrentDictionary = ReactionByUser.GetOrAdd(user.Id,
                arg => new ConcurrentDictionary<Guid, (Predicate<SocketReaction>, Action<SocketReaction>)>());
            concurrentDictionary.TryAdd(key, (ApplyFilters(predicate, filter), reaction => action(new EmoteCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect messages by user

        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, (Predicate<IMessage>, Action<IMessage>)>> MessageByUser =
            new();

        public CollectorController CollectMessage(IUser user, Predicate<IMessage> predicate,
                                                         Action<MessageCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!MessageByUser.TryGetValue(user.Id, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    MessageByUser.TryRemove(user.Id, out _);
                }
            };
            var concurrentDictionary = MessageByUser.GetOrAdd(user.Id,
                arg => new ConcurrentDictionary<Guid, (Predicate<IMessage>, Action<IMessage>)>());
            concurrentDictionary.TryAdd(key,
                (ApplyFilters(predicate, filter), reaction => action(new MessageCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect messages by channel

        private ConcurrentDictionary<ulong, ConcurrentDictionary<Guid, (Predicate<IMessage>, Action<IMessage>)>> MessageByChannel =
            new();

        public CollectorController CollectMessage(IChannel channel, Predicate<IMessage> predicate,
                                                         Action<MessageCollectorEventArgs> action, CollectorFilter filter = CollectorFilter.Off)
        {
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!MessageByChannel.TryGetValue(channel.Id, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    MessageByChannel.TryRemove(channel.Id, out _);
                }
            };
            var concurrentDictionary = MessageByChannel.GetOrAdd(channel.Id,
                arg => new ConcurrentDictionary<Guid, (Predicate<IMessage>, Action<IMessage>)>());
            concurrentDictionary.TryAdd(key,
                (ApplyFilters(predicate, filter), reaction => action(new MessageCollectorEventArgs(collectorController, reaction))));
            return collectorController;
        }

        #endregion

        #region Collect commands

        private readonly ConcurrentDictionary<CommandInfo, ConcurrentDictionary<Guid, (Func<ICommandContext, CommandMatch, bool>,
            Action<IMessage, KeyValuePair<CommandMatch, ParseResult>, ICommandContext>)>> ByCommand =
            new();

        public CollectorController CollectCommand([NotNull] CommandInfo info, Func<ICommandContext, CommandMatch, bool> predicate,
                                                         Action<CommandCollectorEventArgs> action)
        {
            info ??= default!;
            var collectorController = new CollectorController();
            var key = Guid.NewGuid();
            collectorController.Stop += (sender, args) =>
            {
                if (!ByCommand.TryGetValue(info, out var value)) return;
                value.TryRemove(key, out _);
                if (value.IsEmpty)
                {
                    ByCommand.TryRemove(info, out _);
                }
            };
            var concurrentDictionary = ByCommand.GetOrAdd(info,
                arg =>
                    new ConcurrentDictionary<Guid, (Func<ICommandContext, CommandMatch, bool>,
                        Action<IMessage, KeyValuePair<CommandMatch, ParseResult>, ICommandContext>)>());
            concurrentDictionary.TryAdd(key,
                (predicate, (message, pair, arg3) => action(new CommandCollectorEventArgs(collectorController, message, pair, arg3))));
            return collectorController;
        }

        /// <summary>
        /// Method for CommandHandler, do not use!
        /// </summary>
        /// <returns>A value indicating whether to execute a command or not</returns>
        public bool OnCommandExecute(KeyValuePair<CommandMatch, ParseResult> info, ICommandContext context, IMessage message)
        {
            if (!ByCommand.TryGetValue(info.Key.Command, out var commandRequests)) return true;
            var keyValuePairs = commandRequests.ToList().Where(pair => pair.Value.Item1(context, info.Key)).ToList();
            foreach (var i in keyValuePairs)
            {
                i.Value.Item2(message, info, context);
                //logger.Swallow(() => i.Value.Item2(message, info, context));
            }

            return keyValuePairs.Count == 0;
        }

        #endregion

        private Predicate<IMessage> ApplyFilters(Predicate<IMessage> initial, CollectorFilter filter)
        {
            return filter switch
            {
                CollectorFilter.Off => initial,
                CollectorFilter.IgnoreSelf => (message => message.Author.Id != _client.CurrentUser.Id && initial(message)),
                CollectorFilter.IgnoreBots => (message => !message.Author.IsBot && !message.Author.IsWebhook && initial(message)),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
            };
        }

        private Predicate<SocketReaction> ApplyFilters(Predicate<SocketReaction> initial, CollectorFilter filter)
        {
            return filter switch
            {
                CollectorFilter.Off => initial,
                CollectorFilter.IgnoreSelf => (reaction => reaction.UserId != _client.CurrentUser.Id && initial(reaction)),
                CollectorFilter.IgnoreBots => (reaction => !reaction.User.Value.IsBot && !reaction.User.Value.IsWebhook && initial(reaction)),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
            };
        }
    }
}