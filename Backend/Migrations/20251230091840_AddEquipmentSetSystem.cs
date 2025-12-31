using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentSetSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DroppableEquipmentIds",
                table: "Monsters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EquipmentDropRate",
                table: "Monsters",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentSetId",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quality",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetId",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TotalPieces = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetBonuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiredPieces = table.Column<int>(type: "INTEGER", nullable: false),
                    BonusJson = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetBonuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetBonuses_EquipmentSets_SetId",
                        column: x => x.SetId,
                        principalTable: "EquipmentSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "EquipmentSetId", "Quality", "SetId" },
                values: new object[] { null, 0, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Items_EquipmentSetId",
                table: "Items",
                column: "EquipmentSetId");

            migrationBuilder.CreateIndex(
                name: "IX_SetBonuses_SetId",
                table: "SetBonuses",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_EquipmentSets_EquipmentSetId",
                table: "Items",
                column: "EquipmentSetId",
                principalTable: "EquipmentSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_EquipmentSets_EquipmentSetId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "SetBonuses");

            migrationBuilder.DropTable(
                name: "EquipmentSets");

            migrationBuilder.DropIndex(
                name: "IX_Items_EquipmentSetId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DroppableEquipmentIds",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "EquipmentDropRate",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "EquipmentSetId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Quality",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "Items");
        }
    }
}
