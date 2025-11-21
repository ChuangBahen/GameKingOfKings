using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Race",
                table: "PlayerCharacters");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Wis",
                table: "PlayerCharacters",
                newName: "Stats_Wis");

            migrationBuilder.RenameColumn(
                name: "Str",
                table: "PlayerCharacters",
                newName: "Stats_Str");

            migrationBuilder.RenameColumn(
                name: "Int",
                table: "PlayerCharacters",
                newName: "Stats_Int");

            migrationBuilder.RenameColumn(
                name: "Dex",
                table: "PlayerCharacters",
                newName: "Stats_Dex");

            migrationBuilder.RenameColumn(
                name: "Con",
                table: "PlayerCharacters",
                newName: "Stats_Con");

            migrationBuilder.RenameColumn(
                name: "StatsJson",
                table: "Items",
                newName: "PropertiesJson");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "PlayerCharacters",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Class",
                table: "PlayerCharacters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PlayerCharacters",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Stats_Wis",
                table: "PlayerCharacters",
                newName: "Wis");

            migrationBuilder.RenameColumn(
                name: "Stats_Str",
                table: "PlayerCharacters",
                newName: "Str");

            migrationBuilder.RenameColumn(
                name: "Stats_Int",
                table: "PlayerCharacters",
                newName: "Int");

            migrationBuilder.RenameColumn(
                name: "Stats_Dex",
                table: "PlayerCharacters",
                newName: "Dex");

            migrationBuilder.RenameColumn(
                name: "Stats_Con",
                table: "PlayerCharacters",
                newName: "Con");

            migrationBuilder.RenameColumn(
                name: "PropertiesJson",
                table: "Items",
                newName: "StatsJson");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PlayerCharacters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "PlayerCharacters",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PlayerCharacters",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Race",
                table: "PlayerCharacters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Items",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
