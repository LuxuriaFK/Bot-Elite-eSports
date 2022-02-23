/*
 * Arquivo: SocketGuildUserExtension.cs
 * Criado em: 11-10-2021
 * https://github.com/ForceFK
 * Última modificação: 11-10-2021
 */
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class SocketGuildUserExtension
    {
        internal static Color GetColor(this SocketGuildUser user, Color def = default)
        {
            SocketRole role = user.Roles.Where(x => x.Color != default).OrderByDescending(x => x.Position).FirstOrDefault()!;
            if (role != null)
                return role.Color;
            return def;
        }

        public static bool PermSendMessage(this SocketGuildUser user, ITextChannel channel)
        {
            var bot = user.GetPermissions(channel);
            return bot.SendMessages;
        }

        public static bool CheckPerm(this SocketGuildUser user, ITextChannel channel, ChannelPermission perm)
            => user.GetPermissions(channel).Has(perm);

    }
}
