/*
 * Arquivo: GuildConfig.cs
 * Criado em: 18-6-2021
 * https://github.com/ForceFK
 * ForceFK - FK Team
 * Última modificação: 27-7-2021
 */
using BOTDiscord.Localization.Providers;
using BOTDiscord.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.DataBase.SQLite.Tables
{
    public class GuildConfig
    {
        private ILocalizationProvider _loc = null!;
        public static event EventHandler<string> LocalizationChanged = null!;
        [Key]
        public ulong GuildID { get; set; }
        [NotNull]
        public char Prefix { get; set; } = '.';
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public DateTime LastCommand { get; set; } = DateTime.Now;
        [NotNull]
        public string GuildLanguage { get; set; } = "pt-br";
        public ILocalizationProvider Loc => _loc ??= new GuildLocalizationProvider(this);

        internal string GetLanguage() => GuildLanguage;

        /// <summary>
        /// Adiciona no database
        /// </summary>
        /// <returns></returns>
        internal async Task Save() => await GuildManager.Insert(this);

        /// <summary>
        /// Define uma nova linguagem de resposta
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task<GuildConfig> SetLanguageAsync(string language)
        {
            if (GuildID != 0)
            {
                GuildLanguage = language;
                using SDBContext db = GuildManager.GetContext();
                db.Entry(this).Property(x => x.GuildLanguage).IsModified = true;
                await db.SaveChangesAsync();
                LocalizationChanged?.Invoke(this, language);
            }
            return this;
        }

        /// <summary>
        /// Define um novo prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<GuildConfig> SetPrefixAsync(char prefix)
        {
            if (GuildID != 0)
            {
                Prefix = prefix;
                using SDBContext db = GuildManager.GetContext();
                db.Entry(this).Property(x => x.Prefix).IsModified = true;
                await db.SaveChangesAsync();
            }
            return this;
        }

        public GuildConfig CommandExecutad()
        {
            if (GuildID != 0)
            {
                LastCommand = DateTime.Now;

                using SDBContext db = GuildManager.GetContext();
                //pode da pau com o leave
                if (db.Guilds_Configs.Any(x => x.GuildID == GuildID))
                {
                    db.Entry(this).Property(x => x.LastCommand).IsModified = true;
                    db.SaveChanges();
                }
            }
            return this;
        }

    }
}
