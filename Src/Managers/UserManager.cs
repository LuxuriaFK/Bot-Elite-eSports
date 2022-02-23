/*
 * Arquivo: UserManager.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.DataBase.SQLite.Tables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Managers
{
    public static class UserManager
    {
        private static readonly ConcurrentDictionary<ulong, UserConfig> _configCache = new();
        internal static SDBContext GetContext() => new();

        internal static Task<UserConfig> GetAsync(ulong userid, bool create = false)
        {
            return Task.FromResult(_configCache.GetOrAdd(userid, arg =>
            {
                if (userid == 0U)
                    return new();
                using SDBContext db = new();
                return db.Users_Configs.FirstOrDefault(x => x.UserID == userid) ?? (create ? TryCreate(db, userid) : new() { UserID = userid });
            }));
        }

        private static UserConfig TryCreate(SDBContext db, ulong userid)
        {
            UserConfig userConfig = new() { UserID = userid };
            db.Users_Configs.Add(userConfig);
            db.SaveChanges();
            return userConfig;
        }
    }
}
