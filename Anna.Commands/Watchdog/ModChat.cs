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
using DSharpPlus;
using Anna.Commands.Base;
using Anna.DataAccess.Models.Watchdog.Guild;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;
using Anna.Localization.Resources.Watchdog.ModChat;
using Anna.Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog Moderator log chat command.
    /// </summary>
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class ModChat : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public ModChat(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Sets the logging channel for watchdog.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("modchat")]
        [Description("Indstiller en moderator chat til Watchdog beskeder.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task SetModChatAsync(CommandContext ctx, DiscordChannel channel)
        {
            try
            {
                // Get guild
                Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

                // Null check
                if(guild is null)
                {
                    // Setup Watchdog
                    await new Watchdog(unitOfWork, cache).WatchdogAsync(ctx);

                    // Get the guild
                    guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);
                }

                // Set culture
                Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

                // Set mod channel
                guild.LoggingChannel = channel.Id;

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Create embed
                DiscordEmbedBuilder embed = new();
                embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.ModChatSetup}**\n_ _");

                // Send embed
                await ctx.RespondAsync(embed: embed);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(ctx, ex);
            }
        }
    }
}