/*
 * Arquivo: 20210610054242_UpdateGuildConfig.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using Microsoft.EntityFrameworkCore.Migrations;

namespace BOTDiscord_RP.Migrations
{
    public partial class UpdateGuildConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "System_Params",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuildLanguage",
                table: "Guilds_Configs",
                type: "TEXT",
                nullable: false,
                defaultValue: "pt-br");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildLanguage",
                table: "Guilds_Configs");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "System_Params",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
