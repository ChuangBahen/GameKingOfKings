using FluentAssertions;
using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using KingOfKings.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.Services;

/// <summary>
/// T058: 套裝加成計算整合測試
/// 驗證 SC-004: 套裝加成正確計算
/// </summary>
public class SetBonusServiceTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly AppDbContext _dbContext;
    private readonly ISetBonusService _setBonusService;

    public SetBonusServiceTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase($"SetBonusTestDb_{Guid.NewGuid()}"));

        services.AddScoped<ISetBonusService, SetBonusService>();

        _serviceProvider = services.BuildServiceProvider();
        _dbContext = _serviceProvider.GetRequiredService<AppDbContext>();
        _setBonusService = _serviceProvider.GetRequiredService<ISetBonusService>();

        SeedTestData().Wait();
    }

    private async Task SeedTestData()
    {
        // 建立測試套裝
        var testSet = new EquipmentSet
        {
            Id = 1,
            Name = "史萊姆套裝",
            TotalPieces = 3
        };
        _dbContext.EquipmentSets.Add(testSet);

        // 建立套裝加成規則
        var bonuses = new[]
        {
            new SetBonus
            {
                Id = 1,
                SetId = 1,
                RequiredPieces = 2,
                BonusJson = "{\"MaxHp\":20}",
                Description = "HP+20"
            },
            new SetBonus
            {
                Id = 2,
                SetId = 1,
                RequiredPieces = 3,
                BonusJson = "{\"MaxHp\":20,\"MaxMp\":15,\"Str\":2,\"Dex\":2,\"Int\":2,\"Wis\":2,\"Con\":2}",
                Description = "HP+20, MP+15, 全屬性+2"
            }
        };
        _dbContext.SetBonuses.AddRange(bonuses);

        // 建立套裝裝備
        var items = new[]
        {
            new Item { Id = 101, Name = "史萊姆頭盔", Type = "armor", SetId = 1, PropertiesJson = "{\"Def\":3}", IconUrl = "helmet.png", Description = "頭盔" },
            new Item { Id = 102, Name = "史萊姆護甲", Type = "armor", SetId = 1, PropertiesJson = "{\"Def\":5}", IconUrl = "armor.png", Description = "護甲" },
            new Item { Id = 103, Name = "史萊姆靴子", Type = "armor", SetId = 1, PropertiesJson = "{\"Def\":2}", IconUrl = "boots.png", Description = "靴子" }
        };
        _dbContext.Items.AddRange(items);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// T058-1: 測試未裝備任何套裝部件時無加成
    /// </summary>
    [Fact]
    public async Task CalculateSetBonuses_NoEquippedSetItems_ShouldReturnEmpty()
    {
        // Arrange
        var playerId = Guid.NewGuid();

        // Act
        var bonuses = await _setBonusService.CalculateSetBonusesAsync(playerId);

        // Assert
        bonuses.Should().BeEmpty("未裝備任何套裝部件應無加成");
    }

    /// <summary>
    /// T058-2: 測試裝備 2 件套裝時啟用 2 件加成
    /// </summary>
    [Fact]
    public async Task CalculateSetBonuses_TwoSetPieces_ShouldActivate2PieceBonus()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            Username = "test",
            PasswordHash = "test",
            CharacterName = "測試角色",
            MaxHp = 100,
            MaxMp = 100
        };
        _dbContext.Players.Add(player);

        // 裝備 2 件套裝
        var inventory = new[]
        {
            new InventoryItem { PlayerId = playerId, ItemId = 101, Quantity = 1, IsEquipped = true },
            new InventoryItem { PlayerId = playerId, ItemId = 102, Quantity = 1, IsEquipped = true }
        };
        _dbContext.InventoryItems.AddRange(inventory);
        await _dbContext.SaveChangesAsync();

        // Act
        var bonuses = await _setBonusService.CalculateSetBonusesAsync(playerId);

        // Assert
        bonuses.Should().ContainKey("MaxHp", "應該有 HP 加成");
        bonuses["MaxHp"].Should().Be(20, "2 件套裝應有 HP+20 加成");
    }

    /// <summary>
    /// T058-3: 測試裝備全套 3 件時累加所有加成
    /// </summary>
    [Fact]
    public async Task CalculateSetBonuses_ThreeSetPieces_ShouldActivateAllBonuses()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            Username = "test",
            PasswordHash = "test",
            CharacterName = "測試角色",
            MaxHp = 100,
            MaxMp = 100
        };
        _dbContext.Players.Add(player);

        // 裝備全套 3 件
        var inventory = new[]
        {
            new InventoryItem { PlayerId = playerId, ItemId = 101, Quantity = 1, IsEquipped = true },
            new InventoryItem { PlayerId = playerId, ItemId = 102, Quantity = 1, IsEquipped = true },
            new InventoryItem { PlayerId = playerId, ItemId = 103, Quantity = 1, IsEquipped = true }
        };
        _dbContext.InventoryItems.AddRange(inventory);
        await _dbContext.SaveChangesAsync();

        // Act
        var bonuses = await _setBonusService.CalculateSetBonusesAsync(playerId);

        // Assert
        bonuses.Should().ContainKey("MaxHp");
        bonuses.Should().ContainKey("MaxMp");
        bonuses.Should().ContainKey("Str");

        // 2件加成 (HP+20) + 3件加成 (HP+20, MP+15, 全屬性+2) = 累加
        bonuses["MaxHp"].Should().Be(40, "2件加成20 + 3件加成20 = 40");
        bonuses["MaxMp"].Should().Be(15, "3件加成 MP+15");
        bonuses["Str"].Should().Be(2, "3件加成 全屬性+2");
        bonuses["Dex"].Should().Be(2);
        bonuses["Int"].Should().Be(2);
        bonuses["Wis"].Should().Be(2);
        bonuses["Con"].Should().Be(2);
    }

    /// <summary>
    /// T058-4: 測試GetActiveSetBonusesAsync回傳正確結構
    /// </summary>
    [Fact]
    public async Task GetActiveSetBonuses_WithEquippedSet_ShouldReturnCorrectStructure()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            Username = "test",
            PasswordHash = "test",
            CharacterName = "測試角色",
            MaxHp = 100,
            MaxMp = 100
        };
        _dbContext.Players.Add(player);

        var inventory = new[]
        {
            new InventoryItem { PlayerId = playerId, ItemId = 101, Quantity = 1, IsEquipped = true },
            new InventoryItem { PlayerId = playerId, ItemId = 102, Quantity = 1, IsEquipped = true }
        };
        _dbContext.InventoryItems.AddRange(inventory);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeSetBonuses = await _setBonusService.GetActiveSetBonusesAsync(playerId);

        // Assert
        activeSetBonuses.Should().HaveCount(1, "應該有 1 個啟用的套裝");

        var setInfo = activeSetBonuses.First();
        setInfo.SetName.Should().Be("史萊姆套裝");
        setInfo.EquippedPieces.Should().Be(2);
        setInfo.TotalPieces.Should().Be(3);
        setInfo.ActiveBonuses.Should().HaveCount(1, "只有 2 件加成啟用");

        var bonus = setInfo.ActiveBonuses.First();
        bonus.RequiredPieces.Should().Be(2);
        bonus.Description.Should().Be("HP+20");
    }

    /// <summary>
    /// T058-5: 測試脫下裝備後加成消失 (SC-004 即時性)
    /// </summary>
    [Fact]
    public async Task CalculateSetBonuses_AfterUnequip_ShouldRecalculateImmediately()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            Username = "test",
            PasswordHash = "test",
            CharacterName = "測試角色",
            MaxHp = 100,
            MaxMp = 100
        };
        _dbContext.Players.Add(player);

        var inventory = new[]
        {
            new InventoryItem { Id = 1, PlayerId = playerId, ItemId = 101, Quantity = 1, IsEquipped = true },
            new InventoryItem { Id = 2, PlayerId = playerId, ItemId = 102, Quantity = 1, IsEquipped = true }
        };
        _dbContext.InventoryItems.AddRange(inventory);
        await _dbContext.SaveChangesAsync();

        // 驗證初始加成
        var initialBonuses = await _setBonusService.CalculateSetBonusesAsync(playerId);
        initialBonuses["MaxHp"].Should().Be(20);

        // Act: 脫下一件裝備
        var itemToUnequip = await _dbContext.InventoryItems.FirstAsync(i => i.Id == 1);
        itemToUnequip.IsEquipped = false;
        await _dbContext.SaveChangesAsync();

        // 立即重新計算
        var newBonuses = await _setBonusService.CalculateSetBonusesAsync(playerId);

        // Assert
        newBonuses.Should().BeEmpty("裝備件數低於 2 件,加成應消失");
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        _serviceProvider.Dispose();
    }
}
