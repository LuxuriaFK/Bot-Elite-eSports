/*
 * Arquivo: BanCommand.cs
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Moderation
{
    [Grouping("moderation")]
    //[IsDisable]
    [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "RequiredBotBanMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "RequiredUserBanMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    public class BanCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;

        public BanCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        [Hidden]
        public async Task CMD([Summary("Ban0_0s")] SocketGuildUser User, [Summary("Ban0_2s")][Remainder][Optional] string Reason) => await Run(User, reason: Reason);
        
        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        public async Task CMD([Summary("Ban0_0s")] SocketGuildUser User, [Summary("Ban0_1s")][Optional] int PruneDays, [Summary("Ban0_2s")][Remainder] string Reason = null) => await Run(User, PruneDays, Reason);

        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        [Hidden]
        public async Task CMD([Summary("Ban0_3s")] ulong UserID, [Summary("Ban0_2s")][Remainder][Optional] string Reason) => await Run(UserID, reason: Reason);

        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        public async Task CMD([Summary("Ban0_3s")] ulong UserID, [Summary("Ban0_1s")][Optional] int PruneDays, [Summary("Ban0_2s")][Remainder][Optional] string Reason) => await Run(UserID, PruneDays, Reason);

        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        [Hidden]
        public async Task CMD([Summary("Ban0_0s")] string User, [Summary("Ban0_2s")][Remainder][Optional] string Reason) => await Run(User, reason: Reason);

        [Command("ban")]
        [Alias("banir")]
        [Summary("Ban0")]
        [Hidden]
        public async Task CMD([Summary("Ban0_0s")] string User, [Summary("Ban0_1s")][Optional] int PruneDays, [Summary("Ban0_2s")][Remainder] string Reason = null) => await Run(User, PruneDays, Reason);


        private async Task Run(object userban, [Optional] int prune, [Optional] string reason)
        {

            SocketGuildUser UserB;
            if (userban is SocketGuildUser @SUser)
                UserB = SUser;
            else if (userban is string @User)
                UserB = Context.Guild.GetUser((ulong)(userban = @User.GetUserID()));
            else
                UserB = Context.Guild.GetUser((ulong)userban);

            await Context.Channel.TriggerTypingAsync();

            var allBans = await Context.Guild.GetBansAsync();
            var bancheck = allBans.Where(u => u.User.Id == (UserB is null ? (ulong)userban : UserB.Id)).FirstOrDefault();
            if (bancheck != null)
            {
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Ban.AlreadyBanned", bancheck.User.Username, bancheck.User.Discriminator, bancheck.Reason.IsEmpty(Loc.Get("Ban.DefaultMotive"))), Color.Orange);
                Context.Message.SafeDelete();
            }
            else
            {
                if (UserB != null)
                {
                    if (UserB.Id == Context.User.Id)
                        await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Ban.YourSelf"), color: Color.Orange, reply: true);
                    else if (UserB.Id == _client.CurrentUser.Id)
                        await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Ban.BanBot", Context.User.Mention), color: Color.LightOrange, reply: true);
                    else
                    {
                        SocketGuildUser Bot = Context.Guild.CurrentUser;
                        if (UserB.GuildPermissions.Administrator && !Bot.GuildPermissions.Administrator)
                            await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Commands.UserIsAdmin"), color: Color.LightOrange, reply: true);
                        else
                        {
                            if (Bot.Hierarchy < UserB.Hierarchy)
                                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Ban.HighHieratchy"), color: Color.LightOrange, reply: true);

                            else if (Context.Guild.GetUser(Context.User.Id).Hierarchy < UserB.Hierarchy)
                                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Ban.HighHieratchyForYou"), color: Color.LightOrange, reply: true);

                            else
                            {
                                //(reason = reason.IsEmpty(Loc.Get("Ban.DefaultMotive"))).Truncate(512);

                                await UserB.BanAsync(prune, reason.Truncate(512));
                                Context.Message.SafeDelete();
                                await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("Ban.Success", UserB.Username, UserB.Discriminator, reason.IsEmpty(Loc.Get("Ban.DefaultMotive")).Truncate(512)!), Color.DarkRed, true);
                            }
                        }
                    }
                }
                else
                {
                    //(reason = reason.IsEmpty(Loc.Get("Ban.DefaultMotive"))).Truncate(512);
                    await Context.Guild.AddBanAsync((ulong)userban, prune, reason.Truncate(512));
                    Context.Message.SafeDelete();
                    await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("Ban.SuccessID", (ulong)userban, reason.IsEmpty(Loc.Get("Ban.DefaultMotive"))).Truncate(512)!, Color.DarkRed, true);
                }
            }
        }
    }
}