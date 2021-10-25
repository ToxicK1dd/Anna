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
using Anna.Localization.Resources.Watchdog.Watchdog;
using Anna.Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog Server Protection Command.
    /// </summary>
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class Watchdog : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public Watchdog(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Enables, or disables Watchdog on a Guild.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("vagthund")]
        [Aliases("watchdog")]
        [Description("Slår Watchdog-beskyttelse til, eller fra.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task WatchdogAsync(CommandContext ctx)
        {
            try
            {
                // Try and get guild to see if its already added to the db
                Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

                // Create embed object
                DiscordEmbedBuilder embed = new();

                // Null check
                if(guild is not null)
                {
                    // Set culture
                    Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

                    // Enable or disable watchdog
                    guild.IsWatchdogEnabled = !guild.IsWatchdogEnabled;

                    // Save changes
                    await unitOfWork.SaveChangesAsync();

                    // Add guild to cache
                    cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                    // Switch
                    switch (guild.IsWatchdogEnabled)
                    {
                        case true:
                            embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.Activated}**\n_ _");
                            break;
                        case false:
                            embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.Deactivated}**\n_ _");
                            embed.WithColor(new(194, 104, 104));
                            break;
                    };

                    // Respond with embed
                    await ctx.RespondAsync(embed);

                    // Stop the rest of the command from being executed.
                    return;
                }

                // Get the guild
                DiscordGuild discordGuild = ctx.Guild;

                // Create new database object
                guild = new()
                {
                    Id = discordGuild.Id,
                    IsWatchdogEnabled = true,
                    Locale = "da-DK"
                };


                // Add object to the database
                await unitOfWork.GuildRepository.AddAsync(guild);

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Set culture
                Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

                // Create embed
                embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{Translations.Activated}**\n_ _");

                // Respond with embed
                await ctx.RespondAsync(embed);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(ctx, ex);
            }
        }
    }
}