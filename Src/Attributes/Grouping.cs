/*
 * Arquivo: Grouping.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Attributes
{
    public class GroupingAttribute : Attribute
    {
        public string GroupName { get; set; }
        public GroupingAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
