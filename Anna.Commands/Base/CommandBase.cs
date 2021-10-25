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

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Base
{
    /// <summary>
    /// Implements <see cref="BaseCommandModule"/> and <see cref="Config"/> for easy use in command classes.
    /// </summary>
    public class CommandBase : BaseCommandModule
    {
        #region HandleExceptionAsync
        /// <summary>
        /// Creates and responds with an embed containing exception data.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected static async Task HandleExceptionAsync(CommandContext ctx, Exception ex)
        {
            try
            {
                // Create Embed.
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = "https://cdn.discordapp.com/emojis/772175454731632660.gif",
                        Name = "Der opstod en fejl."
                    },
                    Description = $"**Stacktrace**\n```{ex.StackTrace}```",
                    Color = DiscordColor.Green
                };

                // Add fields with useful data.
                embed.AddField("Message", ex.InnerException.Message ?? ex.Message, false);
                embed.AddField("Exception", ex.InnerException.GetType().ToString() ?? ex.GetType().ToString(), false);

                // Get watchdog logging channel
                DiscordChannel AnnaLogChannel = await ctx.Client.GetChannelAsync(721672742272761908);

                // Respond with the created embed.
                await AnnaLogChannel.SendMessageAsync(embed: embed);
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion
    }
}