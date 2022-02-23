/*
 * Arquivo: ColorCommand.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 19-10-2021
 */
using BOTDiscord.Attributes;
using BOTDiscord.Extensions;
using BOTDiscord.Manager.Commands.Modules;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Commands.Utils
{
    [Grouping("utils")]
    //[IsDisable]
    public class ColorCommand : AdvancedModuleBase
    {
        [ThreadStatic]
        private static Random? _rng;
        private static Random RngInstance => _rng ??= new Random();

        [Command("colorinfo")]
        [Alias("color", "cor")]
        [Summary("Color0")]
        public async Task CMD([Summary("Color0_0s")][Optional] string color) => await Run(color);

        public async Task Run(string color)
        {
            var builder = new EmbedBuilder();
            System.Drawing.Color argbColor;
            if (string.IsNullOrWhiteSpace(color))
            {
                argbColor = System.Drawing.Color.FromArgb(RngInstance.Next(0, 256), RngInstance.Next(0, 256), RngInstance.Next(0, 256));
                builder.WithFooter(Loc.Get("Commands.RandomColor"));
            }
            else
            {
                color = color.TrimStart('#');
                if (!int.TryParse(color, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rawColor)
                    && !int.TryParse(color, NumberStyles.Integer, CultureInfo.InvariantCulture, out rawColor))
                {
                    rawColor = System.Drawing.Color.FromName(color).ToArgb();
                    if (rawColor == 0)
                    {
                        rawColor = color.ToColor();
                    }
                }
                argbColor = System.Drawing.Color.FromArgb(rawColor);
            }

            using var bmp = new Bitmap(260, 40);
            using var g = Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.FromArgb(argbColor.R, argbColor.G, argbColor.B));

            await using Stream stream = new MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            string hex = $"{argbColor.R:X2}{argbColor.G:X2}{argbColor.B:X2}";

            builder.AddField("RGB", $"``{argbColor.R}, {argbColor.G}, {argbColor.B}``", true)
                .AddField("Hexadecimal", $"``#{hex}``", true)
                .AddField("Decimal", $"``{argbColor.ToArgb()}``", true)
                //.WithTitle($"🎨 {argbColor.Name}") //Nao pega em cores rgb
                .WithImageUrl($"attachment://{hex}.png")
                .WithColor(new Discord.Color(argbColor.R, argbColor.G, argbColor.B));

            await Context.Channel.SendFileAsync(stream, $"{hex}.png", embed: builder.Build(), messageReference: new MessageReference(Context.Message.Id));
        }

    }
}
