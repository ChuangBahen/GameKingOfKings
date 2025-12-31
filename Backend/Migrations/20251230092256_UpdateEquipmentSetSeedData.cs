using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentSetSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EquipmentSets",
                columns: new[] { "Id", "Description", "Name", "TotalPieces" },
                values: new object[,]
                {
                    { 1, "新手入門套裝，由史萊姆王的凝膠製成", "史萊姆套裝", 3 },
                    { 2, "敏捷型套裝，適合獵人和遊俠", "森林獵人套裝", 4 },
                    { 3, "魔法型套裝，蘊含死亡的力量", "死靈法師套裝", 4 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Description", "EquipmentSetId", "Name", "PropertiesJson", "Quality", "RequiredClass", "RequiredLevel", "SetId", "Type" },
                values: new object[,]
                {
                    { 101, "由史萊姆凝膠製成的頭盔，意外地堅韌。", null, "史萊姆頭盔", "{\"Def\":3,\"Con\":1}", 1, null, 3, 1, 1 },
                    { 102, "彈性十足的護甲，能吸收部分衝擊。", null, "史萊姆護甲", "{\"Def\":5,\"MaxHp\":10}", 1, null, 3, 1, 1 },
                    { 103, "黏稠但靈活的護手。", null, "史萊姆護手", "{\"Def\":2,\"Dex\":1}", 1, null, 3, 1, 1 },
                    { 104, "森林獵人戴的皮帽，增強感知能力。", null, "獵人皮帽", "{\"Def\":4,\"Dex\":2}", 2, null, 6, 2, 1 },
                    { 105, "輕便的皮革護甲，不妨礙行動。", null, "獵人皮甲", "{\"Def\":8,\"Dex\":3}", 2, null, 6, 2, 1 },
                    { 106, "增強握力的皮革手套。", null, "獵人手套", "{\"Def\":3,\"Atk\":2}", 2, null, 6, 2, 1 },
                    { 107, "安靜輕便的長靴，適合潛行。", null, "獵人長靴", "{\"Def\":4,\"Dex\":2}", 2, null, 6, 2, 1 },
                    { 108, "散發死亡氣息的兜帽，增強魔力。", null, "亡靈兜帽", "{\"Def\":3,\"Int\":5,\"MaxMp\":20}", 3, null, 12, 3, 1 },
                    { 109, "死靈法師的長袍，蘊含黑暗能量。", null, "亡靈長袍", "{\"Def\":6,\"Int\":8,\"Wis\":5}", 3, null, 12, 3, 1 },
                    { 110, "骨製護腕，增強施法能力。", null, "亡靈護腕", "{\"Def\":2,\"Int\":3,\"MaxMp\":15}", 3, null, 12, 3, 1 },
                    { 111, "漂浮般輕盈的靴子。", null, "亡靈之靴", "{\"Def\":4,\"Int\":4,\"Wis\":3}", 3, null, 12, 3, 1 }
                });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[101,102,103]", 0.5 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[101,102,103]", 0.5 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[101,102,103]", 50.0 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[104,105,106,107]", 0.5 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[104,105,106,107]", 1.0 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[104,105,106,107]", 70.0 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[108,109,110,111]", 1.0 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[108,109,110,111]", 2.0 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DroppableEquipmentIds", "EquipmentDropRate" },
                values: new object[] { "[108,109,110,111]", 100.0 });

            migrationBuilder.InsertData(
                table: "SetBonuses",
                columns: new[] { "Id", "BonusJson", "Description", "RequiredPieces", "SetId" },
                values: new object[,]
                {
                    { 1, "{\"MaxHp\":20}", "HP+20", 2, 1 },
                    { 2, "{\"MaxMp\":15,\"Str\":2,\"Dex\":2,\"Int\":2,\"Wis\":2,\"Con\":2}", "MP+15, 全屬性+2", 3, 1 },
                    { 3, "{\"Atk\":5}", "攻擊+5", 2, 2 },
                    { 4, "{\"Dex\":5}", "敏捷+5", 3, 2 },
                    { 5, "{\"Atk\":10,\"CritRate\":5}", "攻擊+10, 暴擊+5%", 4, 2 },
                    { 6, "{\"Int\":8}", "智力+8", 2, 3 },
                    { 7, "{\"MaxMp\":50}", "MP+50", 3, 3 },
                    { 8, "{\"MagicDamage\":15}", "魔法傷害+15%", 4, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SetBonuses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EquipmentSets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EquipmentSets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EquipmentSets",
                keyColumn: "Id",
                keyValue: 3);

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
        }
    }
}
