/*
 * Arquivo: IEntry.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using BOTDiscord.Localization.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Localization.Entries
{
    public interface IEntry
    {
        string Get(ILocalizationProvider provider, params object[] additionalArgs);
    }
}
