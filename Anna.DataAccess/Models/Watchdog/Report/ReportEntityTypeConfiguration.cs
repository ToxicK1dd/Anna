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

namespace Anna.DataAccess.Models.Watchdog.Report
{
    /// <summary>
    /// Configuration for the <see cref="Report"/> type.
    /// </summary>
    public class ReportEntityTypeConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            // Configure primary key, and index
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id)
                .IsUnique();

            // Configure auto increment
            builder.Property(x => x.Id)
                .UseMySqlIdentityColumn();

            // Configure relations
            builder.HasOne(x => x.Guild)
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure other properties
            builder.Property(x => x.ReportedId)
                .IsRequired();
            builder.Property(x => x.ReporterId)
                .IsRequired();
            builder.Property(x => x.Description)
                .IsRequired();
            builder.Property(x => x.TimeOfReport)
                .IsRequired();
        }
    }
}