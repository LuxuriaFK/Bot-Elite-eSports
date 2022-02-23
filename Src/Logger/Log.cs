/*
 * Arquivo: Log.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 4-10-2021
 */
using Discord;
using System;

namespace BOTDiscord.Logger
{
    public static class Log
    {
        internal static void Cmd(string text) 
            => LoggerManager.Save("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] " + text, "command");

        internal static void Error(string text) 
            => LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] " + text, "Error", ConsoleColor.Red, true);

        internal static void Error(string local, string text)
            => LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][" + local + "] " + text, "Error", ConsoleColor.Red, true);

        internal static void Warning2(string text)
            => LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][ALERTA] " + text, "System", ConsoleColor.Yellow, true);

        internal static void Info(string text, bool saveLine = true, bool show = false)
        {
            if (saveLine)
                LoggerManager.Write("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + text, "System", ConsoleColor.Gray, false);
            if (show)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        internal static void Discord(string text, bool show = false)
            => LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][Discord] " + text, "Discord", ConsoleColor.DarkYellow, show);

        internal static void DiscordCMD(string text, bool show = false)
            => LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][Discord] " + text, "Discord/Commands", ConsoleColor.DarkYellow, show);

        internal static void Gateway(LogSeverity logSeverity, Exception exception, string source, string message, bool show = false)
        {
            ConsoleColor color;
            try
            {
                color = logSeverity switch
                {
                    LogSeverity.Critical => ConsoleColor.DarkRed,
                    LogSeverity.Error => ConsoleColor.Red,
                    LogSeverity.Warning => ConsoleColor.DarkYellow,
                    LogSeverity.Verbose => ConsoleColor.Yellow,
                    LogSeverity.Debug => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Gray,
                };
                message = string.IsNullOrWhiteSpace(message) && exception != null ? exception.Message : message;
                LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][" + source + "] " + message, "Discord/Gateway", color, show);
                if (exception != null)
                {
                    Debug("Log.Gateway", "[" + source + "] " + message + "\n" + exception.ToString());
                    LoggerManager.Save("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][" + source + "] " + exception.ToString(), "Discord/Gateway");
                }
            }
            catch
            {
            }
        }

        internal static void Debug(string local, string text)
        {
#if DEBUG
            LoggerManager.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][DEBUG][" + local + "] " + text, "Debug", ConsoleColor.DarkGray, true);
#endif
        }
    }
}
