/*
 * Arquivo: LoggerManager.cs
 * Criado em: 9-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using Discord;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace BOTDiscord.Logger
{
    public static class LoggerManager
    {
        private static readonly string Date = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss");
        private static readonly object Sync = new();

        internal static void Start()
        {
            CheckDirectorys();
            StringBuilder sb = new();
            _ = sb.AppendLine("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] " + Assembly.GetExecutingAssembly().GetName().Name + ".exe iniciado.");
            _ = sb.AppendLine("Versão: " + Assembly.GetExecutingAssembly().GetName().Version);
            _ = sb.AppendLine("Desenvolvedor: Force (Force#0010) - https://github.com/ForceFK");
            _ = sb.AppendLine("Computador: " + Environment.MachineName);
            _ = sb.AppendLine("Usuário: " + Environment.UserName);
            _ = sb.AppendLine("Plataforma: " + (Environment.Is64BitOperatingSystem == true ? "x64" : "x86"));
            Save(sb.ToString(), "System");
        }

        internal static void Write(string text, string type, ConsoleColor color, bool show)
        {
            try
            {
                lock (Sync)
                {
                    if (show)
                    {
                        Console.ForegroundColor = color;
                        Console.WriteLine(text);
                        Console.ResetColor();
                    }
                    Save(text, type);
                }
            }
            catch
            {
            }
        }

        internal static void Save(string text, string type)
        {
            try
            {
                using FileStream fileStream = new("Logs/" + type + "/" + Date + ".log", FileMode.Append);
                using StreamWriter stream = new(fileStream);
                try
                {
                    if (stream != null)
                        stream.WriteLine(text);
                }
                catch
                {
                }
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
                stream.Flush();
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.
                stream.Close();
                fileStream.Flush();
                fileStream.Close();
            }
            catch
            {
            }
        }

        private static void CheckDirectorys()
        {
            try
            {
                if (!Directory.Exists("Logs/Command"))
                    Directory.CreateDirectory("Logs/Command");
                if (!Directory.Exists("Logs/Error"))
                    Directory.CreateDirectory("Logs/Error");
                if (!Directory.Exists("Logs/System"))
                    Directory.CreateDirectory("Logs/System");
                if (!Directory.Exists("Logs/Discord"))
                    Directory.CreateDirectory("Logs/Discord");
                if (!Directory.Exists("Logs/Discord/Commands"))
                    Directory.CreateDirectory("Logs/Discord/Commands");
                if (!Directory.Exists("Logs/Discord/Gateway"))
                    Directory.CreateDirectory("Logs/Discord/Gateway");
                if (!Directory.Exists("Logs/Debug"))
                    Directory.CreateDirectory("Logs/Debug");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Logger.CheckDirectorys]: " + ex.Message);
            }
        }
    }
}
