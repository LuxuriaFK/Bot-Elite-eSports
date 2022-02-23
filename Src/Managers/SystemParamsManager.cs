/*
 * Arquivo: SystemParamsManager.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Logger;
using BOTDiscord.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Managers
{
    public static class SystemParamsManager
    {

        internal static async Task Load()
        {
            using SDBContext db = new();
            if (await db.System_Params.AnyAsync(x => x.Key == ParamsKey.BOT_STATUS))
                Instancia._status = JsonConvert.DeserializeObject<ConcurrentDictionary<int, BOTStatusModel>>((await db.System_Params.FirstAsync(x => x.Key == ParamsKey.BOT_STATUS)).Value, new KeyValuePairConverter())!;
            Log.Discord($"[BOT_STATUS]: {Instancia._status.Count} carregados.", true);
        }

        internal static async Task SaveBotStatus()
        {
            using SDBContext db = new();
            if (await db.System_Params.AnyAsync(x => x.Key == ParamsKey.BOT_STATUS))
            {
                var value = db.Entry(new SystemParams() { Key = ParamsKey.BOT_STATUS });
                //value.State = EntityState.Detached;
                value.Property(x => x.Value).CurrentValue = JsonConvert.SerializeObject(Instancia._status);
                value.Property(x => x.Value).IsModified = true;

            }
            else
                db.System_Params.Add(new() { Key = ParamsKey.BOT_STATUS, Value = JsonConvert.SerializeObject(Instancia._status) });
            await db.SaveChangesAsync();
        }

        internal static SDBContext GetContext() => new();
    }
}
