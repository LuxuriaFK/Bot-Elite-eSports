/*
 * Arquivo: SDBContext.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.DataBase.SQLite
{
    public class SDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=source.db");

#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        
        public DbSet<SystemParams> System_Params { get; set; }
        public DbSet<GuildConfig> Guilds_Configs { get; set; }
        public DbSet<UserConfig> Users_Configs { get; set; }

#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    }
}
