/*
 * Arquivo: SystemParams.cs
 * Criado em: 8-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 11-6-2021
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.DataBase.SQLite.Tables
{
    public class SystemParams
    {
        [Key]
        [NotNull]
        public ParamsKey Key { get; set; }
        [NotNull]
        public string? Value { get; set; }
    }
}
