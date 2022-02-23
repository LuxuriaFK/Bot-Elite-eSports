/*
 * Arquivo: IUserExtension.cs
 * Criado em: 11-10-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Managers;
using Discord;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class IUserExtension
    {
        public static async Task<UserConfig> GetConfigAsync(this IUser? user, [Optional] bool create)
             => await UserManager.GetAsync(user is null ? 0U : user.Id, create);

        public static async Task<bool> TrySendMessageAsync(this IUser user, [Optional] string text, [Optional] bool isTts, [Optional] Embed embed, [Optional] RequestOptions options)
        {
            try
            {
                await user.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }

    }
}
