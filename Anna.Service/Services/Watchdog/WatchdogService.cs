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

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Anna.DataAccess.Models.Watchdog.FileRestriction;
using Anna.DataAccess.Models.Watchdog.Guild;
using Anna.DataAccess.Models.Watchdog.LinkBan;
using Anna.DataAccess.UnitOfWork.Derived.Watchdog.Interface;
using Anna.Model.Singleton;
using Anna.Service.Services.Watchdog.Interface;
using Anna.Utility.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace Anna.Service.Services.Watchdog
{
    // We do not await certain methods, we may not need the result for continuing our task,
    // so there is no point in waiting for the methods to finish.
#pragma warning disable CS4014
    public class WatchdogService : IWatchdogService
    {
        private readonly IWatchdogUnitOfWork unitOfWork;
        private readonly IMemoryCache cache;
        private readonly LoggingChannels channels;
        private readonly Regex discordRegex = new Regex(@"discord(?:\.com|app\.com|\.gg)[\/invite\/]?(?:[a-zA-Z0-9\-]{2,32})");

        public WatchdogService(IWatchdogUnitOfWork unitOfWork, IMemoryCache cache, IOptions<LoggingChannels> options)
        {
            this.unitOfWork = unitOfWork;
            this.cache = cache;
            channels = options.Value;
        }


        public virtual async Task CheckAutoKickAsync(DiscordClient client, GuildMemberAddEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and auto kick is enabled
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsAutoKickEnabled is true)
            {
                // Check if member is minimum age
                if ((DateTimeOffset.Now - e.Member.CreationTimestamp).Days < guild.MinimumAutoKickAge)
                {
                    // Kick member
                    e.Member.RemoveAsync($"User was removed by AutoKick, the account was not older than {guild.MinimumAutoKickAge}.");

                    // Send Guild log message
                    SendWatchdogGuildLogMessageAsync(
                        e.Guild, guild,
                        $"{e.Member.DisplayName}#{e.Member.Discriminator} blev smidt ud af AutoKick.",
                        $"{e.Member.DisplayName}#{e.Member.Discriminator} blev fjernet.",
                        e.Member.AvatarUrl);

                    // Send message in watchdog log channel
                    SendWatchdogSupportServerLogMessageAsync(client,
                        WatchdogEmbedHelper.GetAutoKickLogMessage(e.Member, e.Guild,
                            $"Brugeren er under {guild.MinimumAutoKickAge} dage gammel."));
                }
            }
        }

        public virtual async Task CheckInviteKillerAsync(DiscordClient client, GuildMemberRemoveEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and invite killer is enabled
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsInviteKillerEnabled is true)
            {
                // Get all invites (TODO: Find a better way of doing this)
                IReadOnlyList<DiscordInvite> invites = await e.Guild.GetInvitesAsync();

                // Find the members invites
                IEnumerable<DiscordInvite> memberInvites = invites.Where(x => x.Inviter.Id == e.Member.Id);

                // Iterate over the invites
                foreach (DiscordInvite invite in memberInvites)
                {
                    // Delete the invite (TODO: Find a better way of doing this)
                    invite.DeleteAsync();
                }

                // Send Guild log message
                SendWatchdogGuildLogMessageAsync(
                    e.Guild, guild,
                    $"{e.Member.DisplayName}#{e.Member.Discriminator}'s invites blev fjernet af Invite Killer.",
                    $"{e.Member.DisplayName}#{e.Member.Discriminator}'s invites blev slettet.",
                    e.Member.AvatarUrl);
            }
        }

        public virtual async Task CheckInviteBlockerAsync(DiscordClient client, MessageCreateEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and invite blocker is enabled
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsInviteBlockerEnabled is true)
            {
                // Check if message content has any matches.
                if (discordRegex.Matches(e.Message.Content).Count >= 1)
                {
                    e.Message.DeleteAsync();
                    e.Channel.SendMessageAsync($"<@{e.Author.Id}> FY! Du må ikke linke til andre servere!!!");

                    // Send Guild log message
                    SendWatchdogGuildLogMessageAsync(
                        e.Guild, guild,
                        $"{e.Author.Username}#{e.Author.Discriminator}'s invite blev fjernet af Invite Blocker.",
                        $"{e.Author.Username}#{e.Author.Discriminator}'s invite blev slettet.",
                        e.Author.AvatarUrl);
                }
            }
        }

        public virtual async Task CheckAntiBotAsync(DiscordClient client, MessageCreateEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and anti bot is enabled
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsAntiBotEnabled is true)
            {
                // Try and get the last message
                bool lastMessage = cache.TryGetValue($"{e.Author.Id}-message", out DiscordMessage message);

                // If a value was retrieved
                if (lastMessage)
                {
                    // Check if content match
                    if (e.Message.Content == message.Content)
                    {
                        // Try and get the member from cache
                        bool memberRetrieved = cache.TryGetValue($"{e.Author.Id}-member", out DiscordMember member);

                        // If the member was not retrieved
                        if (memberRetrieved is false)
                        {
                            // Get the member who created the message
                            member = await e.Guild.GetMemberAsync(e.Author.Id);

                            // Cache member for 5 minutes
                            cache.Set($"{e.Author.Id}-member", e.Message, e.Message.CreationTimestamp.AddMinutes(5));
                        }

                        // If the member joined less than a week ago
                        if (member is not null && (DateTimeOffset.Now - member.JoinedAt).Days < 7)
                        {
                            // Get permissions
                            bool hasBanPerm = member.PermissionsIn(e.Channel).HasPermission(Permissions.BanMembers);
                            bool hasKickPerm = member.PermissionsIn(e.Channel).HasPermission(Permissions.KickMembers);

                            // Check if the member can ban, or kick, and is not a bot
                            if (hasBanPerm is false || hasKickPerm is false && member.IsBot is false)
                            {
                                // Kick member from the guild
                                member.RemoveAsync("User was kicked by AntiBot for exessive spam.");

                                // React to the message
                                e.Message.CreateReactionAsync(DiscordEmoji.FromName(client, ":boot:"));

                                // Remove member from cache
                                cache.Remove($"{e.Author.Id}-member");

                                // Send Guild log message
                                SendWatchdogGuildLogMessageAsync(
                                    e.Guild, guild,
                                    $"{e.Author.Username}#{e.Author.Discriminator} blev smidt ud af AntiBot for spam.",
                                    $"{e.Author.Username}#{e.Author.Discriminator} forlod serveren.",
                                    e.Author.AvatarUrl);
                            }
                        }
                    }

                    // Cache the latest message for 12 seconds. This saves us a check for when the last message was sent.
                    cache.Set($"{e.Author.Id}-message", e.Message, e.Message.CreationTimestamp.AddSeconds(12));
                }
            }
        }

        public virtual async Task CheckFileRestrictionAsync(DiscordClient client, MessageCreateEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and file restrictions are enabled
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsFileRestrictionsEnabled is true)
            {
                // Get banned links from cache
                ICollection<FileRestriction> fileRestrictions = await GetFileRestrictionsFromCacheAsync(e.Guild.Id);

                // Loop through all attachments
                foreach (DiscordAttachment attachment in e.Message.Attachments)
                {
                    // Check if extention matches any restricted file types
                    if (fileRestrictions.Any(x => x.FileExtension == attachment.FileName.Split('.').Last()))
                    {
                        e.Message.DeleteAsync();

                        DiscordEmbedBuilder embed = new();
                        embed.WithTitle("Mistænkelig fil")
                            .WithDescription($"Brugeren <@{e.Author.Id}>, har lige sendt en mistænkelig fil.\nDette er automatisk blevet slettet")
                            .WithColor(new(104, 194, 175));

                        // Delete, and respondt with a new message
                        e.Message.Channel.SendMessageAsync(embed: embed);

                        // Create embed for the logging channel
                        embed = new();
                        embed.WithAuthor(e.Guild.Name, null, e.Guild.IconUrl)
                            .WithTitle("Mistænkelig fil")
                            .WithDescription($"{e.Author.Username}#{e.Author.Discriminator} har sendt en mistænkelig fil.")
                            .WithColor(new(104, 194, 175))
                            .AddField("Bruger | ID", $"{e.Author.Username}#{e.Author.Discriminator} | {e.Author.Id}")
                            .AddField("Server | ID", $"{e.Guild.Name} | {e.Guild.Id}")
                            .AddField("Fil navn", $"{attachment.FileName}");

                        // Send embed
                        SendWatchdogSupportServerLogMessageAsync(client, embed);

                        // Send embed in the guild log channel
                        // TODO: Make this^
                    }
                }
            }
        }

        public virtual async Task CheckBannedLinksAsync(DiscordClient client, MessageCreateEventArgs e)
        {
            // Try and get the guild from cache
            Guild guild = await GetGuildFromCacheAsync(e.Guild.Id);

            // Check if watchdog, and link bans are enabled.
            if (guild is not null && guild.IsWatchdogEnabled is true && guild.IsBannedLinksEnabled is true)
            {
                // Get banned links from cache
                ICollection<LinkBan> bannedLinks = await GetBannedLinksFromCacheAsync(e.Guild.Id);

                if (bannedLinks is not null)
                {
                    // Loop through all attachments
                    foreach (LinkBan bannedLink in bannedLinks)
                    {
                        // Check if extention matches any restricted file types
                        if (string.Concat(e.Message.Content.Where(c => !char.IsWhiteSpace(c))).Contains(bannedLink.Url))
                        {
                            e.Message.DeleteAsync();

                            DiscordEmbedBuilder embed = new();
                            embed.WithTitle("Forbudt link")
                                .WithDescription($"Brugeren <@{e.Author.Id}>, har lige sendt et forbudt link.\nDette er automatisk blevet slettet")
                                .WithColor(new(104, 194, 175));

                            // Delete, and respondt with a new message
                            e.Message.Channel.SendMessageAsync(embed: embed);

                            // Create embed for the logging channel
                            embed = new();
                            embed.WithAuthor(e.Guild.Name, null, e.Guild.IconUrl)
                                .WithTitle("Forbudt link")
                                .WithDescription($"{e.Author.Username}#{e.Author.Discriminator} har sendt et forbudt link.")
                                .WithColor(new(104, 194, 175))
                                .AddField("Bruger | ID", $"{e.Author.Username}#{e.Author.Discriminator} | {e.Author.Id}")
                                .AddField("Server | ID", $"{e.Guild.Name} | {e.Guild.Id}")
                                .AddField("Link", $"{bannedLink.Url}");

                            // Send embed
                            SendWatchdogSupportServerLogMessageAsync(client, embed);

                            // Send embed in the guild log channel
                            // TODO: Make this^
                        }
                    }
                }
            }
        }


        #region Helpers
        private async Task<Guild> GetGuildFromCacheAsync(ulong guildId)
        {
            // Try and get the guild from cache
            bool guildRetrieved = cache.TryGetValue($"{guildId}", out Guild guild);

            // If the guild was not retrieved
            if (guildRetrieved is false)
            {
                // Get the guild from the database
                guild = await unitOfWork.GuildRepository.GetByIdAsync(guildId);

                // Null check
                if (guild is not null)
                {
                    // Cache the guild
                    cache.Set($"{guildId}", guild, DateTimeOffset.Now.AddHours(2));
                }
            }

            // Return the guild
            return guild;
        }

        private async Task<ICollection<LinkBan>> GetBannedLinksFromCacheAsync(ulong guildId)
        {
            // Try and get the banned links from cache
            bool linksRetrieved = cache.TryGetValue($"{guildId}-BannedLinks", out ICollection<LinkBan> linkBans);

            // If the links was not retrieved
            if (linksRetrieved is false)
            {
                // Get the links from the database
                linkBans = await unitOfWork.LinkBanRepository.GetAllByGuildIdAsync(guildId);

                // Null check
                if (linkBans is not null)
                {
                    // Cache the links
                    cache.Set($"{guildId}-BannedLinks", linkBans, DateTimeOffset.Now.AddHours(2));
                }
            }

            // Return the links
            return linkBans;
        }

        private async Task<ICollection<FileRestriction>> GetFileRestrictionsFromCacheAsync(ulong guildId)
        {
            // Try and get the file restrictions from cache
            bool restrictionsRetrieved = cache.TryGetValue($"{guildId}-FileRestrictions", out ICollection<FileRestriction> fileRestrictions);

            // If the restrictions was not retrieved
            if (restrictionsRetrieved is false)
            {
                // Get the restrictions from the database
                fileRestrictions = await unitOfWork.FileRestrictionRepository.GetAllByGuildIdAsync(guildId);

                // Null check
                if (fileRestrictions is not null)
                {
                    // Cache the guild
                    cache.Set($"{guildId}-FileRestrictions", fileRestrictions, DateTimeOffset.Now.AddHours(2));
                }
            }

            // Return the guild
            return fileRestrictions;
        }

        private async Task SendWatchdogSupportServerLogMessageAsync(DiscordClient client, DiscordEmbedBuilder embed)
        {
            // Get watchdog logging channel
            DiscordChannel watchdogChannel = await client.GetChannelAsync(channels.Watchdog);

            // Send embed
            await watchdogChannel.SendMessageAsync(embed: embed);
        }

        private async Task SendWatchdogGuildLogMessageAsync(DiscordGuild discordGuild, Guild guild, string title, string footer, string url)
        {
            if (guild.LoggingChannel is not 0)
            {
                // Get logging channel
                DiscordChannel channel = discordGuild.GetChannel(guild.LoggingChannel);

                // Create embed
                DiscordEmbedBuilder embed = new();
                embed.WithAuthor(discordGuild.Name, null, discordGuild.IconUrl)
                    .WithTitle(title)
                    .WithThumbnail("https://i.imgur.com/jSML0Zl.png")
                    .WithColor(new(194, 104, 104))
                    .WithFooter(footer, url);

                // Send embed
                await channel.SendMessageAsync(embed: embed);
            }
        }
        #endregion
    }
}