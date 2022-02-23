/*
 * Arquivo: LanguageCommand.cs
 * Criado em: 27-9-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Localization;
using BOTDiscord.Manager.Commands;
using BOTDiscord.Manager.Commands.Modules;
using BOTDiscord.Utilities;
using BOTDiscord.Utilities.Collector;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Settings
{
    [Grouping("botconfig")]
    [RequireUserPermission(GuildPermission.ManageGuild, ErrorMessage = "RequiredUserManageGuild", NotAGuildErrorMessage = "NotAGuildErrorMessage")]
    public class LanguageCommand : AdvancedModuleBase
    {
        private readonly CommandHandler _handler;
        private readonly CollectorsUtils _collectorsUtils;
        private readonly IServiceProvider _service;

        public LanguageCommand(CommandHandler commandhandler, CollectorsUtils collectorsUtils, IServiceProvider service)
        {
            _handler = commandhandler;
            _collectorsUtils = collectorsUtils;
            _service = service;
        }

        [Command("setlanguage")]
        [Alias("setidioma")]
        [Summary("setlanguage0s")]
        //[IsDisable]
        public async Task SetLanguage([Summary("setlanguage0_0s")][Optional] string language)
        {
            if (language == null)
            {
                await ListLanguages();
                return;
            }

            if (LocalizationManager.Languages.ContainsKey(language))
            {
                await GuildConfig.SetLanguageAsync(language);
                Context.Message.SafeDelete();
                await ReplyFormattedAsync(Loc.Get("Commands.Success"), Loc.Get("Language.Success").Format(language), TimeSpan.FromMinutes(1), color: Constants.DiscordBlurple);
            }
            else
            {
                var languagesList = string.Join(" ", LocalizationManager.Languages.Select(pair => $"`{pair.Key}`"));
                await ReplyFormattedAsync(Loc.Get("Commands.Fail"), Loc.Get("Language.Fail").Format(language, languagesList), TimeSpan.FromMinutes(1));
            }
        }

        [Command("language")]
        [Alias("languages", "idiomas")]
        [Summary("language0s")]
        //[IsDisable]
        public async Task ListLanguages()
        {
            var embedBuilder = this.GetAuthorEmbedBuilder().WithColor(Constants.DiscordBlurple).WithTitle(Loc.Get("Language.LanguagesList"));
            foreach (LocalizationPack pack in LocalizationManager.Languages.Values)
            {
                embedBuilder.AddField($"{pack.LocalizationFlagEmoji} **{pack.LocalizedName}** ({pack.LanguageName})",
                    Loc.Get("Language.LanguageDescription").Format(GuildConfig.Prefix, pack.LocalizedName, pack.Authors, pack.TranslationCompleteness), true);
            }

            var message = await ReplyAsync(embed: embedBuilder.Build());
            CollectorsGroup collectors = null!;
            var packsWithEmoji = LocalizationManager.Languages.Where(pair => pair.Value.LocalizationFlagEmoji != null).ToList();
            collectors = new CollectorsGroup(packsWithEmoji.Select(
                pair =>
                {
                    var packName = pair.Key;
                    return _collectorsUtils.CollectReaction(message, reaction => reaction.UserId == Context.Message.Author.Id && reaction.Emote.Equals(pair.Value.LocalizationFlagEmoji), async args =>
                    {
                        var t = await _handler.ExecuteCommand($"setlanguage {packName}", Context, args.Reaction.UserId.ToString(), _service);
                        if (t.IsSuccess)
                        {
                            message.SafeDelete();
                            collectors?.DisposeAll();
                        }
                    }, CollectorFilter.IgnoreBots);
                }));
            try
            {
                await message.AddReactionsAsync(packsWithEmoji.Select(pair => pair.Value.LocalizationFlagEmoji).ToArray());
            }
            catch (Exception)
            {
                // ignored
            }
            Context.Message.SafeDelete();
            collectors.SetTimeoutToAll(Constants.StandardTimeSpan);
            message.DelayedDelete(Constants.StandardTimeSpan);
        }
    }
}
