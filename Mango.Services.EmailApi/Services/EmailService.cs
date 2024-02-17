using Mango.Services.EmailApi.Data;
using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models;
using Mango.Services.EmailApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailApi.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

		public async Task EmailAndLogOrderPlaced(OrderConfirmation orderConfirmation)
		{
			string message = $"<br/>New Order Placed <br/>OrderId: {orderConfirmation.OrderId} <br/>";

			await LogAndEmail(orderConfirmation.ToString(), orderConfirmation.Email);
		}

		//We cannot use registered DbContext here because dbContext is Scoped and EmailService is singleton, so to resolve this
		//we should create a another dbcontext where it will be singleton


		public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total "+ cartDTO.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach(var item in cartDTO.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " x " + item.Count);
                message.AppendLine("<li>");
            }
            message.AppendLine("</ul>");
            await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
        }
        public async Task EmailUserAndLog(string email)
        {
            string message = $"<br/>New User Registered <br/>Email {email} <br/>";
            
            await LogAndEmail(message.ToString(), email);
        }
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = email,
                    Message = message,
                    EmailSent = DateTime.Now
                };
                await using (var dbContext = new AppDbContext(_dbOptions))
                {
                    await dbContext.EmailLoggers.AddAsync(emailLogger);
                    await dbContext.SaveChangesAsync();
                    await dbContext.DisposeAsync();
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
