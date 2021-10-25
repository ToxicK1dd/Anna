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
using Anna.DataAccess.Models.Watchdog.FileRestriction;
using Anna.DataAccess.Models.Watchdog.Guild;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;
using Anna.Localization.Resources.Watchdog.Restrict;
using Anna.Utility.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace Anna.Commands.Watchdog
{
    /// <summary>
    /// Watchdog file restriction command.
    /// </summary>
    [Group("begræns")]
    [Aliases("restrict")]
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class Restrict : CommandBase
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;

        public Restrict(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
        }


        /// <summary>
        /// Enables or disables file restrictions.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [GroupCommand()]
        [Description("Slår filbegrænsninger til, eller fra på en server.")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task SetAntiBotAsync(CommandContext ctx)
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
                guild.IsFileRestrictionsEnabled = !guild.IsFileRestrictionsEnabled;

                // Save changes
                await unitOfWork.SaveChangesAsync();

                // Add guild to cache
                cache.Set($"{ctx.Guild.Id}", guild, DateTimeOffset.Now.AddHours(2));

                // Create embed
                DiscordEmbedBuilder embed = new();

                // Switch
                switch(guild.IsFileRestrictionsEnabled)
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
        [Description("Begrænser en filtype, så den ikke kan vedhæftes.")]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task RestrictFileAsync(CommandContext ctx, string fileExtension, [RemainingText] string reason = null)
        {
            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            // Removes whitespaces, and takes the the last item in the array
            fileExtension = string.Concat(fileExtension.Where(c => !char.IsWhiteSpace(c))).Split('.').Last();

            // Get file restriction
            FileRestriction restriction = await unitOfWork.FileRestrictionRepository.GetByFileExtentionAndGuildIdAsync(fileExtension, ctx.Guild.Id);

            // Create embed
            DiscordEmbedBuilder embed = new();

            // Null check
            if(restriction is not null)
            {
                // Format translation
                string msg = string.Format(Translations.AlreadyRestricted,
                           $"{fileExtension}");

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
                await ctx.RespondAsync(Translations.Reason);

                // Wait for response and save the response
                userResponse = await ctx.Message.GetNextMessageAsync();

                // Set description to response message
                reason = userResponse.Result.Content;
            }

            // Create new file restriction
            restriction = new()
            {
                RestrictorId = ctx.Member.Id,
                FileExtension = fileExtension,
                GuildId = ctx.Guild.Id,
                Created = DateTimeOffset.Now,
                Reason = reason
            };

            // Add file restriction to db
            await unitOfWork.FileRestrictionRepository.AddAsync(restriction);

            // Save changes
            await unitOfWork.SaveChangesAsync();

            // Remove cached restrictions
            cache.Remove($"{ctx.Guild.Id}-FileRestrictions");

            // Format translation
            string message = string.Format(Translations.Restricted,
                       $"{fileExtension}");

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
        [Description("Fjerner begrænsnigen på en filtype, så den kan vedhæftes igen.")]
        [Model.Attribute.RequireUserPermissions(Permissions.Administrator)]
        public virtual async Task RemoveRestrictedFileAsync(CommandContext ctx, [RemainingText] string fileExtension)
        {
            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            // Removes whitespaces, and takes the the last item in the array
            fileExtension = string.Concat(fileExtension.Where(c => !char.IsWhiteSpace(c))).Split('.').Last();

            // Add file restriction to db
            FileRestriction restriction = await unitOfWork.FileRestrictionRepository.GetByFileExtentionAndGuildIdAsync(fileExtension, ctx.Guild.Id);

            // Null check
            if(restriction is null)
            {
                // Respond
                await ctx.RespondAsync(Translations.NotFound);

                // Prevent the rest of the command from running
                return;
            }

            // Remove restriction
            unitOfWork.FileRestrictionRepository.Remove(restriction);

            // Save changes
            await unitOfWork.SaveChangesAsync();

            // Remove cached restrictions
            cache.Remove($"{ctx.Guild.Id}-FileRestrictions");

            // Format translation
            string message = string.Format(Translations.RestrictionRemoved,
                       $"{fileExtension}");

            // Create embed
            DiscordEmbedBuilder embed = new();
            embed.CreateDefaultWatchdogEmbed(ctx, $"_ _\n**{message}**\n_ _");

            // Respond
            await ctx.RespondAsync(embed: embed);
        }

        /// <summary>
        /// Gets the reason for a file restriction.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("grund")]
        [Aliases("reason")]
        [Cooldown(1, 2, CooldownBucketType.Guild)]
        [Description("Finder grunden til en filtype er begrænset.")]
        [Model.Attribute.RequireUserPermissions(Permissions.SendMessages)]
        public virtual async Task GetRestrictedFileReasonAsync(CommandContext ctx, [RemainingText] string fileExtension)
        {
            // Get guild
            Guild guild = await unitOfWork.GuildRepository.GetByIdAsync(ctx.Guild.Id);

            // Set culture
            Translations.Culture = CultureInfo.GetCultureInfo(guild.Locale);

            // Removes whitespaces, and takes the the last item in the array
            fileExtension = string.Concat(fileExtension.Where(c => !char.IsWhiteSpace(c))).Split('.').Last();

            // Add file restriction to db
            FileRestriction restriction = await unitOfWork.FileRestrictionRepository.GetByFileExtentionAndGuildIdAsync(fileExtension, ctx.Guild.Id);

            // Null check
            if(restriction is null)
            {
                // Respond
                await ctx.RespondAsync(Translations.NotFound);

                // Prevent the rest of the command from running
                return;
            }

            // Format translation
            string message = string.Format(Translations.EmbedDescription,
                       $"{ctx.Member.Id}",
                       $"{fileExtension}");

            // Create embed
            DiscordEmbedBuilder embed = new();
            embed.CreateDefaultWatchdogEmbed(ctx, null);
            embed.AddField(Translations.EmbedReason, $"{restriction.Reason}", false);
            embed.WithDescription(message);

            // Respond
            await ctx.RespondAsync(embed: embed);
        }
    }
}