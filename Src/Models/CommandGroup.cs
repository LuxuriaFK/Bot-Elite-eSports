/*
 * Arquivo: CommandGroup.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Models
{
    public class CommandGroup
    {
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
        public string GroupId;
        public string GroupNameTemplate;
        public string GroupTextTemplate;
        public List<CommandInfo> Commands;
#pragma warning restore CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    }
}
