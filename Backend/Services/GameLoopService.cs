using KingOfKings.Backend.Data;
using KingOfKings.Backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KingOfKings.Backend.Services
{
    /// <summary>
    /// Background service that runs the game loop (ticks).
    /// 執行遊戲迴圈 (ticks) 的背景服務。
    /// </summary>
    public class GameLoopService : BackgroundService, IGameLoopService
    {
        private readonly ILogger<GameLoopService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _tickRate = TimeSpan.FromSeconds(1); // 1 tick per second

        public GameLoopService(
            ILogger<GameLoopService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Game Loop Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessGameTick();
                    await Task.Delay(_tickRate, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when stopping
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Game Loop");
                }
            }

            _logger.LogInformation("Game Loop Service is stopping.");
        }

        private async Task ProcessGameTick()
        {
            using var scope = _serviceProvider.CreateScope();
            var combatManager = scope.ServiceProvider.GetRequiredService<ICombatManager>();
            var gameEngine = scope.ServiceProvider.GetRequiredService<IGameEngine>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();

            // Process combat ticks
            var activeCombats = await combatManager.GetActiveCombatsAsync();
            foreach (var combat in activeCombats)
            {
                try
                {
                    var result = await combatManager.ProcessCombatTickAsync(combat.Id);

                    // Send combat update to player via SignalR
                    await hubContext.Clients.All.SendAsync("ReceiveMessage", "Combat", result.Message);

                    // Send HP/MP update
                    if (result.PlayerCurrentHp.HasValue)
                    {
                        await hubContext.Clients.All.SendAsync("UpdateStats", new
                        {
                            currentHp = result.PlayerCurrentHp,
                            maxHp = result.PlayerMaxHp,
                            monsterHp = result.MonsterCurrentHp,
                            monsterMaxHp = result.MonsterMaxHp
                        });
                    }

                    // 戰鬥結束時推送背包更新 (物品掉落即時更新)
                    if (result.CombatEnded)
                    {
                        using var scope2 = _serviceProvider.CreateScope();
                        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
                        await GameHub.PushInventoryUpdateAsync(hubContext, db2, result.PlayerId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing combat tick for combat {CombatId}", combat.Id);
                }
            }

            // Process game tick (regen, etc.)
            await gameEngine.TickAsync();
        }
    }
}
