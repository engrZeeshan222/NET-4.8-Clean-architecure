using CleanApp.Domain.Entities;
using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Stripe;
using CleanApp.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CleanApp.Infrastructure.Repositories
{
    public class StripeRepository : IStripeRepository
    {
        private string endpointSecret = ConfigurationManager.AppSettings["StripeEndpointSecret"];
        private string stripeApiKey= ConfigurationManager.AppSettings["StripeApiKey"];
        private readonly ApplicationDbContext db;

        public StripeRepository()
        {
            db = SingletonContext.Instance;
        }
        public async Task<ResponseDto> CheckOutSessionAsync(List<string> priceIds, int UserId)
        {
            var response = new ResponseDto();
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51N2XwECTIdeS2WufO7o4OXZ4NztBtiLaODpvtjgROviDokgn3itCniC0p5ILrLjUNVDplHIK9U4jvr1mvdMnw5P3008qCAASyL";

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var priceId in priceIds)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1

                    });
                }

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = ConfigurationManager.AppSettings["StripeSuccessUrl"],
                    CancelUrl = ConfigurationManager.AppSettings["StripeCancelUrl"],
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "subscription",
                    ClientReferenceId = UserId.ToString(),
                    PaymentMethodCollection = "if_required"

                };

                // Create a new session
                var service = new Stripe.Checkout.SessionService();
                var session = await service.CreateAsync(options);
                response.Data = session.Url;
                response.Status = true;
                response.Message = "User Adedd SuccessFully";
            }
            catch (Exception ex)
            {
                response.Message = ex.InnerException?.Message;
            }
            return response;
        }


        public async Task<ResponseDto> StripeWebhook(string requestBody)
        {
            var response = new ResponseDto();
            try
            {
                // Read the request body as a string


                DeserializeStripeDTO myDeserializedClass = JsonConvert.DeserializeObject<DeserializeStripeDTO>(requestBody);
                if (myDeserializedClass != null)
                {
                    var stripeEvent = myDeserializedClass.type;

                    // Handle the event

                    if (stripeEvent == Events.CheckoutSessionCompleted)
                    {
                        int id = int.Parse(myDeserializedClass.data.@object.client_reference_id);
                        var customerId = myDeserializedClass.data.@object.customer;
                        var email = myDeserializedClass.data.@object.customer_details.email;
                        var user = await db.Users.FirstOrDefaultAsync(u => u.ID == id);
                        var customer = new CleanApp.Domain.Entities.Customer()
                        {
                            Email = email,
                            stripeCustomerId = customerId,
                            User = user != null ? user : null,
                            CreatedDateTime = DateTime.Now

                        };
                        db.Customers.Add(customer);
                        await db.SaveChangesAsync();
                    }
                    else if (stripeEvent == Events.CheckoutSessionAsyncPaymentFailed)
                    {

                        var email = myDeserializedClass.data.@object.customer_details.email;
                        var customerId = myDeserializedClass.data.@object.customer;
                        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
                        var customer = new CleanApp.Domain.Entities.Customer()
                        {
                            Email = email,
                            stripeCustomerId = customerId,
                            User = user != null ? user : null,
                            IsActive = false,
                            CreatedDateTime = DateTime.Now,
                        };
                        db.Customers.Add(customer);
                        await db.SaveChangesAsync();
                    }

                }

                // Process the webhook event and prepare response
                response.Message = "Success";
                response.Status = true;
                return response;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                response.Status = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

    }
}
