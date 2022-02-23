/*
 * Arquivo: StringExtension.cs
 * Criado em: 18-6-2021
 * https://github.com/ForceFK
 * Última modificação: 18-10-2021
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class StringExtension
    {
        public static string FilterOutMentions(this string str)
        {
            var pattern = new Regex(@"(@everyone|@here|<@!|<@)");
            return pattern.Replace(str, "");
        }

        public static ulong GetUserID(this string str)
        {
            if (str.Split('@').Length == 2 && ulong.TryParse(str.FilterOutMentions().Split('>')[0], out ulong r))
                return r;
            else
                return 0;
        }

        public static bool ContainsMentions(this string str) 
            => Regex.Match(str, "(@everyone|@here|<@)").Success;

        public static bool ContainsEveryone(this string str) 
            => Regex.Match(str, "(@everyone|@here)").Success;

        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString());

        public static string ReplaceToHTML(this string str)
            => str.Replace(Environment.NewLine, "<br>").Replace("\r", "<br>");

        public static string Format(this string format, params object?[] args) 
            => string.Format(format, args);

        public static string? SafeSubstring(this string? text, int start, int length)
        {
            if (text == null) 
                return null;

            return text.Length <= start ? ""
                : text.Length - start <= length ? text[start..]
                                                  : text.Substring(start, length);
        }

        public static string? SafeSubstring(this string text, int length, string postContent = "")
        {
            if (text == null) 
                return null;

            return text.Length <= length ? text 
                : text.Substring(0, length - postContent.Length) + postContent;
        }

        public static string Repeat(this string s, int count)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (count <= 0) return string.Empty;
            var builder = new StringBuilder(s.Length * count);

            for (var i = 0; i < count; i++) builder.Append(s);

            return builder.ToString();
        }

        /// <summary>
        /// Quebra uma string no comprimento especificado.
        /// </summary>
        /// <param name="text">string a ser quebrada.</param>
        /// <param name="maxLength">O comprimento máximo da string.</param>
        /// <returns>String cortada.</returns>
        public static string? Truncate(this string text, int maxLength) 
            => text?.Substring(0, Math.Min(text.Length, maxLength));

        /// <summary>
        /// Método de extensão para validação rápida de string.
        /// WARN: Se true, o método IsNullOrWhiteSpace está implícito
        /// </summary>
        public static bool IsEmpty(this string source)
            => string.IsNullOrWhiteSpace(source);

        /// <summary>
        /// Método de extensão para obtenção rápida de strings. 
        /// WARN: Se true, o método IsNullOrWhiteSpace está implícito
        /// </summary>
        /// <param name="source">String de destino</param>
        /// <param name="replacement">Nova string</param>
        /// <returns>Se a string de destino for nula ou um espaço em branco - return <paramref name="replacement"/>. else - return <paramref name="source"/></returns>
        public static string IsEmpty(this string source, string replacement)
            => string.IsNullOrWhiteSpace(source) ? replacement : source;

        /// <summary>
        /// Verifica e desabilita as formatações via escrita do discord
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DisableDCFormat(this string value)
            => value.Replace("*", @"\*").Replace("`", @"`\").Replace("~~", @"\~~").Replace("_", @"\_");

        /// <summary>
        /// Transforma uma string escrita de um valor em inteiro 64
        /// </summary>
        /// <param name="value">Valor a ser convertido</param>
        /// <returns></returns>
        public static long GetLong(this string value)
            => long.Parse(value.Replace("k", "000").Replace(".", "").Replace(",", ""));

        /// <summary>
        /// Transforma um HEX em Cor Decimal
        /// </summary>
        /// <param name="str">HEX Color</param>
        /// <returns>INT</returns>
        public static int ToColor(this string str)
        {
            int hash = 0;
            foreach (char ch in str)
            {
                hash = ch + ((hash << 5) - hash);
            }
            return hash;
            //string c = (hash & 0x00FFFFFF).ToString("X4").ToUpperInvariant();

            //return "00000".Substring(0, 6 - c.Length) + c;
        }
    }
}
