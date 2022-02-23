/*
 * Arquivo: SetPrefixCommand.cs
 * Criado em: 27-9-2021
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
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Settings
{
    [Grouping("botconfig")]
    //[IsDisable]
    [RequireUserPermission(GuildPermission.ManageGuild, ErrorMessage = "RequiredUserManageGuild", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    public class SetPrefixCommand : AdvancedModuleBase
    {
        [Command("setprefix")]
        [Summary("setprefix0s")]
        public async Task SetPrefix([Summary("setprefix0_0s")] char prefix)
        {
            await Context.Channel.TriggerTypingAsync();
            if (GuildConfig.Prefix == prefix)
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Prefix.PrefixIsActual", prefix), Constants.ShortTimeSpan, Color.DarkOrange);
            else
            {
                await GuildConfig.SetPrefixAsync(prefix);
                await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("Prefix.SetPrefixResponse", prefix), Constants.ShortTimeSpan, Color.Gold);
            }
            Context.Message.SafeDelete();
        }

    }
}
