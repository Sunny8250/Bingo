﻿using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO);
        Task<ResponseDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO?> ValidateStripeSessionAsync(int orderHeaderId);
    }
}
