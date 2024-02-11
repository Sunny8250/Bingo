using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models.DTO;

namespace Mango.Services.EmailApi.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO  cartDTO);
        Task EmailUserAndLog(string email);
        Task EmailAndLogOrderPlaced(OrderConfirmation orderConfirmation);
    }
}
