﻿// <auto-generated />
using System;
using Anna.DataAccess.Models.Watchdog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Anna.DataAccess.Migrations.Watchdog
{
    [DbContext(typeof(WatchdogContext))]
    [Migration("20210801122801_AddInviteBlocker")]
    partial class AddInviteBlocker
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.FileRestriction.FileRestriction", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("RestrictorId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("FileRestriction");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.Guild.Guild", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<bool?>("IsAntiBotEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsAutoKickEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsBannedLinksEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsFileRestrictionsEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsInviteBlockerEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool?>("IsInviteKillerEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsWatchdogEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Locale")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("longtext")
                        .HasDefaultValue("da-DK");

                    b.Property<ulong>("LoggingChannel")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte>("MinimumAutoKickAge")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint unsigned")
                        .HasDefaultValue((byte)7);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Guild");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.LinkBan.LinkBan", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("CreatedBy")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("LinkBan");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.Report.Report", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ReportedId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ReporterId")
                        .HasColumnType("bigint unsigned");

                    b.Property<DateTimeOffset>("TimeOfReport")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Report");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.FileRestriction.FileRestriction", b =>
                {
                    b.HasOne("Anna.DataAccess.Models.Watchdog.Guild.Guild", "Guild")
                        .WithMany("FileRestrictions")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.LinkBan.LinkBan", b =>
                {
                    b.HasOne("Anna.DataAccess.Models.Watchdog.Guild.Guild", "Guild")
                        .WithMany("BannedLinks")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.Report.Report", b =>
                {
                    b.HasOne("Anna.DataAccess.Models.Watchdog.Guild.Guild", "Guild")
                        .WithMany("Reports")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Anna.DataAccess.Models.Watchdog.Guild.Guild", b =>
                {
                    b.Navigation("BannedLinks");

                    b.Navigation("FileRestrictions");

                    b.Navigation("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
