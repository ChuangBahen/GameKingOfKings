using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSetEquipmentSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 101,
                column: "EquipmentSlot",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 102,
                column: "EquipmentSlot",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 103,
                column: "EquipmentSlot",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 104,
                column: "EquipmentSlot",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 105,
                column: "EquipmentSlot",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 106,
                column: "EquipmentSlot",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 107,
                column: "EquipmentSlot",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 108,
                column: "EquipmentSlot",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 109,
                column: "EquipmentSlot",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 110,
                column: "EquipmentSlot",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 111,
                column: "EquipmentSlot",
                value: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 101,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 102,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 103,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 104,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 105,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 106,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 107,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 108,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 109,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 110,
                column: "EquipmentSlot",
                value: null);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 111,
                column: "EquipmentSlot",
                value: null);
        }
    }
}
