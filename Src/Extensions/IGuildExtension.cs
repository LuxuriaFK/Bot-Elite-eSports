/*
 * Arquivo: IGuildExtension.cs
 * Criado em: 11-10-2021
 * https://github.com/ForceFK
 * Última modificação: 11-10-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Managers;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class IGuildExtension
    {
        public static async Task<GuildConfig> GetConfigAsync(this IGuild? guild, bool create = true)
              => await GuildManager.GetAsync(guild is null ? 0U : guild.Id, create);

        public static async Task<GuildConfig> GetConfigAsync(this SocketGuild? guild, bool create = true)
              => await GuildManager.GetAsync(guild is null ? 0U : guild.Id, create);
    }
}
