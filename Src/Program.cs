/*
 * Arquivo: Program.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Logger;
using BOTDiscord.Threads;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace BOTDiscord
{
    class Program
    {
        internal static string _title = "Iniciando...";

        static async Task Main(string[] args)
        { 
            // Exceptions em portugues
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            Logo();
            LoggerManager.Start();
            TitleThread.Start();

            Log.Info($"Versão: {Assembly.GetExecutingAssembly().GetName().Version}", show: true);
            //Vamos ver se o DB existe e esta atualizado antes de iniciar os processos
            using SDBContext sDB = new();
            await sDB.Database.MigrateAsync();

            if (string.IsNullOrWhiteSpace((await sDB.System_Params.FirstOrDefaultAsync(x => x.Key == ParamsKey.BOT_TOKEN))?.Value))
            {
                Log.Error("TOKEN não detectado.");
                await sDB.System_Params.AddAsync(await InitialSettings());
                Console.Clear();
                await sDB.SaveChangesAsync();
                Console.WriteLine("TOKEN salvo");//, use removetoken para altera-lo."); (NÃO FEITO)
                //Console.WriteLine("Para finalizar as configurações, va no discord, me mencione com a escrita initilconfig");
            }

            //inicia o bot
            _title = "";
            await new MainClient().Run();
        }

        internal static Task<SystemParams> InitialSettings()
        {
        dnv:
            Console.ResetColor();
            Console.WriteLine("Informe o TOKEN do BOT:");
            string? token = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(token) || token.Length < 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TOKEN invalido!");
                goto dnv;
            }
            return Task.FromResult(new SystemParams
            {
                Key = ParamsKey.BOT_TOKEN,
                Value = token
            });
        }

        internal static void Logo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("===============================ELITE ESPORTS===============================");
            Console.ResetColor();
            Console.WriteLine("Use o comando [setstatus] para definir um status diferente ao BOT.");
            Console.WriteLine("Use o comando [addactivity] para adicionar atividades simples ao status.");
            Console.WriteLine("Execute [help addactivity+] e descubra como criar atividades avançadas!");
            Console.WriteLine("O comando [activitys] exibe todas as atividades criadas.");
            Console.WriteLine("Em caso de duvida, use [help <comando>] e obtenha dicas de como usa-lo.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("===============================ELITE ESPORTS===============================");
            Console.ResetColor();
        }
    }
}
