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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anna.DataAccess.Models.Watchdog.Guild
{
    /// <summary>
    /// Configuration for the <see cref="Guild"/> type.
    /// </summary>
    public class GuildEntityTypeConfiguration : IEntityTypeConfiguration<Guild>
    {
        public void Configure(EntityTypeBuilder<Guild> builder)
        {
            // Configure primary key, and index
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id)
                .IsUnique();

            // Configure relations
            builder.HasMany(x => x.Reports)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.FileRestrictions)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.BannedLinks)
                .WithOne(x => x.Guild)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure other properties
            builder.Property(x => x.LoggingChannel)
                .IsRequired();
            builder.Property(x => x.Locale)
                .HasDefaultValue("da-DK")
                .IsRequired();
            builder.Property(x => x.IsWatchdogEnabled)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(x => x.IsAntiBotEnabled)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.IsInviteKillerEnabled)
                .IsRequired()
                .HasDefaultValue(true);
                builder.Property(x => x.IsInviteBlockerEnabled)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.IsAutoKickEnabled)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.IsFileRestrictionsEnabled)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.IsBannedLinksEnabled)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.MinimumAutoKickAge)
                .IsRequired()
                .HasDefaultValue(7);
        }
    }
}