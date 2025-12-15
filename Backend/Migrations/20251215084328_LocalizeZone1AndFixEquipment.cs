using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class LocalizeZone1AndFixEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "史萊姆凝膠");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "野鼠尾巴");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "新手之戒");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "生鏽的劍");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "木杖");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "生命藥水");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "魔力藥水");

            migrationBuilder.UpdateData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 4,
                column: "DropRate",
                value: 15.0);

            migrationBuilder.InsertData(
                table: "LootTableEntries",
                columns: new[] { "Id", "DropRate", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[] { 6, 30.0, 7, 2, 1, 3 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "史萊姆");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "野鼠");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Attack", "CurrentHp", "Defense", "ExpReward", "MaxHp", "Name" },
                values: new object[] { 12, 100, 3, 80, 100, "史萊姆王" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "你站在一個寧靜村莊的中心。一座噴泉在旁邊潺潺作響，村民們悠閒地走過。", "新手村廣場" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "木製人偶排列成行。空氣中迴盪著木劍敲擊的聲音，幾位新手正在練習基本招式。", "練武場" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "一間溫馨的小屋，瀰漫著藥草的香氣。牆上掛滿古老的地圖和冒險者的紀念品。", "村長的小屋" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "村莊的邊緣地帶，青草隨風搖曳。你可以看到一些史萊姆在附近游蕩，偶爾有野鼠竄過。", "村莊外圍" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "一片爬滿史萊姆的原野。牠們看起來大多無害，但被激怒時可能會攻擊。傳說這裡偶爾會出現巨大的史萊姆王...", "史萊姆平原" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Slime Gel");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Rat Tail");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Novice Ring");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Rusty Sword");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Wooden Staff");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Health Potion");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Mana Potion");

            migrationBuilder.UpdateData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 4,
                column: "DropRate",
                value: 10.0);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Slime");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Rat");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Attack", "CurrentHp", "Defense", "ExpReward", "MaxHp", "Name" },
                values: new object[] { 15, 150, 5, 100, 150, "King Slime" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "You are standing in the center of a peaceful village. A fountain bubbles softly nearby.", "Village Square" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Wooden dummies stand in rows here. The sound of striking wood fills the air.", "Training Grounds" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A cozy hut with the smell of herbs.", "Village Elder's House" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "The edge of the village. Grass sways in the breeze. You can see slimes wandering about.", "Village Outskirts" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A field crawling with slimes. They seem mostly harmless, but they might attack if provoked.", "Slime Field" });
        }
    }
}
