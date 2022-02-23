/*
 * Arquivo: 20211015203933_CreateUserConfigs.cs
 * Criado em: 15-10-2021
 * https://github.com/ForceFK
 * Última modificação: 15-10-2021
 */
using Microsoft.EntityFrameworkCore.Migrations;

namespace BOTDiscord_RP.Migrations
{
    public partial class CreateUserConfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "System_Params",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Users_Configs",
                columns: table => new
                {
                    UserID = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Configs", x => x.UserID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users_Configs");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "System_Params",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
