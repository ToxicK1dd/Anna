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

using System;

namespace Anna.DataAccess.Models.Watchdog.Report
{
    public class Report
    {
        /// <summary>
        /// The id and primary key of the <see cref="Report"/>.
        /// </summary>
        public virtual ulong Id { get; set; }

        /// <summary>
        /// The ID of the reported person.
        /// </summary>
        public virtual ulong ReportedId { get; set; }

        /// <summary>
        /// The ID of the reporter.
        /// </summary>
        public virtual ulong ReporterId { get; set; }

        /// <summary>
        /// The ID of the guild in which the person got reported.
        /// This is also the foreign key to the <see cref="Guild"/> property.
        /// </summary>
        public virtual ulong GuildId { get; set; }

        /// <summary>
        /// The description from the reporter of what the reported has done.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The time of which the report was made.
        /// </summary>
        public virtual DateTimeOffset TimeOfReport { get; set; }

        // Guild Navigation Property
        public virtual Guild.Guild Guild { get; set; }
    }
}