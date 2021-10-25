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

namespace Anna.DataAccess.Models.Watchdog.FileRestriction
{
    public class FileRestriction
    {
        /// <summary>
        /// The id and primary key of the <see cref="FileRestriction"/>.
        /// </summary>
        public virtual ulong Id { get; set; }

        /// <summary>
        /// The id of the person who created the restriction.
        /// </summary>
        public virtual ulong RestrictorId { get; set; }

        /// <summary>
        /// The file extention of the <see cref="FileRestriction"/>.
        /// </summary>
        public virtual string FileExtension { get; set; }

        /// <summary>
        /// The id, and foreign key of the <see cref="Guild"/> for which the <see cref="FileRestriction"/> was created for.
        /// </summary>
        public virtual ulong GuildId { get; set; }

        /// <summary>
        /// The time of the creation.
        /// </summary>
        public virtual DateTimeOffset Created { get; set; }

        /// <summary>
        /// The reason for the restriction of that <see cref="FileExtension"/>.
        /// </summary>
        public virtual string Reason { get; set; }

        // Guild Navigation Property
        public virtual Guild.Guild Guild { get; set; }
    }
}