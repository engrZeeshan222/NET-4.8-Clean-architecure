using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.Interface
{
    public interface IStripeRepository
    {
        Task<ResponseDto> CheckOutSessionAsync(List<string> priceIds, int userId);
        Task<ResponseDto> StripeWebhook(string requestBody);
    }
}
