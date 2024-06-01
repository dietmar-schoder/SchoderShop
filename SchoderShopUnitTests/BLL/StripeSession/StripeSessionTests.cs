using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoderChain;
using SchoderShop.BLL;
using SchoderShop.BLL.StripeCallback;
using SchoderShop.BLL.StripeSession;
using SchoderShop.DAL;
using SchoderShop.Helpers;
using SchoderShopUnitTests.TestHelpers;
using SchoderShopUnitTests.TestModels;
using Stripe;
using Stripe.Checkout;

namespace SchoderShopUnitTests.BLL.StripeSession
{
    [TestClass]
    public class StripeSessionTests
    {
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task GetProduct_Calls_ProductAccessor_With_Correct_Parameters_And_Returns_Correct_Result(bool productIsFound)
        {
            // Given I have a GetProduct processor with mocked injections and ShopData with a product id
            var testProductId = TestData.TestGuid;
            var testProduct = new TestProduct { Id = testProductId };
            var stripeData = new StripeData { ProductId = testProduct.Id };
            var mockSlackManager = new Mock<ISlackManager>();
            var mockProductAccessor = new Mock<IProductAccessor>();
            mockProductAccessor.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(productIsFound ? testProduct : null);
            var processor = new GetProduct(mockProductAccessor.Object, stripeData, mockSlackManager.Object);
            var processor2 = new TestProcessor(mockSlackManager.Object);

            // When I run the processor with a successor
            var result = await new Chain(new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(GetProduct),
                typeof(TestProcessor));

            // Then I expect the StripeCallbackAccessor.AddStripeCallbackAsync to be called with the Stripe Json callback content
            mockProductAccessor.Verify(m => m.GetAsync(It.Is<Guid>(s => s.Equals(testProductId))), Times.Once);

            // And I expect the second processor to be called if a product id was found
            Assert.AreEqual(productIsFound ? nameof(TestProcessor) : nameof(GetProduct), result.StackTrace.Last());
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [Obsolete]
        public async Task SetStripeParameters_Returns_Correct_Parameters(bool isTest)
        {
            // Given I have a SetStripeUrls processor with mocked injections
            var stripeData = new StripeData { IsTest = isTest }; // !!!! new ShopData { IsDonation = isDonation };
            var stripeSecrets = new TestStripeSecrets();
            var mockSlackManager = new Mock<ISlackManager>();
            var processor = new SetStripeParameters(stripeData, stripeSecrets, mockSlackManager.Object);

            // When I run the processor
            var result = await new Chain(new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(SetStripeParameters));

            // Then I expect the Stripe parameters to contain the correct values
            Assert.AreEqual(isTest ? "sk_test_" : "sk_live_", StripeConfiguration.ApiKey);
        }

        [TestMethod]
        public void CreateStripeCheckoutSession_Works_Correctly()
        {

        }

        [TestMethod]
        public async Task InsertStripeSession_Calls_StripeSessionAccessor_With_Correct_Parameters_And_Returns_Correct_Result()
        {
            // Given I have an InsertStripeSession processor with mocked injections and ShopData
            var testStripeCheckoutSessionId = TestData.TestGuid.ToString();
            var testStripeCheckoutSession = new Session
            {
                Id = testStripeCheckoutSessionId,
                ExpiresAt = TestData.TestDateTime
            };
            var testProductId = TestData.TestGuid3;
            var testPriceAsInteger = 10;
            var testCurrency = "gbp";
            var testProduct = new TestProduct { Id = testProductId, PriceAsInteger = testPriceAsInteger, Currency = "gbp" };

            var stripeData = new StripeData
            {
                CreatedDateTime = TestData.TestDateTime,
                Product = testProduct,
                StripeCheckoutSession = testStripeCheckoutSession
            };
            var mockSlackManager = new Mock<ISlackManager>();
            var mockStripeSessionAccessor = new Mock<IStripeSessionAccessor>();
            mockStripeSessionAccessor.Setup(m => m.InsertAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()
                )).Verifiable();
            var processor = new InsertStripeSession(mockStripeSessionAccessor.Object, stripeData, mockSlackManager.Object);
            var processor2 = new TestProcessor(mockSlackManager.Object);

            // When I run the processor with a successor
            var result = await new Chain(new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(InsertStripeSession),
                typeof(TestProcessor));

            // Then I expect StripeSessionAccessor.AddSpecialEntityAsync to be called with the correct StripeSession fields
            mockStripeSessionAccessor.Verify(m => m.InsertAsync(
                It.Is<string>(s => s.Equals(testStripeCheckoutSessionId)),
                It.Is<Guid?>(s => s.Equals(testProductId)),
                It.Is<int>(s => s.Equals(testPriceAsInteger)),
                It.Is<string>(s => s.Equals(testCurrency)),
                It.Is<DateTime>(s => s.Equals(TestData.TestDateTime)),
                It.Is<DateTime>(s => s.Equals(TestData.TestDateTime)),
                It.Is<DateTime>(s => s.Equals(TestData.TestDateTime))),
                Times.Once);

            // And I expect the second processor to be called
            Assert.AreEqual(nameof(TestProcessor), result.StackTrace.Last());
        }

