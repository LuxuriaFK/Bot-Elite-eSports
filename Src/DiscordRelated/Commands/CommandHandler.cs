/*
 * Arquivo: CommandHandler.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.DiscordRelated;
using BOTDiscord.DiscordRelated.Commands;
using BOTDiscord.Extensions;
using BOTDiscord.Librarys;
using BOTDiscord.Localization.Entries;
using BOTDiscord.Localization.Providers;
using BOTDiscord.Logger;
using BOTDiscord.Managers;
using BOTDiscord.Models;
using BOTDiscord.Utilities;
using BOTDiscord.Utilities.Collector;
using Discord;
using Discord.Commands;
using Discord.Webhook;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Manager.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CollectorsUtils _collectorsUtils;
        public List<CommandInfo> AllCommands { get; } = new();
        public readonly CommandService CommandService;
        public Lookup<string, CommandInfo> CommandAliases = null!;
        private static readonly FuzzySearch FuzzySearch = new();

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services, CollectorsUtils collectorsUtils)
        {
            _client = client;
            CommandService = commandService;
            _services = services;
            _collectorsUtils = collectorsUtils;

            commandService.CommandExecuted += CommandExecutedAsync;
            client.MessageReceived += MessageReceivedAsync;
        }

        internal async Task InitializeAsync()
        {

            Log.Discord("Adicionando módulos...", true);
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            foreach (var cmdsModule in CommandService.Modules)
            {
                foreach (var command in cmdsModule.Commands)
                    AllCommands.Add(command);
            }

            var items = new List<KeyValuePair<string, CommandInfo>>();
            foreach (var command in AllCommands)
                items.AddRange(command.Aliases.Select(alias => new KeyValuePair<string, CommandInfo>(alias, command)));

            CommandAliases = (Lookup<string, CommandInfo>)items.ToLookup(pair => pair.Key, pair => pair.Value);

            items.Clear();
            Log.Discord("Adicionando comandos no fuzzy search...", true);
            foreach (var alias in AllCommands.Where(info => !info.IsDEVCommand()).Where(info => !info.IsHiddenCommand())
                .SelectMany(commandInfo => commandInfo.Aliases).GroupBy(s => s).Select(grouping => grouping.First()))
            {
                FuzzySearch.AddData(alias);
            }
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // garante que não processamos mensagens de sistema / outras mensagens
            if (rawMessage is not SocketUserMessage message)
                return;

            if (message.Source != MessageSource.User)
                return;

            SocketCommandContext context = new(_client, message);

            //verifica mensão de todos 
            //Precisa permissão para excluir a msg q ter o treco

            //Colocar pra desativar e ativar em qual server qer
            ChannelPermissions bot = context.Guild.CurrentUser.GetPermissions((IGuildChannel)context.Channel);
            if (bot.ManageMessages)
                if (!await CheckMention(context))
                    return;


            //User u = UserManager.Get(context.User.Id);

            //Verifica se o user ta banido de usar o bot
            //if (u.Banned)
            //  return;

            //Ignora se nao pode enviar mensagem
            if (context.Guild != null)
            {
                //ChannelPermissions bot = context.Guild.CurrentUser.GetPermissions((IGuildChannel)context.Channel);
                if (!bot.SendMessages)
                    return;
            }

            GuildConfig guildconfig = await context.Guild.GetConfigAsync();
            char prefix = guildconfig.Prefix;
            // define a posição do argumento longe do prefixo que definimos
            var argPos = 0;

#if DEBUG //Apenar quartel
            if (context.Guild?.Id != 641053702022496266)
                return;
#endif

            // determine se a mensagem tem um prefixo válido e ajuste argPos com base no prefixo
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                if (_client.CurrentUser != null && message.HasStringPrefix(_client.CurrentUser.Mention, ref argPos))
                    //Indica que o bot está escrevendo
                    using (context.Channel.EnterTypingState())
                    {
                        await context.Message.AddReactionAsync(new Emoji("🥰"));
                        await context.Channel.SendMessageAsync(guildconfig.Loc.Get("Commands.HelloMention", prefix), messageReference: new MessageReference(context.Message.Id));
                    }
                return;
            }

            UserConfig userConfig = await context.User.GetConfigAsync();
            if (userConfig.NextCommand > DateTime.Now)
            {
                if (context.Message.Content != guildconfig.Prefix.ToString())
                    await context.Channel.SendMessageAsync(guildconfig.Loc.Get("Commands.WaitTime", context.User.Mention), messageReference: new MessageReference(context.Message.Id));
                return;
            }


            // execute o comando
            await CommandService.ExecuteAsync(context, argPos, _services);

        }

        private async Task<bool> CheckMention(SocketCommandContext context)
        {
            //Ve se ta marcado geral ou ve se tem a escrita de marcação (usuario sem perm de marcar) e não é o dono do bot ou o dono do grupo
            if ((context.Message.MentionedEveryone || context.Message.Content.ContainsEveryone()) && !await context.IsBotOwnerAsync() && !context.IsGuildOwner())
            {
                context.Message.SafeDelete();
                (await context.Channel.SendMessageAsync(context.User.Mention + " <:cade:675142040366874654>")).DelayedDelete(Constants.VeryVeryShortTimeSpan);
                return false;
            }
            return true;
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            var guildConfig = await GuildManager.GetAsync(context.Guild);
            var loc = guildConfig.Loc;

            // se um comando não for encontrado, registre essa informação no console e saia deste método
            if (!command.IsSpecified)
            {

                var argPos = 0;
                if (context.Message.HasCharPrefix(guildConfig.Prefix, ref argPos) || context.Message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var query = context.Message.Content.Try(s1 => s1[argPos..], "");
                    if (!string.IsNullOrEmpty(query))
                    {
                        //    query = " ";
                        //if (string.IsNullOrWhiteSpace(query))
                        //    query = "help";

                        await AddEmojiErrorHint(context, loc, new Emoji("ℹ"),
                                loc.Get("CommandHandler.UnknownCommand", query.SafeSubstring(40, "...")!,
                                    string.Join(", ", FuzzySearch.Search(query).GetBestMatches(3).Select(match => $"`{match.SimilarTo}`")), guildConfig.Prefix.ToString()));


                        Log.DiscordCMD($"Command failed to execute for [{context.User}] <-> [{context.Message}]!");
                    }
                   
                }
                return;
            }

            // registre o sucesso no console e saia deste método
            if (result.IsSuccess)
            {
                Log.DiscordCMD($"Command [{context.Message}] executed for -> [{context.User}] - guild [{(await context.Guild.GetConfigAsync()).GuildID}]");

                if (!await context.IsBotOwnerAsync())
                    //colocar em atributo os comando q precisam mais tempo de espera e validar aqui
                    (await context.User.GetConfigAsync()).NextCommand = DateTime.Now.AddSeconds(4);

                return;
            }
            else
            {
                switch (result.Error)
                {
                    case CommandError.ParseFailed:
                        await AddEmojiErrorHint(context, loc, new Emoji("⚠"), new EntryLocalized("CommandHandler.ParseFailed"),
                        BuildHelpFields(command.Value.Aliases[0], guildConfig.Prefix, loc));
                        break;
                    case CommandError.BadArgCount:
                        await AddEmojiErrorHint(context, loc, new Emoji("⚠"), new EntryLocalized("CommandHandler.BadArgCount"),
                        BuildHelpFields(command.Value.Aliases[0], guildConfig.Prefix, loc));
                        break;
                    case CommandError.UnmetPrecondition:
                        await AddEmojiErrorHint(context, loc, new Emoji("🚫"), string.IsNullOrWhiteSpace(result.ErrorReason) ? loc.Get("CommandHandler.UnmetPrecondition") : loc.Get("CommandError." + result.ErrorReason));
                        break;
                    default:
                        var a = new EmbedFieldBuilder { Name = result.Error.ToString(), Value = result.ErrorReason };
                        EmbedFieldBuilder[] aa = { a };
                        await AddEmojiErrorHint(context, loc, new Emoji("❗"), new EntryLocalized("CommandHandler.DefaultFailed"), aa);
                        break;
                        //case CommandError.ObjectNotFound:
                        //    await SendErrorMessage(msg, loc, result.ErrorReason);
                        //    break;
                        //case CommandError.MultipleMatches:
                        //    await SendErrorMessage(msg, loc, result.ErrorReason);
                        //    break;
                }
                //Cenário de falha, so o dono saberá
                if (await context.IsBotOwnerAsync())
                    (await context.Channel.SendMessageAsync($"Vixi, algo deu errado -> `{result.Error} >> {result.ErrorReason}`")).DelayedDelete(Constants.ShortTimeSpan);
                Log.DiscordCMD($"[ERROR] Command [{context.Message}] executed by [{context.User}] in guild [{(await context.Guild.GetConfigAsync()).GuildID}] -> Error [{result.Error}] - Reason [{result.ErrorReason}]");
            }
        }

        private async Task AddEmojiErrorHint(ICommandContext targetcommand, ILocalizationProvider loc, IEmote emote, string description)
        {
            CollectorController collector = null!;
            collector = _collectorsUtils.CollectReaction(targetcommand.Message, reaction => reaction.UserId == targetcommand.Message.Author.Id, async eventArgs =>
            {
                collector?.Dispose();
                await SendErrorMessage(targetcommand, loc, description);
            });
            await targetcommand.Message.AddReactionAsync(emote);
        }

        private async Task AddEmojiErrorHint(ICommandContext targetcommand, ILocalizationProvider loc, IEmote emote, IEntry description, [Optional] IEnumerable<EmbedFieldBuilder> builders)
        {
            CollectorController collector = null!;
            collector = _collectorsUtils.CollectReaction(targetcommand.Message, reaction => reaction.UserId == targetcommand.Message.Author.Id, async eventArgs =>
            {
                collector?.Dispose();
                
                await SendErrorMessage(targetcommand, loc, description.Get(loc), builders);
            });
            await targetcommand.Message.AddReactionAsync(emote);
        }

        public async Task<IResult> ExecuteCommand(IMessage message, string query, ICommandContext context, KeyValuePair<CommandMatch, ParseResult> pair, string authorId)
        {
            IResult result = _collectorsUtils.OnCommandExecute(pair, context, message)
                ? await pair.Key.ExecuteAsync(context, pair.Value, EmptyServiceProvider.Instance)
                : ExecuteResult.FromSuccess();

            return result;
        }

        public async Task<IResult> ExecuteCommand(string query, ICommandContext context, string authorId, IServiceProvider service)
        {
            var result = await CommandService.ExecuteAsync(context, query, service);
           
            return result;
        }
        private async Task SendErrorMessage(ICommandContext command, ILocalizationProvider loc, string description, IEnumerable<EmbedFieldBuilder> fieldBuilders)
        {
            if (fieldBuilders == null)
            {
                await SendErrorMessage(command, loc, description);
                return;
            }
            var chat = await GetResponseChannel(command);
            (await chat.SendMessageAsync(embed: GetErrorEmbed(command.Message, loc, description).WithFields(fieldBuilders).Build(), messageReference: chat is IDMChannel ? null : new MessageReference(command.Message.Id, command.Message.Channel?.Id))).DelayedDelete(Constants.LongTimeSpan);
        }

        private async Task SendErrorMessage(ICommandContext command, ILocalizationProvider loc, string description)
        {
            var chat = await GetResponseChannel(command);
            (await chat.SendMessageAsync(embed: GetErrorEmbed(command.Message, loc, description).Build(), messageReference: chat is IDMChannel ? null : new MessageReference(command.Message.Id, command.Message.Channel?.Id))).DelayedDelete(Constants.LongTimeSpan);
        }

        private EmbedBuilder GetErrorEmbed(IUserMessage message, ILocalizationProvider loc, string description)
        {
            var builder = new EmbedBuilder();
            builder.WithFooter(DiscordUtils.GetFooterEmbedBuilder(message.Author, loc))
                   .WithColor(Color.DarkRed);
            builder.WithTitle(loc.Get("CommandHandler.FailedTitle"))
                   .WithDescription(description);
            return builder;
        }

        private async Task<IMessageChannel> GetResponseChannel(ICommandContext command)
        {
            if (command.Guild is null)
                return await command.User.GetOrCreateDMChannelAsync();
            var bot = (await command.Guild.GetCurrentUserAsync()).GetPermissions((IGuildChannel)command.Channel);
            var user = (await command.Guild.GetUserAsync(command.User.Id)).GetPermissions((IGuildChannel)command.Channel);
            return bot.SendMessages && /*bot.AttachFiles &&*/ user.SendMessages // user.AttachFiles
                ? command.Channel
                : await command.User.GetOrCreateDMChannelAsync();
        }


        #region Help area
        public Lazy<Dictionary<string, CommandGroup>> CommandsGroups => new(AllCommands.Where(info => !info.IsHiddenCommand())
                          .GroupBy(info => info.GetGroup()?.GroupName ?? "")
                          .Where(grouping => !string.IsNullOrWhiteSpace(grouping.Key)).Select(infos =>
                               new CommandGroup
                               {
                                   Commands = infos.ToList(),
                                   GroupId = infos.Key,
                                   GroupNameTemplate = $"{{0}} ({{1}}help {infos.Key}):",
                                   GroupTextTemplate = string.Join(" ", infos.Select(info => info.Name)
                                                                             .GroupBy(s => s).Select(grouping => grouping.First())
                                                                             .Select(s => $"``{{0}}{s}``"))
                               }).ToDictionary(group => group.GroupId));

        public IEnumerable<EmbedFieldBuilder> BuildHelpFields(string command, char prefix, ILocalizationProvider loc)
        {
            return CommandAliases[command].Where(x => !x.IsHiddenCommand()).Select(info => new EmbedFieldBuilder
            {
                Name = loc.Get("Help.CommandTitle").Format(command, GetAliasesString(info.Aliases, loc, command)),
                Value = $"{loc.Get($"Help.{info.Summary}")}\n" +
                        "```css\n" +
                        $"{prefix}{info.Name} {(info.Parameters.Count == 0 ? "" : $"[{string.Join("] [", info.Parameters.Select(x => x.Name))}]")}```" +
                        (info.Parameters.Count == 0
                            ? ""
                            : "\n" + string.Join("\n",
                                info.Parameters.Select(x => $"`{x.Name}` - {(string.IsNullOrWhiteSpace(x.Summary) ? "" : loc.Get("Help." + x.Summary))}")))
            });
        }

        public string GetAliasesString(IEnumerable<string> aliases, ILocalizationProvider loc, string cmd, bool skipFirst = false)
        {
            aliases = skipFirst ? aliases.Skip(1) : aliases;
            var enumerable = aliases.ToList();
            if (!string.IsNullOrWhiteSpace(cmd))
                enumerable.Remove(cmd);
            if (!enumerable.Any())
                return "";

            return "(" + loc.Get("Help.Aliases") + GetAliases(enumerable) + ")";
        }

        private string GetAliases(IEnumerable<string> aliases)
        {
            var s = new StringBuilder();
            foreach (var aliase in aliases)
            {
                s.Append($" `{aliase}` ");
            }

            return s.ToString().Trim();
        }
        #endregion
    }
}
