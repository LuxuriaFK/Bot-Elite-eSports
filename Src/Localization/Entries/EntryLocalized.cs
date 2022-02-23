/*
 * Arquivo: EntryLocalized.cs
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
    public class EntryLocalized : EntryString
    {
        public EntryLocalized(string id) : base(id) { }

        public EntryLocalized(string id, params object[] args) : base(id, args) { }
        public EntryLocalized(string id, params Func<object>[] args) : base(id, args) { }

        private protected override string GetFormatString(ILocalizationProvider provider)
        {
            return provider.Get(Content);
        }

        public new EntryLocalized Add(params string[] args)
        {
            base.Add(args);
            return this;
        }

        public new EntryLocalized Add(params Func<string>[] args)
        {
            base.Add(args);
            return this;
        }
    }
}
