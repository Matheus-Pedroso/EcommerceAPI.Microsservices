using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Services;

public class RewardsService : IRewardsService
{
    private DbContextOptions<AppDbContext> _dbOptions;

    public RewardsService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public async Task UpdateRewards(RewardsMessage rewardsMessage)
    {
        try
        {
            Rewards rewards = new()
            {
                OrderId = rewardsMessage.OrderId,
                RewardsActivity = rewardsMessage.RewardsActivity,
                UserId = rewardsMessage.UserId,
                RewardsDate = DateTime.Now,
            };
            await using var context = new AppDbContext(_dbOptions);
            await context.Rewards.AddAsync(rewards);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
        }
    }
}
