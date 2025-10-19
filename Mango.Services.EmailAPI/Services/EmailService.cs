using System.Text;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Model;
using Mango.Services.EmailAPI.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Mango.Services.EmailAPI.Services;

public class EmailService : IEmailService
{
    private DbContextOptions<AppDbContext> _dbOptions;

    public EmailService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public async Task EmailCartAndLog(CartDTO cartDTO)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/>Cart Email Requested");
        message.AppendLine("<br/>Total " + cartDTO.CartHeader.CartTotal);
        message.AppendLine("<br/> ");
        message.AppendLine("<ul> ");
        foreach (var item in cartDTO.CartDetails)
        {
            message.Append("<li>");
            message.Append(item.Product.Name + " x " + item.Count);
            message.Append("</li>");
        }
        message.Append("</ul>");

        await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
    }

    public async Task EmailRegisterUserAndLog(string email)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/>Register User Requested");
        message.AppendLine("<br/>User registered with email: " + email);

        await LogAndEmail(message.ToString(), email);
    }

    public async Task LogOrderPlaced(RewardsMessage rewardsMessage)
    {
        string message = "New Order Placed. <br/> Order ID: " + rewardsMessage.OrderId;
        await LogAndEmail(message, "admin@email.com");
    }

    private async Task<bool> LogAndEmail(string message, string email)
    {
        try
        {
            EmailLogger emailLogger = new()
            {
                Email = email,
                EmailSent = DateTime.Now,
                Message = message
            };
            await using var _db = new AppDbContext(_dbOptions);
            await _db.EmailLoggers.AddAsync(emailLogger);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
