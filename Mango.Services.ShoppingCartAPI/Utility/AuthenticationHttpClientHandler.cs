
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Mango.Services.ShoppingCartAPI.Utility;

public class AuthenticationHttpClientHandler(IHttpContextAccessor _acessor) : DelegatingHandler 
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _acessor.HttpContext.GetTokenAsync("access_token");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
