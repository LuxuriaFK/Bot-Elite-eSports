/*
 * Arquivo: SayEmbedCommand.cs
 * Criado em: 4-11-2021
 * https://github.com/ForceFK
 * Última modificação: 4-11-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BOTDiscord.Commands.Moderation
{
    [Grouping("moderation")]
    [RequireOwner(ErrorMessage = "RequireOwner")]
    [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "RequiredUserManageMessages")]
    public class SayEmbedCommand : AdvancedModuleBase
    {
        [Command("sayeb")]
        [Summary("GMSayEmbed0")]
        public async Task CMD([Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message);
        
        [Command("sayeb")]
        [Summary("GMSayEmbed0")]
        public async Task CMD([Summary("SayEmbed0_1s")] SocketTextChannel Channel, [Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, Channel);

        [Command("sayebred")]
        [Summary("GMSayEmbedRed0")]
        public async Task CMDR([Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, color: Color.Red);

        [Command("sayebred")]
        [Summary("GMSayEmbedRed0")]
        public async Task CMDR([Summary("SayEmbed0_1s")] SocketTextChannel Channel, [Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, Channel, Color.Red);

        [Command("sayebgold")]
        [Summary("GMSayEmbedGold0")]
        public async Task CMDGo([Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, color: Color.Gold);

        [Command("sayebgold")]
        [Summary("GMSayEmbedGold0")]
        public async Task CMDGo([Summary("SayEmbed0_1s")] SocketTextChannel Channel, [Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, Channel, Color.Gold);

        [Command("sayebgreen")]
        [Summary("GMSayEmbedGreen0")]
        public async Task CMDG([Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, color: Color.Green);

        [Command("sayebgreen")]
        [Summary("GMSayEmbedGreen0")]
        public async Task CMDG([Summary("SayEmbed0_1s")] SocketTextChannel Channel, [Summary("SayEmbed0_0s")][Remainder] string Message) => await Run(Message, Channel, Color.Green);
        
        [Command("sayebjson")]
        [Summary("GMSayEmbed1")]
        public async Task CMDJ([Summary("SayEmbed1_0s")][Remainder] string Json) => await RunJson(Json);

        [Command("sayebjson")]
        [Summary("GMSayEmbed1")]
        public async Task CMDJ([Summary("SayEmbed0_1s")] SocketTextChannel Channel, [Summary("SayEmbed1_0s")][Remainder] string Json) => await RunJson(Json, Channel);

        private async Task Run(string msg, [Optional] SocketTextChannel Channel, [Optional] Color color)
        {
            EmbedBuilder eb;
            if (msg.Contains("|"))
            {
                string titulo = msg.Split('|')[0];
                string desc = msg[(titulo.Length + 1)..];

                eb = new EmbedBuilder
                {
                    Title = titulo,
                    Description = desc
                };
            }
            else
                eb = new EmbedBuilder
                {
                    Description = msg.Replace("\\n", "\n")
                };

            if (eb != null)
            {
                CheckMsg(msg, eb);
                eb.WithColor(color == default ? RoleColor() : color);
                if (Channel is null || Context.Channel.Id == Channel.Id)
                {
                    await Context.Channel.TriggerTypingAsync();
                    await ReplyAsync(embed: eb.Build());
                    Context.Message.SafeDelete();
                }
                else
                {
                    if (!Context.Guild.CurrentUser.PermSendMessage(Channel))
                        await ReplyAsync(Loc.Get("Commands.NoPermWriteChannel", Context.User.Mention, Channel.Mention), reply: true);
                    else
                    {
                        await Channel.TriggerTypingAsync();
                        await Channel.SendMessageAsync(embed: eb.Build());
                        await ReplyAsync(Loc.Get("Commands.MessageSendToChat", Context.User.Mention, Channel.Mention), reply: true);
                    }
                }
            }
            else
                await ReplyAsync("Algo fora do comum aconteceu...", reply: true);
        }
        
        private async Task RunJson(string msg, [Optional] SocketTextChannel Channel)
        {
            SayEBJson msgembed = JsonConvert.DeserializeObject<SayEBJson>(msg);
            if (msgembed.embed != null)
            {
                EmbedJson e = msgembed.embed;
                EmbedBuilder eb = new()
                {
                    Title = e.title,
                    Description = e.description,
                    ImageUrl = e.image?.url,
                    Color = new Color(e.color ?? 0),
                    Author = new() { Name = e.author?.name, IconUrl = e.author?.icon_url, Url = e.author?.url },
                    Footer = new() { IconUrl = e.footer?.icon_url, Text = e.footer?.text },
                    ThumbnailUrl = e.thumbnail?.url

                };
                if (e.fields != null && e.fields.Any())
                    eb.WithFields(e.fields.Select(x => new EmbedFieldBuilder { Name = x.name, Value = x.value, IsInline = x.inline }).ToArray());
                    
                CheckMsg(msg, eb);

                if (Channel is null || Context.Channel.Id == Channel.Id)
                {
                    await Context.Channel.TriggerTypingAsync();
                    await ReplyAsync(msgembed.content, embed: eb.Build());
                    Context.Message.SafeDelete();
                }
                else
                {
                    if (!Context.Guild.CurrentUser.PermSendMessage(Channel))
                        await ReplyAsync(Loc.Get("Commands.NoPermWriteChannel", Context.User.Mention, Channel.Mention), reply: true);
                    else
                    {
                        await Channel.TriggerTypingAsync();
                        await Channel.SendMessageAsync(embed: eb.Build());
                        await ReplyAsync(Loc.Get("Commands.MessageSendToChat", Context.User.Mention, Channel.Mention), reply: true);
                    }
                }
            }
            else
                await ReplyAsync("Algo fora do comum aconteceu...", reply: true);
        }
        
        private async void CheckMsg(string msg, EmbedBuilder eb)
        {
            if (!await Context.IsBotOwnerAsync() && (Regex.IsMatch(msg, @"(^\d{3}\.\d{3}\.\d{3}\-\d{2}$)|(^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$)") || Regex.IsMatch(msg, @"^\d{5}-\d{3}$")))
                eb.WithFooter(Loc.Get("Commands.SayMsgBy", Context.User.Mention), Context.User.GetAvatarUrl());
        }

        private Color RoleColor()
        {
            SocketRole role = Context.Guild.CurrentUser.Roles.Where(x => x.Color != default).OrderByDescending(x => x.Position).FirstOrDefault()!;
            if (role != null)
                return role.Color;
            return default;
        }

        private class SayEBJson
        {
            public string? content { get; set; }
            public EmbedJson? embed  { get; set; }
        }

        private class EmbedJson
        {
            public string? title { get; set; }
            public string? description { get; set; }
            public uint? color { get; set; }
            public ImageJson? image { get; set; }
            public ThumbnailJson? thumbnail { get; set; }
            public FooterJson? footer { get; set; }
            public AuthorJson? author { get; set; }
            public FieldsJson[]? fields { get; set; }
        }

        private class ImageJson
        {
            public string? url { get; set; }
        }

        private class ThumbnailJson
        {
            public string? url { get; set; }
        }

        private class FooterJson
        {
            public string? text { get; set; }
            public string? icon_url { get; set; }
        }

        private class AuthorJson
        {
            public string? name { get; set; }
            public string? url { get; set; }
            public string? icon_url { get; set; }
        }

        private class FieldsJson
        {
            public string? name { get; set; }
            public string? value { get; set; }
            public bool inline { get; set; }
        }

    }
}
