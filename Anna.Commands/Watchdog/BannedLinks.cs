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
using Anna.DataAccess.Models.Watchdog.LinkBan;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;
using Anna.Localization.Resources.Watchdog.BannedLinks;
using Anna.Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog banned links command
    /// </summary>
    [ModuleLifespan(ModuleLifespan.Transient)]
    [Group("linkforbud")]
    [Aliases("linkban")]
    public class BannedLinks : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public BannedLinks(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Enables or disables banned links.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [GroupCommand]
        [Description("Slår link-forbud til, eller fra på en server.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task SetLinkBansAsync(CommandContext ctx)
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
                guild.IsBannedLinksEnabled = !guild.IsBannedLinksEnabled;

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Create embed
                DiscordEmbedBuilder embed = new();

                // Switch
                switch(guild.IsBannedLinksEnabled)
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
        /// Restricts a file type.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("tilføj")]
        [Aliases("add")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Description("Begrænser et link, så det ikke kan sendes.")]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task BanLinkAsync(CommandContext ctx, string link, [RemainingText] string reason = null)
        {
            // Trim whitespace
            link = string.Concat(link.Where(c => !char.IsWhiteSpace(c)));

            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            try
            {
                // Remove all but the link itselt
                Uri myUri = new Uri(link);
                link = myUri.Host;
            }
            catch(Exception)
            {
                await ctx.RespondAsync(Translations.WrongLinkFormat);
                return;
            }

            // Get file restriction
            LinkBan linkBan = await unitOfWork.LinkBanRepository.GetByLinkAndGuildIdAsync(link, ctx.Guild.Id);

            // Create embed
            DiscordEmbedBuilder embed = new();

            // Null check
            if(linkBan is not null)
            {
                // Format translation
                string msg = string.Format(Translations.LinkAlreadyBanned,
                           $"{link}");

                // Add default properties
                embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{msg}**\n_ _");

                // Respond
                await ctx.RespondAsync(embed: embed);

                return;
            }

            // Create Interactivity object
            InteractivityResult<DiscordMessage> userResponse = new();

            // Check
            if(reason is null)
            {
                // Send message to user, and wait for response
                await ctx.RespondAsync(Translations.LinkBanReason);

                // Wait for response and save the response
                userResponse = await ctx.Message.GetNextMessageAsync();

                // Set description to response message
                reason = userResponse.Result.Content;
            }

            // Create new file restriction
            linkBan = new()
            {
                CreatedBy = ctx.Member.Id,
                Url = link,
                GuildId = ctx.Guild.Id,
                Created = DateTimeOffset.Now,
                Reason = reason
            };

            // Add file restriction to db
            await unitOfWork.LinkBanRepository.AddAsync(linkBan);

            // Save changes
            await unitOfWork.SaveChangesAsync();

            // Remove cached link bans 
            cache.Remove($"{ctx.Guild.Id}-BannedLinks");

            // Format translation
            string message = string.Format(Translations.LinkAlreadyBanned,
                       $"{link}");

            // Create embed
            embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{message}**\n_ _");

            // Respond
            await ctx.RespondAsync(embed: embed);
        }

        /// <summary>
        /// Restricts a file type.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("fjern")]
        [Aliases("remove")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Description("Fjerner et forbud på et link, så det kan sendes igen.")]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task RemoveBannedLinkAsync(CommandContext ctx, [RemainingText] string link)
        {
            // Trim whitespace
            link = string.Concat(link.Where(c => !char.IsWhiteSpace(c)));

            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            try
            {
                // Remove all but the link itselt
                Uri myUri = new Uri(link);
                link = myUri.Host;
            }
            catch(Exception)
            {
                await ctx.RespondAsync(Translations.WrongLinkFormat);
                return;
            }

            // Get file restriction
            LinkBan linkBan = await unitOfWork.LinkBanRepository.GetByLinkAndGuildIdAsync(link, ctx.Guild.Id);

            // Null check
            if(linkBan is null)
            {
                // Respond
                await ctx.RespondAsync(Translations.NotFound);

                // Prevent the rest of the command from running
                return;
            }

            // Remove restriction
            unitOfWork.LinkBanRepository.Remove(linkBan);

            // Save changes
            await unitOfWork.SaveChangesAsync();

            // Remove cached link bans 
            cache.Remove($"{ctx.Guild.Id}-BannedLinks");

            // Format translation
            string msg = string.Format(Translations.LinkBanRemoved,
                       $"{linkBan.Url}");

            // Create embed
            DiscordEmbedBuilder embed = new();
            embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{msg}**\n_ _");

            // Respond
            await ctx.RespondAsync(embed: embed);
        }

        /// <summary>
        /// Gets the reason for a banned link.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("grund")]
        [Aliases("reason")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Description("Finder grunden til at et link er forbudt.")]
        [Model.Attribute.RequireUserPermissions(Permissions.SendMessages)]
        public virtual async Task GetBannedLinkReasonAsync(CommandContext ctx, [RemainingText] string link)
        {
            // Trim whitespace
            link = string.Concat(link.Where(c => !char.IsWhiteSpace(c)));

            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            try
            {
                // Remove all but the link itselt
                Uri myUri = new Uri(link);
                link = myUri.Host;
            }
            catch(Exception)
            {
                await ctx.RespondAsync(Translations.WrongLinkFormat);
                return;
            }

            // Add file restriction to db
            LinkBan linkBan = await unitOfWork.LinkBanRepository.GetByLinkAndGuildIdAsync(link, ctx.Guild.Id);

            // Null check
            if(linkBan is null)
            {
                // Respond
                await ctx.RespondAsync(Translations.NotFound);

                // Prevent the rest of the command from running
                return;
            }

            // Format translation
            string msg = string.Format(Translations.Description,
                       $"{linkBan.CreatedBy}",
                       $"{linkBan.Url}");

            // Create embed
            DiscordEmbedBuilder embed = new();
            embed.CreateDefaultWatchdogEmbed(ctx, null);
            embed.AddField($"{Translations.Reason}", $"{linkBan.Reason}", false);
            embed.WithDescription(msg);

            // Respond
            await ctx.RespondAsync(embed: embed);
        }
    }
}