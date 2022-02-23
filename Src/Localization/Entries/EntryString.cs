/*
 * Arquivo: EntryString.cs
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
    public class EntryString : IEntry
    {
        public string Content { get; set; }

        public EntryString(string content)
        {
            Content = content;
        }

        public EntryString(string content, params object[] args) : this(content)
        {
            FormatArgs = args.Select(o => new Func<object>(() => o)).ToList();
        }

        public EntryString(string content, params Func<object>[] args) : this(content)
        {
            FormatArgs = args.ToList();
        }

        private bool _isCalculated;
        private List<Func<object>> FormatArgs { get; set; } = new List<Func<object>>();

        public EntryString Add(params string[] args)
        {
            FormatArgs.AddRange(args.ToList().Select<string, Func<string>>(s => () => s));

            return this;
        }

        public EntryString Add(params Func<string>[] args)
        {
            _isCalculated = true;
            FormatArgs.AddRange(args);
            return this;
        }

        private protected virtual string GetFormatString(ILocalizationProvider provider)
        {
            return Content;
        }

        private ILocalizationProvider _lastProvider = null!;
        private string _cache = null!;

        public string Get(ILocalizationProvider provider, params object[] additionalArgs)
        {
            if (_isCalculated || _lastProvider != provider || additionalArgs.Length != 0)
            {
                var formatArgs = FormatArgs.ToList().Concat(additionalArgs.Select(o => new Func<object>(() => o)))
                                        .Select(func =>
                                        {
                                            var result = func();
                                            return result is IEntry loc ? loc.Get(provider) : result;
                                        }).ToArray();
                _cache = formatArgs.Length == 0 ? GetFormatString(provider) : string.Format(GetFormatString(provider), formatArgs);
                _lastProvider = provider;
            }

            return _cache;
        }

        public static implicit operator EntryString(string s)
        {
            return new EntryString(s);
        }
    }
}
