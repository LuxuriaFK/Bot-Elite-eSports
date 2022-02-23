/*
 * Arquivo: ClearCommand.cs
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Moderation
{
    [Grouping("moderation")]
    //[IsDisable]
    [RequireContext(ContextType.Guild, ErrorMessage = "NotAGuildErrorMessage")]
    [RequireBotPermission(ChannelPermission.ManageMessages, ErrorMessage = "RequiredBotManageMessages")]
    [RequireUserPermission(ChannelPermission.ManageMessages, ErrorMessage = "RequiredUserManageMessages")]
    public class ClearCommand : AdvancedModuleBase
    {

        [Command("clear")]
        [Alias("apagar", "limpar", "purgue")]
        [Summary("PurgueChat0")]
        public async Task LimparCommand([Summary("PurgueChat0_0s")][Optional] int Count, [Summary("PurgueChat0_1s")][Optional] IUser Author)
        {
            //Indica que o bot está escrevendo
            await Context.Channel.TriggerTypingAsync();

            if (Count == 0)
            {
                await ReplyAsync(Loc.Get("Purgue.PurgueCount0", GuildConfig.Prefix), reply: true);
                return;
            }

            //Pra nao bugar td e dar disconnect
            if (Count > 1000)
                Count = 1000;

            //Apaga a mensagem do autor
            Count++;

            var messages = (await Context.Channel.GetMessagesAsync(Count).FlattenAsync()).Where(x => !x.IsPinned).ToList();
            IEnumerable<IMessage> msgdel;
            if (Author != null)
                msgdel = messages.Where(x => x.Author.Id == Author.Id).Where(x => x.Timestamp > DateTimeOffset.UtcNow.AddDays(-14));
            else
                msgdel = messages.Where(x => x.Timestamp > DateTimeOffset.UtcNow.AddDays(-14));

            if (msgdel.Any())
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(msgdel);


            int msgCount = msgdel.Count() - 1; //tira a do autor
            string result = Loc.Get("Purgue.Success", msgCount);

            if (msgdel.Count() < Count || Author is not null)
                if (Author is not null)
                    result = Loc.Get("Purgue.SuccessAuthor", msgCount, Author.Username);
                else
                    result = Loc.Get("Purgue.SuccessOld", Context.User.Mention, Count - 1 - msgCount);


            var resp = await ReplyAsync(result);
            resp.DelayedDelete(Constants.VeryVeryShortTimeSpan);

            if (Author is not null)
                Context.Message.SafeDelete();
        }

    }
}
