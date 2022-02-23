/*
 * Arquivo: LocalizationPack.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Localization
{
    public class LocalizationPack
    {
        private string _localizationFlagEmojiText = null!;
        public string LanguageName { get; set; } = null!;
        public string LocalizedName { get; set; } = null!;

        [JsonProperty("LocalizationFlagEmoji")]
        public string LocalizationFlagEmojiText
        {
            get => _localizationFlagEmojiText;
            set
            {
                _localizationFlagEmojiText = value;
                try
                {
                    LocalizationFlagEmoji = new Discord.Emoji(value);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        [JsonIgnore]
        public IEmote LocalizationFlagEmoji { get; set; } = null!;

        public string FallbackLanguage { get; set; } = null!;
        public string Authors { get; set; } = null!;
        public Dictionary<string, Dictionary<string, string>> Data { get; set; } = null!;

        [JsonIgnore]
        public int TranslationCompleteness { get; set; }
    }
}
