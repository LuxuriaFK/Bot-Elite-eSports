/*
 * Arquivo: 20210610022605_InitialGuildConfig.cs
 * Criado em: 9-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BOTDiscord_RP.Migrations
{
    public partial class InitialGuildConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds_Configs",
                columns: table => new
                {
                    GuildID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Prefix = table.Column<char>(type: "TEXT", nullable: false, defaultValue:'!'),
                    JoinDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastCommand = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds_Configs", x => x.GuildID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds_Configs");
        }
    }
}
