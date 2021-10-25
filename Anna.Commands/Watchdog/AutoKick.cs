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
using Anna.Localization.Resources.Watchdog.AutoKick;
using Anna.Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog Auto Kick command.
    /// </summary>
    [Group("autokick")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class AutoKick : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public AutoKick(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Enables or disables anti bot.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [GroupCommand()]
        [Description("Slår Auto Kick til, eller fra på en server.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task SetAutoKickAsync(CommandContext ctx)
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

                // Change enabled
                guild.IsAutoKickEnabled = !guild.IsAutoKickEnabled;

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Create embed
                DiscordEmbedBuilder embed = new();

                // Switch
                switch(guild.IsAutoKickEnabled)
                {
                    case true:
                        embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.Activated}**\n_ _");
                        break;
                    case false:
                        embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.Deactivated}**\n_ _");
                        embed.WithColor(new(194, 104, 104));
                        break;
                };

                // Send embed
                await ctx.RespondAsync(embed: embed);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(ctx, ex);
            }
        }

        /// <summary>
        /// Sets autokick minimum age
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("alder")]
        [Aliases("age")]
        [Description("Sætter minimumsalderen på autokick.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task SetAutoKickAgeAsync(CommandContext ctx, byte age)
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

                // Change enabled
                guild.MinimumAutoKickAge = age;

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Format translation
                string msg = string.Format(Translations.MinimumAge,
                           $"{age}");

                // Create embed 
                DiscordEmbedBuilder embed = new();
                embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{msg}**\n_ _");

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