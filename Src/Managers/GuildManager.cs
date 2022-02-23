/*
 * Arquivo: GuildManager.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.DataBase.SQLite.Tables;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Managers
{
    public static class GuildManager
    {
        private static readonly ConcurrentDictionary<ulong, GuildConfig> _configCache = new();

        internal static async Task<GuildConfig> GetAsync(IGuild guild) => await GetAsync(guild is null ? 0U : guild.Id);

        internal static async Task<GuildConfig> GetAsync(SocketGuild guild, [Optional]bool create) => await GetAsync(guild is null ? 0U : guild.Id, create);

        internal static Task<GuildConfig> GetAsync(ulong guildid, bool create = true)
        {
            return Task.FromResult(_configCache.GetOrAdd(guildid, arg =>
            {
                if (guildid == 0)
                    return new GuildConfig();
                using SDBContext db = new();
                return  db.Guilds_Configs.FirstOrDefault(x => x.GuildID == guildid) ?? (create ? TryCreate(db,guildid): new());
            }));
        }

        private static GuildConfig TryCreate(SDBContext db, ulong guildid)
        {
            GuildConfig guild = new() { GuildID = guildid };
            db.Guilds_Configs.Add(guild);
            db.SaveChanges();
            return guild;
        }

        internal static async Task Insert(this GuildConfig guildConfig)
        {
            if (guildConfig.GuildID > 0)
            {
                using SDBContext db = new();

                if (!await db.Guilds_Configs.AnyAsync(x => x.GuildID == guildConfig.GuildID))
                {
                    db.Guilds_Configs.Add(guildConfig);
                    if (await db.SaveChangesAsync() > 0)
                        _configCache.TryAdd(guildConfig.GuildID,guildConfig);
                }
            }
        }

        internal static async Task Remove(this GuildConfig guildConfig)
        {
            if (guildConfig.GuildID > 0)
            {
                using SDBContext db = new();


                db.Guilds_Configs.Remove(guildConfig);
                if (await db.SaveChangesAsync() > 0)
                    _configCache.TryRemove(guildConfig.GuildID, out _);
            }
        }

        internal static SDBContext GetContext() => new(); 
    }
}
