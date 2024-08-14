using CleanApp.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Stripes
{
    public interface IStripeService
    {
        Task<ResponseDto> CheckOutSessionAsync(List<string> priceIds, int userId);
        Task<ResponseDto> StripeWebhook(string requestBody);

    }
}
