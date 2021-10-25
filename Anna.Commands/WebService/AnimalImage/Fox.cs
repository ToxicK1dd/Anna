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
using Anna.Utility.Enums;
using Anna.WebService.WebServices.Derived.AnimalImage.Interface;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.WebService.AnimalImage
{
    /// <summary>
    /// Get an image of a fox.
    /// </summary>
    public class Fox : CommandBase
    {
        private readonly IAnimalImageService service;

        public Fox(IAnimalImageService service)
        {
            this.service = service;
        }


        [Hidden()]
        [Command("fox")]
        [Description("Returns a random fox image from the internet!")]
        [Cooldown(1, 15, CooldownBucketType.Guild)]
        public virtual async Task GetFoxImageAsync(CommandContext ctx)
        {
            try
            {
                // Trigger typing
                await ctx.TriggerTypingAsync();

                // Stopwatch for timing, obviously...
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Get link to the image
                string link = await service.GetAnimalImageAsync(AnimalType.Fox);

                // Stop stopwatch!
                stopwatch.Stop();

                // Create an embed for our image
                DiscordEmbedBuilder.EmbedFooter footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = $"{ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}  |  Took {stopwatch.ElapsedMilliseconds}ms.",
                    IconUrl = $"{ctx.Message.Author.AvatarUrl}"
                };
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Title = "***Random fox noises*** :fox:",
                    Color = DiscordColor.Green,
                    ImageUrl = link,
                    Timestamp = DateTime.Now,
                    Footer = footer
                };

                // Respond with the embed
                await ctx.RespondAsync(embed: embed);
            }
            catch(Exception ex)
            {
                // Handle exception.
                await HandleExceptionAsync(ctx, ex);

                // Rethrow for use elsewhere.
                throw;
            }
        }
    }
}