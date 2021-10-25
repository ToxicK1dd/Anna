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

using System.Collections.Generic;

namespace Anna.DataAccess.Models.Watchdog.Guild
{
    public class Guild
    {
        /// <summary>
        /// The ID of the guild.
        /// </summary>
        public virtual ulong Id { get; set; }

        /// <summary>
        /// Guild language region. (Defaults to da-DK)
        /// </summary>
        public virtual string Locale { get; set; }

        /// <summary>
        /// The ID of the logging channel.
        /// </summary>
        public virtual ulong LoggingChannel { get; set; }

        /// <summary>
        /// Determines if Watchdog is enabled.
        /// </summary>
        public virtual bool? IsWatchdogEnabled { get; set; }

        /// <summary>
        /// Determines if Anti-Bot is enabled.
        /// </summary>
        public virtual bool? IsAntiBotEnabled { get; set; }

        /// <summary>
        /// Determines if Invite-Killer is enabled.
        /// </summary>
        public virtual bool? IsInviteKillerEnabled { get; set; }

        /// <summary>
        /// Determines if Invite-Blocker is enabled.
        /// </summary>
        public virtual bool? IsInviteBlockerEnabled { get; set; }

        /// <summary>
        /// Determines if AutoKick is enabled.
        /// </summary>
        public virtual bool? IsAutoKickEnabled { get; set; }

        /// <summary>
        /// Dertermines if File Restrictions is enabled.
        /// </summary>
        public virtual bool? IsFileRestrictionsEnabled { get; set; }

        /// <summary>
        /// Dertermines if Banned Links is enabled.
        /// </summary>
        public virtual bool? IsBannedLinksEnabled { get; set; }

        /// <summary>
        /// Minimum age for a user to trigger Auto-Kick
        /// </summary>
        public virtual byte MinimumAutoKickAge { get; set; }

        // Reports
        public virtual ICollection<Report.Report> Reports { get; set; }

        // File Restrictions
        public virtual ICollection<FileRestriction.FileRestriction> FileRestrictions { get; set; }

        // Banned links
        public virtual ICollection<LinkBan.LinkBan> BannedLinks { get; set; }
    }
}