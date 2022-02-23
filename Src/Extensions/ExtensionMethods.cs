/*
 * Arquivo: ExtensionMethods.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 11-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.DiscordRelated;
using BOTDiscord.Localization.Providers;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Managers;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class ExtensionMethods
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
            => !enumerable.Any();

        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static bool IsMatch(this Regex regex, string str, out Match match)
        {
            match = regex.Match(str);
            return match.Success;
        }

        public static void DelayedDelete(this IMessage message, TimeSpan span)
        {
            Task.Run(async () =>
            {
                await Task.Delay(span);
                message.SafeDelete();
            });
        }

        public static void DelayedDelete(this Task<IUserMessage> message, TimeSpan span)
        {
            Task.Run(async () =>
            {
                await Task.Delay(span);
                (await message.ConfigureAwait(false)).SafeDelete();
            });
        }

        public static void SafeDelete(this IMessage message)
        {
            try
            {
                message?.DeleteAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static async Task<bool> IsBotOwnerAsync(this ICommandContext cmd)
        {
            var application = await cmd.Client.GetApplicationInfoAsync().ConfigureAwait(false);
            return cmd.User.Id == application.Owner.Id;
        }
        
        public static bool IsGuildOwner(this ICommandContext cmd)
            => cmd.Guild.OwnerId == cmd.User.Id;

        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"Argumento {typeof(T).FullName} não  é um Enum");

            var arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf(arr, src) + 1;
            return arr.Length == j ? arr[0] : arr[j];
        }

        public static EmbedBuilder GetAuthorEmbedBuilder(this AdvancedModuleBase moduleBase)
            => DiscordUtils.GetAuthorEmbedBuilder(moduleBase.Context.User, moduleBase.Loc);

        public static int Normalize(this int value, int min, int max)
            => Math.Max(min, Math.Min(max, value));

        public static async Task<IMessage> SendTextAsFile(this IMessageChannel channel, string content, string filename, [Optional]string text,
                                                          [Optional] bool isTTS, [Optional] Embed embed, [Optional] RequestOptions options, [Optional] bool isSpoiler)
        {
            using var ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            await tw.WriteAsync(content);
            await tw.FlushAsync();
            ms.Position = 0;

            return await channel.SendFileAsync(ms, filename);
        }

        public static TResult Try<TSource, TResult>(this TSource o, Func<TSource, TResult> action, Func<TSource, TResult> onFail)
        {
            try
            {
                return action(o);
            }
            catch
            {
                return onFail(o);
            }
        }

        /// <summary>
        /// var query = msg.Content.Try(s1 => s1.Substring(argPos), "");
        /// </summary>
        public static TResult Try<TSource, TResult>(this TSource o, Func<TSource, TResult> action, TResult onFail)
        {
            try
            {
                return action(o);
            }
            catch
            {
                return onFail;
            }
        }

       

        public static string FormattedToString(this TimeSpan span)
        {
            string s = $"{span:mm':'ss}";
            if ((int)span.TotalHours != 0)
                s = s.Insert(0, $"{(int)span.TotalHours}:");
            return s;
        }
    }
}
