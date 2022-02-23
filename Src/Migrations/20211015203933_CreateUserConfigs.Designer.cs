/*
 * Arquivo: 20211015203933_CreateUserConfigs.Designer.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using System;
using BOTDiscord.DataBase.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BOTDiscord_RP.Migrations
{
    [DbContext(typeof(SDBContext))]
    [Migration("20211015203933_CreateUserConfigs")]
    partial class CreateUserConfigs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("BOTDiscord.DataBase.SQLite.Tables.GuildConfig", b =>
                {
                    b.Property<ulong>("GuildID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GuildLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastCommand")
                        .HasColumnType("TEXT");

                    b.Property<char>("Prefix")
                        .HasColumnType("TEXT");

                    b.HasKey("GuildID");

                    b.ToTable("Guilds_Configs");
                });

            modelBuilder.Entity("BOTDiscord.DataBase.SQLite.Tables.SystemParams", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("System_Params");
                });

            modelBuilder.Entity("BOTDiscord.DataBase.SQLite.Tables.UserConfig", b =>
                {
                    b.Property<ulong>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("UserID");

                    b.ToTable("Users_Configs");
                });
#pragma warning restore 612, 618
        }
    }
}
