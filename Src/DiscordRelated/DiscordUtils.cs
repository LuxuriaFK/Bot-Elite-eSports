/*
 * Arquivo: DiscordUtils.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using BOTDiscord.Localization.Providers;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.DiscordRelated
{
    public static class DiscordUtils
    {
        public static EmbedBuilder GetAuthorEmbedBuilder(IUser user, ILocalizationProvider? loc)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithFooter(loc?.Get("Commands.RequestedBy", user.Username), user.GetAvatarUrl());
            return embedBuilder;
        }

        public static EmbedFooterBuilder GetFooterEmbedBuilder(IUser user, ILocalizationProvider? loc)
        {
            var embedBuilder = new EmbedFooterBuilder
            {
                Text = loc?.Get("Commands.RequestedBy", user.Username),
                IconUrl = user.GetAvatarUrl()
            };
            return embedBuilder;
        }

        public static PriorityEmbedBuilderWrapper GetAuthorEmbedBuilderWrapper(IUser user, ILocalizationProvider loc)
        {
            var embedBuilder = new PriorityEmbedBuilderWrapper();
            embedBuilder.WithFooter(loc.Get("Commands.RequestedBy", user.Username), user.GetAvatarUrl());
            return embedBuilder;
        }
    }
}
