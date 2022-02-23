/*
 * Arquivo: KickCommand.cs
 * Criado em: 27-9-2021
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
    [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "RequiredBotKickMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "RequiredUserKickMembers", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    public class KickCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;

        public KickCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("kick")]
        [Alias("expulsar")]
        [Summary("Kick0")]
        public async Task KickUser([Summary("Kick0_0s")] SocketGuildUser User, [Summary("Kick0_1s")][Remainder][Optional] string Reason) => await Run(User, Reason);


        [Command("kick")]
        [Alias("expulsar")]
        [Hidden]
        public async Task KickUser([Summary("Kick0_0s")] IUser User, [Summary("Kick0_1s")][Remainder][Optional] string Reason) => await Run(User, Reason);

        [Command("kick")]
        [Alias("expulsar")]
        [Summary("Kick0")]
        public async Task KickUser([Summary("Kick0_3s")] ulong UserID, [Summary("Kick0_1s")][Remainder][Optional] string Reason) => await Run(UserID, Reason);


        private async Task Run(object UserK, string reason)
        {
            SocketGuildUser User = UserK is ulong @int ? Context.Guild.GetUser(@int) : UserK is IUser @userk ? Context.Guild.GetUser(@userk.Id) : (SocketGuildUser)UserK;

            await Context.Channel.TriggerTypingAsync();
            if (User != null)
            {
                if (User.Id == Context.User.Id)
                    await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Kick.YourSelf"), color: Color.DarkOrange, reply: true, footer: false);
                else if (User.Id == _client.CurrentUser.Id)
                    await ReplyFormattedAsync(description: Loc.Get("Kick.KickBot", Context.User.Mention), color: Color.DarkOrange, reply: true, footer: false);
                else
                {
                    SocketGuildUser Bot = Context.Guild.CurrentUser;
                    if (User.GuildPermissions.Administrator && !Bot.GuildPermissions.Administrator)
                        await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Commands.UserIsAdmin"), color: Color.DarkOrange, reply: true, footer: false);
                    else
                    {
                        if (Bot.Hierarchy <= User.Hierarchy)
                            await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Kick.HighHieratchy"), color: Color.DarkOrange, reply: true, footer: false);

                        else if (Context.Guild.GetUser(Context.User.Id).Hierarchy <= User.Hierarchy)
                            await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Kick.HighHieratchyForYou"), color: Color.DarkOrange, reply: true, footer: false);

                        else
                        {
                            await User.KickAsync(reason.Truncate(512));
                            await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("Kick.Success", User.ToString(), reason.IsEmpty(Loc.Get("Kick.DefaultMotive")).Truncate(512)!), Color.Orange, footer: false);
                        }
                    }
                }
            }
            else
            {
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Kick.UserNotFound"), color: Color.DarkOrange, reply: true, footer: false);
            }
        }
    }
}