        [TestMethod]
        public async Task RedirectToStripe_Returns_Correct_Parameters()
        {
            // Given I have a SetStripeUrls processor with mocked injections
            var httpContext = new DefaultHttpContext();
            var testStripeCheckoutSessionUrl = "testStripeCheckoutSessionUrl";
            var stripeData = new StripeData
            {
                HttpResponse = httpContext.Response,
                StripeCheckoutSession = new Session { Url = testStripeCheckoutSessionUrl }
            };
            var mockSlackManager = new Mock<ISlackManager>();
            var processor = new RedirectToStripe(stripeData, mockSlackManager.Object);
            var processor2 = new TestProcessor(mockSlackManager.Object);

            // When I run the processor with a successor
            var result = await new Chain(new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(RedirectToStripe),
                typeof(TestProcessor));

            // Then I expect the shopdata response header to contain the correct location stripe url
            Assert.AreEqual(testStripeCheckoutSessionUrl, stripeData.HttpResponse.Headers[Constants.HEADER_STRIPE_REDIRECT].Single());

            // And I expect the chaindata to contain the correct actionresult
            Assert.AreEqual(new StatusCodeResult(303).StatusCode, (stripeData.ActionResult as StatusCodeResult).StatusCode);

            // And I expect the second processor to be called
            Assert.AreEqual(nameof(TestProcessor), result.StackTrace.Last());
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ConvertStripeCallbackJson_Works_Correctly(bool isTest)
        {
            // Given I have test data
            var testCheckoutSessionId = TestData.TestGuid.ToString();
            var testCheckoutSession = new Session { Id = testCheckoutSessionId };
            var testType = "testType";
            var testJson = "testJson";
            var testEvent = new Event
            {
                Type = testType,
                Data = new EventData { Object = testCheckoutSession }
            };
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add(Constants.HEADER_STRIPE_SIGNATURE, "testSignature");
            var stripeData = new StripeData
            {
                StripeJson = testJson,
                HttpRequest = httpContext.Request,
                IsTest = isTest
            };
            var stripeSecrets = new TestStripeSecrets();

            // And I have a ConvertStripeCallbackJson processor with mocked injections
            var mockSlackManager = new Mock<ISlackManager>();
            var mockStripeEventUtility = new Mock<IStripeEventUtility>();
            mockStripeEventUtility.Setup(m => m.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(testEvent);
            var processor = new ConvertStripeCallbackJson(mockStripeEventUtility.Object, stripeData, stripeSecrets, mockSlackManager.Object);

            // When I run the processor
            var result = await new Chain(new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(ConvertStripeCallbackJson));

            // Then I expect the Stripe EventType and CheckoutSession to contain the correct values
            Assert.AreEqual(testType, stripeData.StripeEventType);
            Assert.AreEqual(testCheckoutSessionId, stripeData.StripeCheckoutSession.Id);

            // And I expect StripeEventUtility.ConstructEvent to be called once with the correct parameters
            mockStripeEventUtility.Verify(m => m.ConstructEvent(
                It.Is<string>(s => s.Equals(testJson)),
                It.Is<string>(s => s.Equals(httpContext.Request.Headers[Constants.HEADER_STRIPE_SIGNATURE])),
                It.Is<string>(s => s.Equals(isTest ? "whsec_TEST" : "whsec_LIVE"))),
                Times.Once);

            // And I expect the chainData exception to be null
            Assert.IsNull(result.Exception);
        }
    }
}
