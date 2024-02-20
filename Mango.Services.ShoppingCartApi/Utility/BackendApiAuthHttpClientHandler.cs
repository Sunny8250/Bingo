using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Mango.Services.ShoppingCartApi.Utility
{
    //Delegating Handler is like .Net core middleware, the key difference is it will be on client side.
    //When we make an http request using http client we can leverage the delegating handler to pass out bearer token
    public class BackendApiAuthHttpClientHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public BackendApiAuthHttpClientHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //access_token where token is stored
            var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
            if(token !=null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
