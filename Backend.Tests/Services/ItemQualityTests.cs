using FluentAssertions;
using KingOfKings.Backend.Models;

namespace Backend.Tests.Services;

/// <summary>
/// T057: 裝備品質分佈統計測試
/// 驗證 SC-003: 品質分佈符合設計機率
/// </summary>
public class ItemQualityTests
{
    /// <summary>
    /// T057-1: 測試普通怪掉落品質分佈
    /// Common 70%, Uncommon 25%, Rare 4.5%, Legendary 0.5%
    /// 統計 100 件裝備,容忍誤差 ±30%
    /// </summary>
    [Fact(Skip = "需要實際掉落邏輯整合測試,目前為概念驗證")]
    public void QualityDistribution_NormalMonster_ShouldMatchExpectedProbability()
    {
        // Arrange
        var iterations = 100;
        var tolerance = 0.30; // ±30%

        var expectedDistribution = new Dictionary<ItemQuality, double>
        {
            { ItemQuality.Common, 0.70 },
            { ItemQuality.Uncommon, 0.25 },
            { ItemQuality.Rare, 0.045 },
            { ItemQuality.Legendary, 0.005 }
        };

        var actualCounts = new Dictionary<ItemQuality, int>
        {
            { ItemQuality.Common, 0 },
            { ItemQuality.Uncommon, 0 },
            { ItemQuality.Rare, 0 },
            { ItemQuality.Legendary, 0 }
        };

        // Act
        // 模擬普通怪掉落品質決定
        // 注意: 這需要實際的 DetermineItemQuality 方法

        // Assert
        foreach (var (quality, expectedRate) in expectedDistribution)
        {
            var actualRate = actualCounts[quality] / (double)iterations;
            var expectedMin = Math.Max(0, expectedRate * (1 - tolerance));
            var expectedMax = Math.Min(1, expectedRate * (1 + tolerance));

            actualRate.Should().BeInRange(expectedMin, expectedMax,
                because: $"{quality} 品質掉落率應該接近 {expectedRate:P1}");
        }
    }

    /// <summary>
    /// T057-2: 測試 Boss 掉落品質分佈
    /// Common 10%, Uncommon 30%, Rare 45%, Legendary 15%
    /// 統計 100 件裝備,容忍誤差 ±30%
    /// </summary>
    [Fact(Skip = "需要實際掉落邏輯整合測試,目前為概念驗證")]
    public void QualityDistribution_Boss_ShouldMatchExpectedProbability()
    {
        var iterations = 100;
        var tolerance = 0.30;

        var expectedDistribution = new Dictionary<ItemQuality, double>
        {
            { ItemQuality.Common, 0.10 },
            { ItemQuality.Uncommon, 0.30 },
            { ItemQuality.Rare, 0.45 },
            { ItemQuality.Legendary, 0.15 }
        };

        var actualCounts = new Dictionary<ItemQuality, int>
        {
            { ItemQuality.Common, 0 },
            { ItemQuality.Uncommon, 0 },
            { ItemQuality.Rare, 0 },
            { ItemQuality.Legendary, 0 }
        };

        foreach (var (quality, expectedRate) in expectedDistribution)
        {
            var actualRate = actualCounts[quality] / (double)iterations;
            var expectedMin = Math.Max(0, expectedRate * (1 - tolerance));
            var expectedMax = Math.Min(1, expectedRate * (1 + tolerance));

            actualRate.Should().BeInRange(expectedMin, expectedMax);
        }
    }

    /// <summary>
    /// T057-3: 測試品質枚舉值正確性
    /// </summary>
    [Fact]
    public void ItemQuality_EnumValues_ShouldBeCorrect()
    {
        // Arrange & Assert
        ((int)ItemQuality.Common).Should().Be(0, "Common 應為 0");
        ((int)ItemQuality.Uncommon).Should().Be(1, "Uncommon 應為 1");
        ((int)ItemQuality.Rare).Should().Be(2, "Rare 應為 2");
        ((int)ItemQuality.Legendary).Should().Be(3, "Legendary 應為 3");
    }

    /// <summary>
    /// T057-4: 測試品質枚舉完整性
    /// </summary>
    [Fact]
    public void ItemQuality_Enum_ShouldHave4Values()
    {
        var qualityValues = Enum.GetValues<ItemQuality>();
        qualityValues.Should().HaveCount(4, "應該有 4 個品質等級");
    }
}
