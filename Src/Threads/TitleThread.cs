/*
 * Arquivo: TitleThread.cs
 * Criado em: 9-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 9-6-2021
 */
using BOTDiscord.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BOTDiscord.Threads
{
    class TitleThread
    {
        internal static void Start() => new Thread(Run).Start();

        private static void Run()
        {
            Log.Debug("ThreadStart", "TitleThread");
            while (true)
            {
                Console.Title = $"[BOT Discord]{MainClient._title} {Program._title}  -  RAM: { GC.GetTotalMemory(true) / 1024}KB";
                Thread.Sleep(1000);
            }

        }
    }
}
