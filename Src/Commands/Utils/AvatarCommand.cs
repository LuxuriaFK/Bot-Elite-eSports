/*
 * Arquivo: AvatarCommand.cs
 * Criado em: 27-9-2021
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
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Utils
{
    [Grouping("utils")]
    //[IsDisable]
    public class AvatarCommand : AdvancedModuleBase
    {
        [Command("avatar", true)]
        [Hidden]
        [Alias("foto")]
        //Evitar erro do parse procurar usuario com a string
        public async Task CMD([Optional] string _) => await Run(0);

        [Command("avatar")]
        [Alias("foto")]
        [Summary("Avatar0")]
        public async Task CMD([Summary("Avatar0_1s")][Optional] ulong userid) => await Run(userid);

        [Command("avatar")]
        [Alias("foto")]
        [Summary("Avatar0")]
        public async Task CMD([Summary("Avatar0_0s")] IUser user) => await Run(user.Id);

        private async Task Run([Optional] ulong userid)
        {
            SocketUser user = userid > 0 ? Context.Guild.GetUser(userid) ?? Context.User : Context.User;
            EmbedBuilder embed = new EmbedBuilder
            {
                ImageUrl = user.GetAvatarUrl(ImageFormat.Auto, 1024) ?? user.GetDefaultAvatarUrl(),
                Color = Context.Guild is null ? Constants.DiscordBlurple : Context.Guild.GetUser(user.Id).GetColor() //cor padrao discord #7289DA Blurple

            }.WithAuthor(user).WithFooter(DiscordUtils.GetFooterEmbedBuilder(Context.User, Loc));
            
            await ReplyAsync(embed: embed.Build(), reply: true);

        }

    }
}