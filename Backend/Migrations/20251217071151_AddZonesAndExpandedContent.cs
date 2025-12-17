using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddZonesAndExpandedContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ZoneId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Monsters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBoss",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Monsters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredClass",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredLevel",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    MinLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "黏稠的凝膠，可用於製作藥水。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "野鼠的尾巴，村裡的雜貨店會收購。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "適合新手冒險者的戒指，提供少量屬性加成。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "一把生鏽的舊劍，勉強能用。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "普通的木製法杖，適合初學法師。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "恢復 30 點生命值。", null, 1 });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "RequiredClass", "RequiredLevel" },
                values: new object[] { "恢復 20 點魔力值。", null, 1 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Description", "Name", "PropertiesJson", "RequiredClass", "RequiredLevel", "Type" },
                values: new object[,]
                {
                    { 8, "粗糙的狼皮，可用於製作皮革裝備。", "狼皮", "{}", null, 1, 3 },
                    { 9, "鋒利的狼牙，常用於製作飾品或武器。", "狼牙", "{}", null, 1, 3 },
                    { 10, "哥布林使用的短刀，雖然生鏽但仍然鋒利。", "生鏽短刀", "{\"Atk\":8}", null, 5, 0 },
                    { 11, "基本的皮革護甲，提供不錯的防護。", "皮革護甲", "{\"Def\":5}", null, 5, 1 },
                    { 12, "哥布林酋長的巨斧，只有戰士能夠駕馭。", "酋長之斧", "{\"Atk\":15,\"Str\":3}", 0, 8, 0 },
                    { 13, "骷髏的殘骸，散發著微弱的魔力。", "骨頭碎片", "{}", null, 1, 3 },
                    { 14, "腐爛的肉塊，用途不明，或許死靈法師需要它。", "腐肉", "{}", null, 1, 3 },
                    { 15, "死靈法師的長袍，蘊含強大的魔力，只有法師能穿戴。", "法師長袍", "{\"Def\":3,\"Int\":8,\"Wis\":5}", 1, 12, 1 },
                    { 16, "濃縮的生命藥水，恢復 80 點生命值。", "高級生命藥水", "{\"HealHp\":80}", null, 8, 2 },
                    { 17, "濃縮的魔力藥水，恢復 50 點魔力值。", "高級魔力藥水", "{\"HealMp\":50}", null, 8, 2 }
                });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "IsBoss", "Level" },
                values: new object[] { "一團軟趴趴的黏液生物，對新手來說是很好的練習對象。", false, 1 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "IsBoss", "Level" },
                values: new object[] { "敏捷的小型齧齒動物，雖然弱小但攻擊迅速。", false, 2 });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "IsBoss", "Level" },
                values: new object[] { "史萊姆族群的首領，體型巨大且極具威脅性。", true, 5 });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "Id", "Attack", "CurrentHp", "Defense", "Description", "ExpReward", "IsBoss", "Level", "LocationId", "MaxHp", "Name" },
                values: new object[,]
                {
                    { 4, 12, 60, 4, "森林中的掠食者，成群結隊狩獵，牙齒鋒利無比。", 25, false, 5, 7, 60, "野狼" },
                    { 5, 15, 50, 3, "狡猾的小型類人生物，喜歡在森林中埋伏旅人。", 30, false, 6, 9, 50, "哥布林" },
                    { 6, 25, 200, 8, "哥布林部落的首領，身披戰甲，揮舞著巨斧，是森林中最危險的存在。", 150, true, 10, 10, 200, "哥布林酋長" },
                    { 7, 18, 80, 6, "被黑暗魔法復活的亡者骨骸，永不疲倦地守護著礦坑。", 45, false, 10, 12, 80, "骷髏" },
                    { 8, 20, 100, 8, "腐爛的屍體被邪惡力量驅動，行動遲緩但力量驚人。", 55, false, 12, 13, 100, "殭屍" },
                    { 9, 35, 300, 10, "操控亡靈的邪惡法師，是礦坑中所有不死生物的主人。", 250, true, 15, 15, 300, "死靈法師" }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ExitsJson", "ZoneId" },
                values: new object[] { "你站在一個寧靜村莊的中心。一座噴泉在旁邊潺潺作響，村民們悠閒地走過。往東可見一片幽暗的森林入口。", "{\"n\":2,\"e\":6,\"s\":4,\"w\":3}", 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "ZoneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ExitsJson", "ZoneId" },
                values: new object[] { "{\"e\":1}", 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "ZoneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "ExitsJson", "ZoneId" },
                values: new object[] { "一片爬滿史萊姆的原野。牠們看起來大多無害，但被激怒時可能會攻擊。往南可見一個廢棄礦坑的入口...", "{\"n\":4,\"s\":11}", 1 });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Description", "ExitsJson", "Name", "ZoneId" },
                values: new object[,]
                {
                    { 6, "高聳的樹木遮蔽了大部分陽光，空氣中瀰漫著潮濕的泥土味。你隱約聽到遠處傳來狼嚎聲...", "{\"w\":1,\"e\":7}", "低語森林入口", 2 },
                    { 7, "蜿蜒的小徑穿過茂密的森林，地上到處可見野狼的腳印。偶爾可見樹幹上哥布林留下的奇怪記號。", "{\"w\":6,\"e\":8,\"s\":9}", "幽暗林道", 2 },
                    { 8, "一棵巨大的橡樹矗立在森林深處，樹齡看起來有數百年。樹下堆滿了落葉和小動物的骨骸。", "{\"w\":7}", "古老橡樹", 2 },
                    { 9, "簡陋的帳篷和篝火殘跡顯示這裡是哥布林的聚集地。空氣中飄散著腐臭的氣味，地上散落著啃食過的骨頭。", "{\"n\":7,\"s\":10}", "哥布林營地", 2 },
                    { 10, "一個用樹枝和獸皮搭建的大型棚屋，這裡是哥布林酋長的領地。牆上掛滿了冒險者的裝備作為戰利品。", "{\"n\":9}", "酋長的巢穴", 2 },
                    { 11, "一個被木頭支架撐起的礦坑入口，散發著陰冷潮濕的氣息。牆上的火把早已熄滅，只剩下漆黑的通道。", "{\"n\":5,\"s\":12}", "礦坑入口", 3 },
                    { 12, "狹窄的隧道向下延伸，兩側是粗糙的岩壁。地上散落著生鏽的採礦工具和不明的白骨。", "{\"n\":11,\"s\":13,\"e\":14}", "昏暗隧道", 3 },
                    { 13, "這裡曾經發生過嚴重的坍塌，巨石堵住了大部分通道。在碎石堆中，你可以看到骷髏正在四處遊蕩。", "{\"n\":12}", "崩塌礦井", 3 },
                    { 14, "礦坑深處竟然有一座古老的祭壇，上面刻滿了不祥的符文。空氣中瀰漫著死亡的氣息，殭屍在此徘徊。", "{\"w\":12,\"n\":15}", "詭異祭壇", 3 },
                    { 15, "一間被黑暗能量籠罩的密室，牆上的燭火發出詭異的綠光。一位身披黑袍的死靈法師正在進行邪惡的儀式...", "{\"s\":14}", "死靈法師密室", 3 }
                });

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "Id", "Description", "MaxLevel", "MinLevel", "Name" },
                values: new object[,]
                {
                    { 1, "適合新手冒險的安全區域", 5, 1, "新手村" },
                    { 2, "森林深處潛伏著危險的野獸和哥布林", 10, 5, "低語森林" },
                    { 3, "被亡靈佔據的古老礦坑，充滿死亡的氣息", 15, 10, "廢棄礦坑" }
                });

            migrationBuilder.InsertData(
                table: "LootTableEntries",
                columns: new[] { "Id", "DropRate", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 7, 60.0, 8, 1, 1, 4 },
                    { 8, 40.0, 9, 2, 1, 4 },
                    { 9, 25.0, 10, 1, 1, 5 },
                    { 10, 15.0, 11, 1, 1, 5 },
                    { 11, 100.0, 12, 1, 1, 6 },
                    { 12, 50.0, 16, 2, 1, 6 },
                    { 13, 70.0, 13, 3, 1, 7 },
                    { 14, 60.0, 14, 2, 1, 8 },
                    { 15, 100.0, 15, 1, 1, 9 },
                    { 16, 60.0, 17, 3, 1, 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "LootTableEntries",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "IsBoss",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "RequiredClass",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RequiredLevel",
                table: "Items");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ExitsJson" },
                values: new object[] { "你站在一個寧靜村莊的中心。一座噴泉在旁邊潺潺作響，村民們悠閒地走過。", "{\"n\":2,\"e\":3,\"s\":4}" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "ExitsJson",
                value: "{\"w\":1}");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "ExitsJson" },
                values: new object[] { "一片爬滿史萊姆的原野。牠們看起來大多無害，但被激怒時可能會攻擊。傳說這裡偶爾會出現巨大的史萊姆王...", "{\"n\":4}" });
        }
    }
}
