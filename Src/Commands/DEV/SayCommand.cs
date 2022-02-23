/*
 * Arquivo: SayCommand.cs
 * Criado em: 4-10-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.DEV
{
    [Grouping("devgroup")]
    [RequireOwner(ErrorMessage = "RequireOwner")]
    //[IsDisable]
    [RequireContext(ContextType.Guild, ErrorMessage = "NotAGuildErrorMessage")]
    [RequireBotPermission(ChannelPermission.SendMessages | ChannelPermission.ManageMessages, ErrorMessage = "RequiredBotSendManageMessages")]
    //[RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "RequiredUserManageMessages")] //Sou dono, posso
    public class SayCommand : AdvancedModuleBase
    {

        [Command("say")]
        [Alias("diga", "escreva")]
        [Summary("Say0")]
        public async Task CMD([Summary("Say0_0s")][Remainder] string message) => await Run(message: message);

        [Command("say")]
        [Alias("diga", "escreva")]
        [Summary("Say0")]
        public async Task CMD([Summary("Say0_1s")] ITextChannel chat, [Summary("Say0_0s")][Remainder] string message) => await Run(chat, message);


        private async Task Run([Optional] ITextChannel chat, [Optional] string message)
        {
            bool editmode = false;
            if (chat != null && !Context.Guild.CurrentUser.PermSendMessage(chat))
                await ReplyAsync(Loc.Get("Commands.NoPermWriteChannel", Context.User.Mention, chat.Mention), reply: true);
            else
            {
                await Context.Channel.TriggerTypingAsync();


                string[] args = message.Split(' ');
                string[] msg;

                if (args[0] == "edit" || args[0] == "editar")
                {
                    editmode = true;
                    message = message.Remove(0, args[0].Length);
                }
                if (editmode)
                {
                    if (args.Length > 2 && All.IsValidUrl(args[1]))
                    {
                        msg = args[1].Split('/');
                        message = message.Remove(0, args[1].Length + 1);

                        ulong msgid = ulong.Parse(msg[^1]);
                        SocketTextChannel channel = Context.Guild.GetTextChannel(ulong.Parse(msg[^2]));
                        if (channel is null)
                            await ReplyAsync(Loc.Get("Commands.ChannelNotFound", Context.User.Mention));
                        else
                        {
                            IUserMessage msgedit = (IUserMessage)await channel.GetMessageAsync(msgid);
                            if (msgedit is null)
                                await ReplyAsync(Loc.Get("Commands.MessageNotFound", Context.User.Mention));
                            else if (msgedit.Author != Context.Guild.CurrentUser)
                                await ReplyAsync(Loc.Get("Commands.NoEditOtherMessage", Context.User.Mention));
                            else if (message != null && message.Length > 0)
                            {
                                await msgedit.ModifyAsync(async x => x.Content = await CheckMsg(message));

                                //Mesmo canal, só edita
                                if (Context.Channel.Id != msgedit.Channel.Id)
                                    await ReplyAsync(Loc.Get("Commands.MessageEditedSuccess", Context.User.Mention), reply: true);
                                else
                                    Context.Message.SafeDelete(); //deleta a mensagem de quem mandou (as vezes da erro e ele exclui o emoji de erro)
                                                                  //await ReplyAsync("msg: "+ msg[msg.Count()-1] +" channel: "+ msg[msg.Count() - 2]);
                            }
                            else
                                await ReplyAsync(Loc.Get("Commands.MessageEditNotFound", Context.User.Mention), reply: true);
                        }
                    }
                    else
                        await ReplyAsync(Loc.Get("Commands.UriInvalid", Context.User.Mention), reply: true);
                }

                else if (message != null && message.Length != 0)
                {

                    if (chat != null && Context.Channel.Id != chat.Id)
                    {
                        await chat.SendMessageAsync(await CheckMsg(message));
                        await ReplyAsync(Loc.Get("Commands.MessageSendToChat", Context.User.Mention, chat.Mention), reply: true); ;
                    }
                    else
                    {
                        await ReplyAsync(await CheckMsg(message));
                        Context.Message.SafeDelete(); //deleta a mensagem de quem mandou (as vezes da erro e ele exclui o emoji de erro)
                    }
                }
                else
                    //throw new ArgumentOutOfRangeException();
                    await ReplyAsync(Loc.Get("Commands.SayMsgCount0", GuildConfig.Prefix, Context.User.Mention), reply: true);

            }
        }

        private async Task<string> CheckMsg(string msg)
        {
            if (!await Context.IsBotOwnerAsync() /*&& !Context.Guild.GetUser(Context.User.Id).IsGameManager(Context.Guild.GetConfig()) */|| Regex.IsMatch(msg, @"(^\d{3}\.\d{3}\.\d{3}\-\d{2}$)|(^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$)") || Regex.IsMatch(msg, @"^\d{5}-\d{3}$"))
                msg += Loc.Get("Commands.SayMsgBy", Context.User.Mention);
            return msg;
        }
    }
}
