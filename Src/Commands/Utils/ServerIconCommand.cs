/*
 * Arquivo: ServerIconCommand.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.DiscordRelated;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Utilities;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Utils
{
    [Grouping("utils")]
    //[IsDisable]
    [RequireContext(ContextType.Guild, ErrorMessage = "NotAGuildErrorMessage")]
    public class ServerIconCommand : AdvancedModuleBase
    {
        [Command("servericon")]
        [Summary("ServIcon0")]
        public async Task CMD() => await Run();

        private async Task Run()
        {
            if (!Context.Guild.IconUrl.IsEmpty())
            {
                EmbedBuilder embed = new EmbedBuilder
                {
                    ImageUrl = Context.Guild.IconUrl + "?size=2048",
                    Color = Constants.DiscordBlurple //cor padrao discord #7289DA Blurple

                }.WithAuthor(Context.Guild.Name).WithFooter(DiscordUtils.GetFooterEmbedBuilder(Context.User, Loc));

                await ReplyAsync(embed: embed.Build(), reply: true);
            }
            else
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("ServerIcon.None"), Color.DarkOrange, reply: true);

        }
    }
}
