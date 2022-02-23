/*
 * Arquivo: PatchableModuleBase.cs
 * Criado em: 9-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using BOTDiscord.Extensions;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Manager.Commands.Modules
{
    public class PatchableModuleBase : ModuleBase<SocketCommandContext>
    {
        public CommandInfo CurrentCommandInfo = null!;

        protected override void BeforeExecute(CommandInfo command)
        {
            CurrentCommandInfo = command;
            base.BeforeExecute(command);
        }

        protected override async void AfterExecute(CommandInfo command)
        {
            //log de data do comando?
            (await Context.Guild.GetConfigAsync()).CommandExecutad();
            base.AfterExecute(command);
        }
    }
}
