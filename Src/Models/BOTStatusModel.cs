/*
 * Arquivo: BOTStatusModel.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.Extensions;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Models
{
    public class BOTStatusModel
    {
        public UserStatus userStatus { get; set; }
        public string? Status { get; set; }
        public string? StreamUrl { get; set; } = null;
        public ActivityType Activity { get; set; } = 0;
        public int Await { get; set; } = 20000;

        internal async Task Execute(DiscordSocketClient Client)
        {
            if (!Status.IsEmpty())
            {
                string? status = Format(Client);

                await Client.SetGameAsync(status, StreamUrl, Activity);
                await Task.Delay(Await);
            }
        }
        internal string? Format(DiscordSocketClient Client) => Status?
            .Replace("{discordversion}", DiscordConfig.Version)
            .Replace("{discordapi}", DiscordConfig.APIVersion.ToString())
            .Replace("{uptime}", Process.GetCurrentProcess().CalculateUptime())
            .Replace("{version}", Assembly.GetExecutingAssembly().GetName().Version!.ToString())
            .Replace("{guildscount}", Client.Guilds.Count.ToString());
    }
}
