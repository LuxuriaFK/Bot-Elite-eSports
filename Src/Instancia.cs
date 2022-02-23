/*
 * Arquivo: Instancia.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord
{
    public static class Instancia
    {
        internal static ConcurrentDictionary<ParamsKey, string> _configs = new();
        internal static ConcurrentDictionary<int, BOTStatusModel> _status = new();
    }
}
