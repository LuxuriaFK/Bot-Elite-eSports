/*
 * Arquivo: ILocalizationProvider.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Localization.Providers
{
    public interface ILocalizationProvider
    {
        string Get(string id, params object[]? formatArgs);

        event EventHandler? LanguageChanged;
    }
}
