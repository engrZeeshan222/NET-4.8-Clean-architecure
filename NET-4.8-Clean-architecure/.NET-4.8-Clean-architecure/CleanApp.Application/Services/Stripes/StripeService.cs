using CleanApp.Application.Services.Stripes;
using CleanApp.Infrastructure;
using CleanApp.Infrastructure.Interface;
using CleanApp.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanApplication.Application.Services.Stripes
{
    public class StripeService:IStripeService
    {
        private readonly IStripeRepository stripeRepository;

        public StripeService()
        {
            this.stripeRepository = new StripeRepository();
        }

        public async Task<ResponseDto> CheckOutSessionAsync(List<string> priceIds, int userId)
        {
            return await this.stripeRepository.CheckOutSessionAsync(priceIds, userId);
        }
        public async Task<ResponseDto> StripeWebhook(string requestBody)
        {
            return await this.stripeRepository.StripeWebhook(requestBody);
        }
    }
}
