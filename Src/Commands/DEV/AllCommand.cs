/*
 * Arquivo: AllCommand.cs
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
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.DEV
{
    [Grouping("devgroup")]
    [RequireOwner(ErrorMessage = "RequireOwner")]
    public class AllCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;

        public AllCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("leave")]
        [Summary("LeaveBot0")]
        internal async Task Run()
        {
            await Context.Message.AddReactionAsync(new Emoji("☹"));
            await Context.Guild.LeaveAsync();
        }

        [Command("invite")]
        [Alias("convite")]
        [Summary("Invite0")]
        public async Task Invite()
        {
            Context.Message.SafeDelete();
            var inviteUrl = $"https://discordapp.com/api/oauth2/authorize?client_id={_client.CurrentUser.Id}&permissions=2080374975&scope=bot%20applications.commands";

            await ReplyFormattedAsync("", $"Aqui está [**seu link**]({inviteUrl}) para me adicionar em seu servidor.\nEspero encontrar você lá! 😎", Color.Blue);//(Loc.Get("Common.Invite"), Loc.Get("Common.InviteDescription").Format(inviteUrl));
        }
    }
}
