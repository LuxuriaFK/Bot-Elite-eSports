/*
 * Arquivo: AdvancedModuleBase.cs
 * Criado em: 9-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 17-6-2021
 */
using BOTDiscord.DataBase.SQLite.Tables;
using BOTDiscord.Extensions;
using BOTDiscord.Localization.Providers;
using BOTDiscord.Managers;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Manager.Commands.Modules
{
    public class AdvancedModuleBase : PatchableModuleBase
    {
        private Lazy<GuildLocalizationProvider> _loc = null!;

        public GuildLocalizationProvider Loc => _loc.Value;
        public GuildConfig GuildConfig { get; private set; } = null!;

        public async Task<IMessageChannel> GetResponseChannel(bool fileSupport = false)
        {
            if (Context.Guild != null)
            {
                var bot = Context.Guild.CurrentUser.GetPermissions((IGuildChannel)Context.Channel);
                var user = Context.Guild.GetUser(Context.User.Id).GetPermissions((IGuildChannel)Context.Channel);
                return bot.SendMessages && (!fileSupport || bot.AttachFiles) && (!fileSupport || user.AttachFiles)
                    ? Context.Channel
                    : await Context.User.GetOrCreateDMChannelAsync();
            }
            else
                return await Context.User.GetOrCreateDMChannelAsync();
        }

        protected override async void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            GuildConfig = await GuildManager.GetAsync(Context.Guild, false);
            _loc = new Lazy<GuildLocalizationProvider>(() => new GuildLocalizationProvider(GuildConfig));
        }

        protected override async Task<IUserMessage> ReplyAsync([Optional] string message, [Optional] bool isTTS, [Optional] Embed embed, [Optional] RequestOptions options, [Optional] AllowedMentions allowedMentions, [Optional]MessageReference messageReference)
        {
            if (Context.Guild != null)
                return await (await GetResponseChannel()).SendMessageAsync(message, isTTS, embed, options, allowedMentions, messageReference).ConfigureAwait(false);
            else
                return await Context.User.SendMessageAsync(message, isTTS, embed, options, allowedMentions).ConfigureAwait(false);
        }

        protected async Task<IUserMessage> ReplyAsync([Optional]string? message, [Optional]bool isTTS, [Optional]Embed embed, [Optional]RequestOptions options, [Optional]AllowedMentions allowedMentions, [Optional]bool reply)
        {
            if (Context.Guild != null)
                return await (await GetResponseChannel()).SendMessageAsync(message, isTTS, embed, options, allowedMentions, reply ? new MessageReference(Context.Message.Id) : null).ConfigureAwait(false);
            else
                return await Context.User.SendMessageAsync(message, isTTS, embed, options, allowedMentions).ConfigureAwait(false);
        }

        protected async Task<IUserMessage> ReplyFormattedAsync([Optional] string title, [Optional] string description, [Optional] Color color, [Optional] bool data, [Optional] AllowedMentions? allowedMentions, [Optional] bool reply, bool footer = true)
        {

            EmbedBuilder embed = (footer ? this.GetAuthorEmbedBuilder() : new()).WithTitle(title).WithDescription(description).WithColor(color);
            if (data)
                embed.WithCurrentTimestamp();
            if (Context.Guild != null)
                return await (await GetResponseChannel()).SendMessageAsync(null, false, embed.Build(), null, allowedMentions, reply ? new MessageReference(Context.Message.Id) : null).ConfigureAwait(false);
            else
                return await Context.User.SendMessageAsync(null, false, embed.Build()).ConfigureAwait(false);
        }

        protected async Task<IUserMessage> ReplyFormattedAsync(string title, string description, TimeSpan delayedDeleteTime, [Optional] Color color, [Optional] AllowedMentions allowedMentions, [Optional] bool reply, bool footer = true)
        {
            var replyFormattedAsync = ReplyFormattedAsync(title, description, color, false, allowedMentions, reply, footer);
            replyFormattedAsync.DelayedDelete(delayedDeleteTime);
            return await replyFormattedAsync;
        }
    }
}
