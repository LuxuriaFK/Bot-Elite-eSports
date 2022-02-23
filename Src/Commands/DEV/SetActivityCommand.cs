/*
 * Arquivo: SetActivityCommand.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Managers;
using BOTDiscord.Models;
using BOTDiscord.Utilities;
using BOTDiscord.Utilities.Collector;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.DEV
{
    [Grouping("devgroup")]
    [RequireOwner(ErrorMessage = "RequireOwner")]
    public class SetActivityCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _handler;
        private readonly CollectorsUtils _collectorsUtils;
        private readonly IServiceProvider _service;

        public SetActivityCommand(DiscordSocketClient client, CommandHandler commandhandler, CollectorsUtils collectorsUtils, IServiceProvider service)
        {
            _client = client;
            _handler = commandhandler;
            _collectorsUtils = collectorsUtils;
            _service = service;
        }

        [Command("activitys")]
        [Summary("ActivityListBot0")]
        internal async Task Run() => await ActivityList();

        [Command("removeactivity")]
        [Summary("ActivityRemoveBot0")]
        internal async Task Run([Summary("RemoveActivityBot0_0s")] int id) => await RemoveActivity(id);

        [Command("addactivity+")]
        [Summary("AddActivityBot1")]
        internal async Task RunWizard() => await AddActivityWizad();

        [Command("addactivity")]
        [Summary("AddActivityBot0")]
        internal async Task Run([Summary("AddActivityBot0_0s")][Remainder] string gamename) => await AddActivity(gamename);

        [Command("addactivity")]
        [Summary("AddActivityBot0")]
        internal async Task Run([Summary("AddActivityBot0_1s")] int millisecond, [Summary("AddActivityBot0_0s")][Remainder] string gamename) => await AddActivity(gamename, awaits: millisecond);

        private async Task ActivityList()
        {
            await Context.Channel.TriggerTypingAsync();
            EmbedBuilder embed = new()
            {
                Title = "Lista de Atividades",
                Color = Constants.DiscordBlurple
            };
            if (!Instancia._status.Any(x => x.Key != 0))
                embed.WithDescription($"Nenhuma atividade cadastrada!\n\nUse ``{Context.Guild.GetConfigAsync().Result.Prefix}help addactivity`` para mais informações.");
            else
            {
                foreach (KeyValuePair<int, BOTStatusModel> st in Instancia._status.Where(x => x.Key != 0))
                {
                    embed.AddField($"ID: {st.Key} - Activity Type: {st.Value.Activity}",
                        st.Value.Status?.SafeSubstring(50, "..."), false);
                }
                embed.WithFooter($"Use {Context.Guild.GetConfigAsync().Result.Prefix}removeactivity ID para excluir");
            }
            await ReplyAsync(embed: embed.Build(), reply: true);
        }

        private async Task AddActivityWizad()
        {
            CollectorController collector = null!;
            CollectorController collector2 = null!;
            string[] result = new string[4];
            EmbedBuilder eb = new()
            {
                Title = "Qual será a escrita?",
                Description = "Variáveis disponíveis: " +
                "\n``{discordversion}`` - Versão da biblioteca Discord.net." +
                "\n``{discordapi}`` - Versão da API em uso." +
                "\n``{uptime}`` - Tempo de operação do bot." +
                "\n``{version}`` - Versão de compilação do executável." +
                "\n``{guildscount}`` - Quantidade de servidor em que o BOT está.",
                Color = Constants.DiscordBlurple
            };
            eb.WithFooter("Escreva exit para cancelar ou aguarde 5 minutos.");
            await Context.Channel.TriggerTypingAsync();
            IUserMessage? msg = await ReplyAsync(embed: eb.Build(), reply: true);
            collector = _collectorsUtils.CollectMessage(Context.Channel, Predicate => Predicate.Author.Id == Context.User.Id, async eventArgs =>
            {
                try
                {
                check:
                    if (eventArgs.Message.Content == "exit")
                    {
                        eventArgs.StopCollect();
                        await eventArgs.Message.AddReactionAsync(new Emoji("👌"));
                    }
                    else if (string.IsNullOrEmpty(result[0]))
                    {
                        await Context.Channel.TriggerTypingAsync();
                        result[0] = eventArgs.Message.Content;
                        eb.WithTitle("Link da stream? (suportado twitch.tv)");
                        eb.WithDescription("Escreva ``null`` se não deseja usar!\n " +
                            "Se especificar um URL válido, a atividade será do tipo ``Streaming``.");
                        await msg.ModifyAsync(x => x.Embed = eb.Build());
                    }
                    else if (string.IsNullOrEmpty(result[1]))
                    {
                        await Context.Channel.TriggerTypingAsync();
                        result[1] = eventArgs.Message.Content.ToLower() == "null" || !All.IsValidUrl(eventArgs.Message.Content) ? "null" : eventArgs.Message.Content;
                        eb.WithTitle("Tempo em de exibição do status?");
                        eb.WithDescription("Valor em milissegundos, minimo 10000, padrão é 20000.");
                        await msg.ModifyAsync(x => x.Embed = eb.Build());
                    }
                    else if (string.IsNullOrEmpty(result[2]))
                    {
                        if (int.TryParse(eventArgs.Message.Content, out _))
                        {
                            result[2] = eventArgs.Message.Content;
                            if (result[1] == "null")
                            {
                                eb.WithTitle("Tipo da atividade:");
                                eb.WithDescription("" +
                                    "0️⃣ - Jogando" +
                                    "\n1️⃣ - Transmitindo" +
                                    "\n2️⃣ - Ouvindo" +
                                    "\n3️⃣ - Assistindo");
                                await msg.ModifyAsync(x => x.Embed = eb.Build());
                                collector2 = _collectorsUtils.CollectReaction(msg, reaction => reaction.UserId == Context.User.Id, eventArgs5 =>
                                {
                                    try
                                    {
                                        result[3] = eventArgs5.Reaction.Emote.Name;
                                        collector.Dispose();
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                });
                                collector2.SetTimeout(Constants.ShortTimeSpan);
                                try
                                {
                                    await msg.AddReactionsAsync(new IEmote[] { new Emoji("0️⃣"), new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣") });
                                }
                                catch
                                {
                                }
                                await collector2.WaitForEventOrDispose();
                                await Task.Delay(500);
                                result[3] ??= "";
                                goto check;
                            }
                            else
                            {
                                result[3] = "1️⃣";
                                goto check;
                            }
                        }
                        else
                            await ReplyAsync("Número inválido, tente novamente!");
                    }
                    else
                    {
                        collector.Dispose();
                        msg.SafeDelete();
                        await AddActivity(result[0], result[1] == "null" ? null : result[1], result[3].Equals("1️⃣") ? ActivityType.Streaming : result[3].Equals("2️⃣") ? ActivityType.Listening : result[3].Equals("3️⃣") ? ActivityType.Watching : ActivityType.Playing, int.Parse(result[2]));
                    }
                }
                catch
                {
                    // ignored
                }

            }, CollectorFilter.IgnoreSelf);
            collector.SetTimeout(Constants.LongTimeSpan);
            collector.Stop += (sender, s) =>
            {
                collector2?.Dispose();
            };
        }

        private async Task AddActivity(string gamename, [Optional] string? streamUrl, [Optional] ActivityType activityType, int awaits = 20000)
        {
            await Context.Channel.TriggerTypingAsync();
            if (awaits < 1000)
                awaits = 1000;
            BOTStatusModel stt = new()
            {
                Status = gamename,
                StreamUrl = streamUrl,
                Activity = activityType,
                Await = awaits
            };
            int id = Instancia._status.Last().Key + 1;
            if (Instancia._status.TryAdd(id, stt))
            {
                await SystemParamsManager.SaveBotStatus();
                EmbedBuilder embed = new()
                {
                    Title = "Atividade criada",
                    Color = Constants.DiscordBlurple,
                    Description = $"**ID:** {Instancia._status.Last().Key}"
                };
                embed.AddField("Activity Type", stt.Activity.ToString(), true)
                    .AddField("Await", $"{stt.Await}ms", true)
                    .AddField("StreamUrl", stt.StreamUrl ?? "Null", true)
                    .AddField("String Fomated", stt.Format(_client))
                    .WithFooter($"Total: {Instancia._status.Count - 1}");

                var msg = await ReplyAsync(embed: embed.Build(), reply: true);
                CollectorController collector = null!;
                collector = _collectorsUtils.CollectReaction(msg, reaction => reaction.UserId == Context.User.Id && reaction.Emote.Equals(new Emoji("🗑")), async eventArgs =>
                {

                    eventArgs.StopCollect();
                    try
                    {
                        var t = await _handler.ExecuteCommand($"removeactivity {id}", Context, eventArgs.Reaction.UserId.ToString(), _service);
                        if (t.IsSuccess)
                        {
                            msg.SafeDelete();
                            collector?.Dispose();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }, CollectorFilter.IgnoreSelf);
                collector?.SetTimeout(Constants.StandardTimeSpan);
                try
                {
                    await msg.AddReactionAsync(new Emoji("🗑"));
                }
                catch
                {
                }
            }
            else
                await Context.Message.AddReactionAsync(new Emoji("⚠"));
        }

        private async Task RemoveActivity(int id)
        {
            if (id <= 0)
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), "id inválido.", color: Color.DarkOrange, reply: true);
            else
            {
                if (Instancia._status.TryRemove(id, out BOTStatusModel? removed))
                {
                    await SystemParamsManager.SaveBotStatus();
                    EmbedBuilder embed = new()
                    {
                        Title = "Atividade Removida",
                        Color = Color.DarkOrange,
                        Description = $"**ID:** {id}"
                    };
                    embed.AddField("Activity Type", removed.Activity.ToString(), true)
                        .AddField("Await", $"{removed.Await}ms", true)
                        .AddField("StreamUrl", removed.StreamUrl ?? "Null", true)
                        .AddField("String", removed.Status)
                        .WithFooter($"Total: {Instancia._status.Count - 1}");
                    await ReplyAsync(embed: embed.Build(), reply: true);
                }
                else
                    await ReplyFormattedAsync(Loc.Get("Commands.Fail"), "Erro ao remover o item, verifique o id", color: Color.DarkOrange, reply: true);
            }
        }
    }
}
