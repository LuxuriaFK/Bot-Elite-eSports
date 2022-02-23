/*
 * Arquivo: SetStatusCommand.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Managers;
using BOTDiscord.Utilities;
using BOTDiscord.Utilities.Collector;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.DEV
{
    [Grouping("devgroup")]
    [RequireOwner(ErrorMessage = "RequireOwner")]
    public class SetStatusCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _handler;
        private readonly CollectorsUtils _collectorsUtils;
        private readonly IServiceProvider _service;
        private readonly IEmote[] Emojis = { new Emoji("🟢"), new Emoji("🟠"), new Emoji("🔴") };

        public SetStatusCommand(DiscordSocketClient client, CommandHandler commandhandler, CollectorsUtils collectorsUtils, IServiceProvider service)
        {
            _client = client;
            _handler = commandhandler;
            _collectorsUtils = collectorsUtils;
            _service = service;
        }

        [Command("setstatus")]
        [Summary("SetStatusBot0")]
        internal async Task Run([Summary("SetStatusBot0_0s")][Optional] string emoji)
        {
            await Context.Channel.TriggerTypingAsync();
            EmbedBuilder embedBuilder = new()
            {
                Color = Constants.DiscordBlurple
            };
#warning Ajustar erros de incompactibilidade quando atualizar a biblioteca
            if (emoji is null || /*!Emoji.TryParse(emoji, out Emoji emojiPased) || !Emojis.Contains(Emoji.Parse(emoji))*/ !Emojis.Any(x=> x.Name == emoji)) //Só na Versao 3
            {
                embedBuilder.WithDescription(Loc.Get("Status.Title"));
                var message = await ReplyAsync(embed: embedBuilder.Build(), reply: true);

                CollectorsGroup collectors = null!; 
                    
                   collectors = new CollectorsGroup(Emojis.Select(
                    pair =>
                    {
                        return _collectorsUtils.CollectReaction(message, reaction => reaction.UserId == Context.Message.Author.Id && reaction.Emote.Equals(pair), async args =>
                        {
                            var t = await _handler.ExecuteCommand($"setstatus {pair}", Context, args.Reaction.UserId.ToString(), _service);
                            if (t.IsSuccess)
                            {
                                message.SafeDelete();
                                collectors?.DisposeAll();
                            }
                        }, CollectorFilter.IgnoreBots);
                    }));
                collectors?.SetTimeoutToAll(Constants.StandardTimeSpan);

                try
                {
                    await message.AddReactionsAsync(Emojis);
                }
                catch { }
            }
            else
            {
                UserStatus status = Array.IndexOf(Emojis, new Emoji(emoji) /*emojiPased*/) switch
                {
                    0 => UserStatus.Online,
                    1 => UserStatus.AFK,
                    2 => UserStatus.DoNotDisturb,
                    _ => throw new NotImplementedException()
                };

                if (Instancia._status[0].userStatus != status)
                {
                    Instancia._status[0].userStatus = status;
                    await SystemParamsManager.SaveBotStatus();
                    await _client.SetStatusAsync(status);
                }

                embedBuilder.WithDescription(Loc.Get("Status.Success", emoji/*emojiPased*/, status));
                await ReplyAsync(embed: embedBuilder.Build(), reply: true);
            }
        }
    }
}
