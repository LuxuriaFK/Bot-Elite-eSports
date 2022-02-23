/*
 * Arquivo: CommandInfoExtension.cs
 * Criado em: 11-10-2021
 * https://github.com/ForceFK
 * Última modificação: 11-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Localization.Providers;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Extensions
{
    public static class CommandInfoExtension
    {
        public static GroupingAttribute? GetGroup(this CommandInfo info)
            => (info.Attributes.FirstOrDefault(attribute => attribute is GroupingAttribute) ??
                    info.Module.Attributes.FirstOrDefault(attribute => attribute is GroupingAttribute)) as GroupingAttribute;

        public static string GetLocalizedName(this GroupingAttribute groupingAttribute, ILocalizationProvider loc)
            => loc.Get($"Groups.{groupingAttribute?.GroupName ?? ""}");

        public static bool IsHiddenCommand(this CommandInfo info)
            => (info.Attributes.FirstOrDefault(attribute => attribute is HiddenAttribute) ??
                    info.Module.Attributes.FirstOrDefault(attribute => attribute is HiddenAttribute)) != null;

        public static bool IsDEVCommand(this CommandInfo info) 
            => (info.Preconditions.FirstOrDefault(attribute => attribute is RequireOwnerAttribute) ??
                    info.Module.Preconditions.FirstOrDefault(attribute => attribute is RequireOwnerAttribute)) != null;
    }
}
