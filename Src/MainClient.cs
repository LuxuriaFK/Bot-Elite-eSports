/*
 * Arquivo: MainClient.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.DataBase.SQLite;
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Extensions;
using BOTDiscord.Localization;
using BOTDiscord.Logger;
using BOTDiscord.Manager.Commands;
using BOTDiscord.Managers;
using BOTDiscord.Utilities.Collector;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BOTDiscord
{
#pragma warning disable CS8618 // O campo não anulável precisa conter um valor não nulo ao sair do construtor. Considere declará-lo como anulável.
    public class MainClient
    {
        private DiscordSocketClient Client;
        internal static string _title;
        private bool _clientStarted;
        internal DateTime _started;

        internal async Task Run()
        {
            Log.Discord("Carregando configurações...", true);
            using SDBContext sDB = new();

            Instancia._configs.TryAdd(ParamsKey.BOT_TOKEN, (await sDB.System_Params.FirstOrDefaultAsync(x => x.Key == ParamsKey.BOT_TOKEN)).Value);

            await SystemParamsManager.Load();

            //Shards = limit atual 3 / cada shars suporta 2500 servers / shard 0 sempre para pv / se usar o comando dos grupos nao pode ser continuado no pv
            Client = new DiscordSocketClient(new()
            {
                MessageCacheSize = 100,
                ExclusiveBulkDelete = true // Gera apenas o  MessagesBulkDeleted
            });
            _title = $"[{Client.CurrentUser}][Status: {Client.ConnectionState}]";
            Client.Log += OnClientLog;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.Connected += Client_Connected;
            Log.Discord("Sistema inicializado com sucesso!");
            Log.Discord("Iniciando o login no Gateway...", true);

            var connectDelay = 30;
            while (true)
            {
                try
                {
                    await Client.LoginAsync(TokenType.Bot, Instancia._configs[ParamsKey.BOT_TOKEN]);
                    Log.Discord("Logado com sucesso no Gateway!", true);
                    break;
                }
                catch (Exception e)
                {
                    if (connectDelay < 100)
                    {
                        Log.Discord($"Falha ao Logar, possível TOKEN errado![{e.Message}]", true);
                        Log.Discord(string.Format("Esperando antes da próxima tentativa - {0}s", connectDelay));
                        await Task.Delay(TimeSpan.FromSeconds(connectDelay));
                        connectDelay += 10;
                    }
                    else
                    {
                        Log.Discord($"Impossível logar![{e.Message}]", true);
                        Console.WriteLine("Resetar o TOKEN? (s/n)");
                        if (Console.ReadLine()?.ToLower() == "s")
                        {
                            sDB.System_Params.Update(await Program.InitialSettings());
                            await sDB.SaveChangesAsync();
                            Instancia._configs[ParamsKey.BOT_TOKEN] = (await sDB.System_Params.FirstOrDefaultAsync(x => x.Key == ParamsKey.BOT_TOKEN))!.Value;
                        }
                        connectDelay = 30;
                    }
                }
            }
            LocalizationManager.Initialize();

            var services = SetupServices();

            //Resolver dependencias

            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            await StartClient();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
        {
            Log.Discord("Criando serviços...", true);
            CommandService commandService = new(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
            return new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(commandService)
                .AddSingleton<CommandHandler>()
                .AddSingleton<CollectorsUtils>()
                .BuildServiceProvider();
        }

        private Task Client_Connected()
        {
            _title = $"[{Client.CurrentUser}][ShardID: {Client.ShardId}][Guilds: {Client.Guilds.Count}][Status: {Client.ConnectionState}]";
            return Task.CompletedTask;
        }

        private Task OnClientLog(LogMessage message)
        {
            Log.Gateway(message.Severity, message.Exception, message.Source, message.Message, true);
            _title = $"[{Client.CurrentUser}][ShardID: {Client.ShardId}][Guilds: {Client.Guilds.Count}][Status: {Client.ConnectionState}]";
            return Task.CompletedTask;
        }

        private async Task Client_LeftGuild(SocketGuild arg)
        {
            await (await GuildManager.GetAsync(arg)).Remove();
            _title = $"[{Client.CurrentUser}][ShardID: {Client.ShardId}][Guilds: {Client.Guilds.Count}][Status: {Client.ConnectionState}]";
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            GuildConfig guildconfig = new() { GuildID = arg.Id };
            await guildconfig.Insert();

            EmbedBuilder welcome = new() { Title = guildconfig.Loc.Get("Welcome.Title"), Description = guildconfig.Loc.Get("Welcome.Description", guildconfig.Prefix), Color = Color.Gold };

            ITextChannel skc = arg.SystemChannel ?? arg.DefaultChannel;
            if (arg.CurrentUser.GetPermissions(skc).SendMessages)
                await skc.SendMessageAsync(null, false, welcome.Build());
            _title = $"[{Client.CurrentUser}][ShardID: {Client.ShardId}][Guilds: {Client.Guilds.Count}][Status: {Client.ConnectionState}]";
        }

        public async Task StartClient()
        {
            if (_clientStarted)
                return;
            _clientStarted = true;
            Log.Discord("Iniciando o Cliente...", true);
            await Client.StartAsync();
            _started = DateTime.Now; 
            Log.Debug("ThreadStart", "SetStatus");
            new Thread(SetStatus).Start();
            //_reliabilityService = new ReliabilityService(Client, OnClientLog);
        }

        private async void SetStatus()
        {
            if (Instancia._status.ContainsKey(0))
                await Client.SetStatusAsync(Instancia._status[0].userStatus);
            else
            {
                Instancia._status.TryAdd(0, new() { userStatus = UserStatus.Online, Await = 0 });
                await SystemParamsManager.SaveBotStatus();
                await Client.SetStatusAsync(Instancia._status[0].userStatus);
            }

            Thread.Sleep(1000);
            while (_clientStarted)
            {
                if (!Instancia._status.Where(x => x.Key > 0).Any())
                {
                    await Client.SetGameAsync("");
                    await Task.Delay(20000);
                }
                else
                    foreach (var item in Instancia._status.Where(x => x.Key > 0))
                        await item.Value.Execute(Client);
            }
        }
    }
}
