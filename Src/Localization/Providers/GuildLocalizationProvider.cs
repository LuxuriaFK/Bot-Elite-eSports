/*
 * Arquivo: GuildLocalizationProvider.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Localization.Providers
{
    public class GuildLocalizationProvider : ILocalizationProvider
    {
        private GuildConfig _guildConfig;

        public GuildLocalizationProvider(GuildConfig guildConfig)
        {
            _guildConfig = guildConfig;
            GuildConfig.LocalizationChanged += (sender, s) =>
            {
                if (sender is GuildConfig config && config.GuildID == _guildConfig.GuildID)
                {
                    _guildConfig = config;
                    OnLanguageChanged();
                }
            };
        }

        public string Get(string id, params object[]? formatArgs)
        {
            return LocalizationManager.Get(_guildConfig.GetLanguage(), id, formatArgs);
        }

        public event EventHandler? LanguageChanged;

        protected virtual void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
