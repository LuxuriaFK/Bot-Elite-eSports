/*
 * Arquivo: HelpCommand.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 4-10-2021
 */
using BOTDiscord.DiscordRelated.Commands;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands;
using BOTDiscord.Manager.Commands.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands
{
    public class HelpCommand : AdvancedModuleBase
    {
        private readonly CommandHandler _handler;
        private readonly DiscordSocketClient _client;

        public HelpCommand(DiscordSocketClient client, CommandHandler commandhandler)
        {
            _client = client;
            _handler = commandhandler;
        }

        [Command("help")]
        [Alias("ajuda", "commands")]
        [Summary("help0s")]
        public async Task PrintHelp([Remainder][Summary("help0_0s")] string message)
        {
            message = message.ToLower();
            var eb = this.GetAuthorEmbedBuilder().WithColor(Context.Guild.CurrentUser.GetColor());
            if ((message != "devgroup" || message == "devgroup" && await Context.IsBotOwnerAsync()) && _handler.CommandsGroups.Value.TryGetValue(message, out var commandGroup))
            {
                eb.WithTitle(Loc.Get("Help.CommandsOfGroup").Format(message.ToLower()))
                  .WithFields(commandGroup.Commands.GroupBy(info => info.Summary).Select(infos => infos.First()).Select(info => new EmbedFieldBuilder
                  {
                      Name = $"{GuildConfig.Prefix}{info.Name} {_handler.GetAliasesString(info.Aliases, Loc, info.Name)}",
                      Value = Loc.Get($"Help.{info.Summary}")
                  }));
            }
            else if (_handler.CommandAliases.Contains(message))
            {
                eb.WithTitle(Loc.Get("Help.ByCommand").Format(message))
                  .WithFields(_handler.BuildHelpFields(message, GuildConfig.Prefix, Loc));
            }
            else
            {
                eb.WithTitle(Loc.Get("Help.NotFoundTitle"))
                  .WithDescription(Loc.Get("Help.NotFoundDescription").Format(message.SafeSubstring(100, "..."), GuildConfig.Prefix)).WithColor(Color.LightOrange);
            }

            await ReplyAsync(embed: eb.Build(), reply: true);//).DelayedDelete(Constants.LongTimeSpan);
        }

        [Command("help")]
        [Alias("ajuda", "commands")]
        [Summary("help1s")]
        public async Task PrintHelp()
        {
            try
            {
                var eb = this.GetAuthorEmbedBuilder()
                             .WithTitle(Loc.Get("Help.HelpTitle"))
                             .WithColor(Context.Guild.CurrentUser.GetColor())
                             .WithDescription(Loc.Get("Help.HelpPrefix").Format(GuildConfig.Prefix, _client.CurrentUser.Mention))
                             .AddField($"{GuildConfig.Prefix}help", Loc.Get("Help.HelpDescription"))
                             //.WithFields(HelpUtils.CommandsGroups.Value.Where(info => !(!Context.Guild.GetConfigAsync().EliteServer && (info.Key == "eliterank" || info.Key == "elitegm" || info.Key == "eliteutils"))).Where(info => !(!Context.User.IsBotOwner() && info.Key == "devgroup")).Select(pair =>
                             .WithFields(_handler.CommandsGroups.Value.Where(info => !(!Context.IsBotOwnerAsync().Result && info.Key == "devgroup")).Select(pair =>
                                    new EmbedFieldBuilder
                                    {
                                        Name = pair.Value.GroupNameTemplate.Format(Loc.Get($"Groups.{pair.Key}"), GuildConfig.Prefix),
                                        Value = pair.Value.GroupTextTemplate.Format(GuildConfig.Prefix)
                                    }));
                //eb.AddField(Loc.Get("Common.Vote"), Loc.Get("Common.VoteDescription"));
                await ReplyAsync(embed: eb.Build(), reply: true);//).DelayedDelete(Constants.LongTimeSpan);
            }
            catch (Exception e)
            {
                await ReplyAsync("[Erro]: " + e.Message);
            }
        }
    }
}
