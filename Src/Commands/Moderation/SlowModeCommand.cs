/*
 * Arquivo: SlowModeCommand.cs
 * Criado em: 8-10-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Utilities;
using Discord;
using Discord.Commands;
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
    [RequireContext(ContextType.Guild, ErrorMessage = "NotAGuildErrorMessage")]
    [RequireBotPermission(ChannelPermission.ManageChannels, ErrorMessage = "RequiredBotManageChannels")]
    [RequireUserPermission(ChannelPermission.ManageChannels, ErrorMessage = "RequiredUserManageChannels")]
    public class SlowModeCommand : AdvancedModuleBase
    {
        [Command("slowmode")]
        [Alias("modolento")]
        [Summary("SlowModeChat0")]
        public async Task SlowMode([Summary("SlowModeChat0_0s")][Optional] int seconds) => await Run(seconds);

        private async Task Run(int temp)
        {
            IUser user = Context.User;
            if (temp < 0)
                temp = 0;
            ITextChannel _chat = Context.Guild.GetTextChannel(Context.Channel.Id);
            if (_chat.SlowModeInterval != temp)
            {
                await _chat.ModifyAsync(x => x.SlowModeInterval = temp);
                Context.Message.SafeDelete();
                (await ReplyAsync(temp == 0 ?
                    Loc.Get("SlowMode.Disable", user.Mention, temp) :
                   Loc.Get("SlowMode.Active", user.Mention, temp))).DelayedDelete(Constants.ShortTimeSpan);
            }
            else
                (await ReplyAsync(Loc.Get(temp == 0 ? "SlowMode.None" : "SlowMode.NoChanges", user.Mention, temp))).DelayedDelete(Constants.ShortTimeSpan);
        }
    }
}
