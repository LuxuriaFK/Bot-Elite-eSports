/*
 * Arquivo: UnBanCommand.cs
 * Criado em: 19-10-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Moderation
{
    [Grouping("moderation")]
    //[IsDisable]
    [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "RequiredBotBanMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "RequiredUserBanMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    public class UnBanCommand : AdvancedModuleBase
    {
        [Command("unban")]
        [Alias("desbanir")]
        [Summary("UnBan0")]
        public async Task CMD([Summary("UnBan0_0s")] SocketGuildUser User) => await Run(User);

        [Command("unban")]
        [Alias("desbanir")]
        [Summary("UnBan0")]
        [Hidden]
        public async Task CMD([Summary("UnBan0_0s")] string User) => await Run(User);

        [Command("unban")]
        [Alias("desbanir")]
        [Summary("UnBan0")]
        public async Task CMD([Summary("UnBan0_1s")] ulong UserID) => await Run(UserID);


        private async Task Run(object user)
        {
            ulong userid = 0;
            if (user is string @User)
                userid = @User.GetUserID();
            else if (user is ulong @long)
                userid = @long;
            else
                userid = ((SocketGuildUser)user).Id;

            var allBans = await Context.Guild.GetBansAsync();
            var bancheck = allBans.Where(u => u.User.Id == userid).FirstOrDefault();
            if (bancheck == null)
            {
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("UnBan.NoBanned", userid), Color.DarkOrange);
                Context.Message.SafeDelete();
            }
            else
            {
                await Context.Guild.RemoveBanAsync(userid);
                await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("UnBan.Success", userid), Color.DarkBlue, true);
                Context.Message.SafeDelete();

            }
        }
    }
}
