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
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Anna.Commands.Base;
using Anna.DataAccess.Models.Watchdog.Guild;
using Anna.DataAccess.Models.Watchdog.Report;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;
using Anna.Localization.Resources.Watchdog.Rapporter;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog report command.
    /// </summary>
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class Rapporter : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public Rapporter(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Adds a report to the watchdog system.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="member"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Command("rapporter")]
        [Aliases("report")]
        [Description("Laver en rapport på en person i Watchdog-systemet.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task CreateReportAsync(CommandContext ctx, DiscordMember member, [RemainingText] string description = null)
        {
            try
            {
                // Get the guild
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

                // Create Interactivity object
                InteractivityResult<DiscordMessage> userResponse = new();

                // Null check
                if(description is null)
                {
                    // Send message to user, and wait for response
                    await ctx.RespondAsync(Translations.ReportReason);

                    // Wait for response and save the response
                    userResponse = await ctx.Message.GetNextMessageAsync();

                    // Set description to response message
                    description = userResponse.Result.Content;
                }

                // Create new database model
                Report report = new()
                {
                    Description = description,
                    ReportedId = member.Id,
                    ReporterId = ctx.Member.Id,
                    GuildId = guild.Id,
                    TimeOfReport = DateTime.Now
                };

                // Add report to the database
                await unitOfWork.ReportRepository.AddAsync(report);

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // TODO: Add logic that sends an embed on the support server.
                // Get watchdog logging channel
                DiscordChannel watchdogChannel = await ctx.Client.GetChannelAsync(728260699268186162);

                // Format translation
                string title = string.Format(Translations.EmbedTitle,
                           $"{ctx.Guild.Name}");

                // Format translation
                string footer = string.Format(Translations.EmbedFooter,
                           $"{ctx.Member.Nickname ?? ctx.Member.DisplayName}",
                           $"{ctx.Member.Id}");

                // Create embed
                DiscordEmbedBuilder embed = new();
                embed.WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
                    .WithTitle(title)
                    .WithThumbnail("https://i.imgur.com/jSML0Zl.png")
                    .WithColor(new(104, 194, 175))
                    .WithFooter(footer, ctx.Member.AvatarUrl)
                    .AddField($"{Translations.EmbedUserId}", $"{ctx.Member.Id}", false)
                    .AddField($"{Translations.EmbedDescription}", description, false);

                // Respond with the created embed.
                await watchdogChannel.SendMessageAsync(embed: embed);

                // Get emoji
                DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");

                // Respond
                if(userResponse.Result is null)
                {
                    await ctx.Message.CreateReactionAsync(emoji);
                }
                else
                {
                    await userResponse.Result.CreateReactionAsync(emoji);
                }
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(ctx, ex);
            }
        }
    }
}