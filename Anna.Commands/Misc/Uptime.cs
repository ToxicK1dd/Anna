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
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Misc
{
    public class Uptime : CommandBase
    {
        [Hidden()]
        [Command("uptime")]
        [Description("How long has the bot been running?")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public virtual async Task GetUptimeAsync(CommandContext ctx)
        {
            try
            {
                // Gets the process start time
                DateTime processStartTime = Process.GetCurrentProcess().StartTime;

                // Calculate the total uptime
                TimeSpan processUptime = DateTime.Now - processStartTime;

                // Generate the process uptime strings to show end users
                string days = (processUptime.Days >= 1 ? $"{processUptime.Days} dag{(processUptime.Days > 1 ? "e" : "")}, " : "");
                string hours = (processUptime.Hours >= 1 ? $"{processUptime.Hours} time{(processUptime.Hours > 1 ? "r" : "")}, " : "");
                string minutes = (processUptime.Minutes >= 1 ? $"{processUptime.Minutes} minut{(processUptime.Minutes > 1 ? "ter" : "")}, og " : "");
                string secounds = (processUptime.Seconds >= 1 ? $"{processUptime.Seconds} sekund{(processUptime.Seconds > 1 ? "er" : "")}" : "");

                // Respond to Discord server
                await ctx.RespondAsync(embed: new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Green,
                    Description = $"Jeg har på nuværende tidspunkt kørt i {days}{hours}{minutes}{secounds}.",
                });
            }
            catch (Exception ex)
            {
                // Handle exception.
                await HandleExceptionAsync(ctx, ex);

                // Rethrow for use elsewhere.
                throw;
            }
        }
    }
}