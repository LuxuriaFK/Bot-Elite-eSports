/*
 * Arquivo: IGuildUserExtension.cs
 * Criado em: 18-10-2021
 * https://github.com/ForceFK
 * Última modificação: 18-10-2021
 */
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class IGuildUserExtension
    {
        public static int GetHierarchy(this IGuildUser user)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return int.MaxValue;
            }

            int maxPos = 0;
            for (int i = 0; i < user.RoleIds.Count; i++)
            {
                var role = user.Guild.GetRole(user.RoleIds.ElementAt(i));
                if (role != null && role.Position > maxPos)
                {
                    maxPos = role.Position;
                }
            }

            return maxPos;
        }

    }
}
