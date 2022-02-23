/*
 * Arquivo: UserConfig.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.DataBase.SQLite.Tables
{
    public class UserConfig
    {
        [Key]
        public ulong UserID { get; set; }

        internal DateTime NextCommand;
    }
}
