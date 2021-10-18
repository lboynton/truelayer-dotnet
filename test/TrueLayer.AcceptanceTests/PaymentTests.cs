using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    public class PaymentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PaymentTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_create_payment()
        {
            var paymentRequest = new CreatePaymentRequest(
                100,
                Currencies.GBP,
                PaymentMethod.BankTransfer(providerFilter: new ProviderFilter
                {
                    ProviderIds = new[] { "mock-payments-gb-redirect" }
                }),
                new ExternalAccountBeneficiary(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SortCodeAccountNumberSchemeIdentifier("567890", "12345678")
                )
            );

            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, Guid.NewGuid().ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Data.Value.ShouldBeOfType<CreatePaymentResponse.AuthorizationRequired>();

            response.Data.Switch(
                authRequired =>
                {
                    authRequired.Status.ShouldBe("authorization_required");

                    authRequired.AmountInMinor.ShouldBe(paymentRequest.AmountInMinor);
                    authRequired.Currency.ShouldBe(paymentRequest.Currency);
                    authRequired.Id.ShouldNotBeNullOrWhiteSpace();
                    authRequired.ResourceToken.ShouldNotBeNullOrWhiteSpace();
                    authRequired.CreatedAt.ShouldNotBe(default);
                }
            );
        }
    }
}