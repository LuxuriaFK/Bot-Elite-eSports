/*
 * Arquivo: PingCommand.cs
 * Criado em: 18-10-2021
 * https://github.com/ForceFK
 * Última modificação: 18-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Outros
{
    [Grouping("misc")]
    public class PingCommand : AdvancedModuleBase
    {
        private readonly DiscordSocketClient _client;

        public PingCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("ping")]
        [Summary("Ping0")]
        public async Task CMD() => await Run();

        private async Task Run()
        {
            //Calcular o tempo de envio de uma mensagem ao discord
            var sw = Stopwatch.StartNew();
            IUserMessage msg = await ReplyAsync(embed: new EmbedBuilder().WithTitle(Format.Bold("Pong! 🏓")).Build());
            sw.Stop();

            var builder = new EmbedBuilder()
                .WithDescription($"⏱{Format.Bold(Loc.Get("Other.Message"))}: {sw.ElapsedMilliseconds}ms\n\n" +
                                 $"📡{Format.Bold("WebSocket")}: {_client.Latency}ms\n")
                .WithColor(Constants.DiscordBlurple);

            await msg.ModifyAsync(x => x.Embed = builder.Build());
        }
    }
}
