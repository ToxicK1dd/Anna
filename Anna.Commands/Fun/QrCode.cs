// Copyright (c) 2021 ToxicK1dd
// Copyright (C) 2021 Project Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Anna.Commands.Base;
using QRCoder;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Anna.Commands.Fun
{
    /// <summary>
    /// QR code generator command.
    /// </summary>
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class QrCode : CommandBase
    {
        private readonly Stopwatch stopwatch = new();

        public QrCode()
        {
            stopwatch.Start();
        }


        /// <summary>
        /// Generates a QR code from the given input.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [Command("qrcode")]
        [Aliases("qr")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public virtual async Task CreateQrCodeAsync(CommandContext ctx, [RemainingText] string input)
        {
            // Trigger typing in chat
            await ctx.TriggerTypingAsync();

            // Get qr code base64 string
            byte[] base64 = await GetBase64QrCodeImage(input);

            // Create new memory stream
            using MemoryStream ms = new MemoryStream(base64);

            // Generate embed
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "QR Kode Genereret!",
                Color = new DiscordColor(104, 194, 175),
                Description = "Her har du den! :arrow_down:",
                ImageUrl = "attachment://qr.png",
                Footer = new()
                {
                    Text = $"Anmodningen fra {ctx.Member.Nickname ?? ctx.Member.DisplayName} tog {stopwatch.ElapsedMilliseconds}ms.",
                    IconUrl = ctx.Member.AvatarUrl
                }
            };

            // Generate messae
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile("qr.png", ms);
            builder.WithEmbed(embedBuilder.Build());

            // Respond with message
            await ctx.RespondAsync(builder);
        }


        #region Helpers
        private static Task<byte[]> GetBase64QrCodeImage(string content)
        {
            // Create new QR Code generator
            using QRCodeGenerator qrGenerator = new();

            // Generate QR code data from string
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // Convert QR code data to qr code
            using QRCode qrCode = new(qrCodeData);

            // Convert QR code to bitmap
            using Bitmap qrCodeImage = qrCode.GetGraphic(20, 
                Color.Black, 
                Color.FromArgb(47, 49, 54), 
                true);

            // Memory stream for saving the image
            using MemoryStream stream = new();

            // Save the image to the stream
            qrCodeImage.Save(stream, ImageFormat.Jpeg);

            // Return image as byte array
            return Task.FromResult(stream.ToArray());
        }
        #endregion
    }
}