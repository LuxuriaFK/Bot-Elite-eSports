/*
 * Arquivo: LocalizationManager.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 8-10-2021
 */
using BOTDiscord.Extensions;
using BOTDiscord.Logger;
using BOTDiscord.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Localization
{
    internal static class LocalizationManager
    {
        public static readonly Dictionary<string, LocalizationPack> Languages;
#pragma warning disable CS8714 // O tipo não pode ser usado como parâmetro de tipo no tipo ou método genérico. A nulidade do argumento de tipo não corresponde à restrição 'notnull'.

        static LocalizationManager()
        {
            Log.Discord("Carregando pacotes de idiomas...", true);
            try
            {
                Dictionary<string?, string>? indexes = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "LocalizationSTR")).ToDictionary(Path.GetFileNameWithoutExtension);
                
                Dictionary<string?, LocalizationPack> localizationPacks = indexes.ToDictionary(variable => variable.Key,
                    variable => JsonConvert.DeserializeObject<LocalizationPack>(All.DownloadString(variable.Value)));

                var localizationEntries = localizationPacks["pt-br"].Data.SelectMany(groups => groups.Value.Select(pair => groups.Key + pair.Key)).ToList();
                foreach (var pack in localizationPacks)
                {
                    var entriesNotLocalizedCount = pack.Value.Data.SelectMany(groups => groups.Value.Select(pair => groups.Key + pair.Key))
                                                       .Count(s => localizationEntries.Contains(s));

                    pack.Value.TranslationCompleteness = (int)(entriesNotLocalizedCount / (double)localizationEntries.Count * 100);
                }

                Languages = localizationPacks!;
                Log.Discord(string.Format("Idiomas carregados: {0}.", string.Join(", ", Languages.Select(pair => $"{pair.Key} - {pair.Value.TranslationCompleteness}%"))), true);
            }
            catch (Exception e)
            {
                Log.Error("LanguageManager", "Erro ao baixar bibliotecas: " + e.Message);
                Log.Discord("Carregando pacote padrão (pt-br).", true);
                Languages = new Dictionary<string, LocalizationPack> {
                    {
                        "pt-br",
                        JsonConvert.DeserializeObject<LocalizationPack>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "LocalizationSTR/pt-br.json")))
                    }
                };
            }
            finally
            {
                Log.Discord("Todos os pacotes de idiomas foram carregados.", true);
            }
        }

        public static void Initialize()
        {
            //Método fictício para chamar o construtor estático
        }

        public static string Get(string lang, string id, params object[]? args)
        {
            var split = id.Split('.');

            var s = Get(lang, split[0], split[1]);
            try
            {
                return args == null || args.Length == 0 ? s : s.Format(args);
            }
            catch (Exception e)
            {
                Log.Error("LanguageManager.Get", "Falha ao formatar string de idioma\n" + e.Message);
                return s;
            }
        }

        public static string Get(string lang, string group, string id)
        {
            Log.Debug("LanguageManager.Get", string.Format("Solicitado {0}.{1} no idioma {2}", group, id, lang));
            if (Languages.TryGetValue(lang, out var pack) &&
                pack.Data.TryGetValue(group, out var reqGroup) &&
                reqGroup.TryGetValue(id, out var reqText))
            {
                return reqText;
            }

            if (pack?.FallbackLanguage == null)
            {
                Log.Error("LanguageManager.Get", $"Falha ao carregar {group}.{id} no idioma pt-br");
                return $"{group}.{id}";
            }

            Log.Discord(string.Format("Falha ao carregar {0}.{1} no idioma {2}", group, id, lang));
            return Get(pack.FallbackLanguage, group, id);
        }
    }
}
