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
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var mockProductAccessor = new Mock<IProductAccessor>();
            mockProductAccessor.Setup(m => m.GetAsync(It.IsAny<Guid>())).ReturnsAsync(productIsFound ? testProduct : null);
            var processor = new GetProduct(mockProductAccessor.Object, stripeData, chainData, mockSlackManager.Object);
            var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

            // When I run the processor with a successor
            await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(GetProduct),
                typeof(TestProcessor));

            // Then I expect the StripeCallbackAccessor.AddStripeCallbackAsync to be called with the Stripe Json callback content
            mockProductAccessor.Verify(m => m.GetAsync(It.Is<Guid>(s => s.Equals(testProductId))), Times.Once);

            // And I expect the second processor to be called if a product id was found
            Assert.AreEqual(productIsFound ? nameof(TestProcessor) : nameof(GetProduct), chainData.StackTrace.Last());
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [Obsolete]
        public async Task SetStripeParameters_Returns_Correct_Parameters(bool isTest)
        {
            // Given I have a SetStripeUrls processor with mocked injections
            var stripeData = new StripeData { IsTest = isTest }; // !!!! new ShopData { IsDonation = isDonation };
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var processor = new SetStripeParameters(stripeData, chainData, mockSlackManager.Object);

            // When I run the processor
            await new Chain(chainData, new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(SetStripeParameters));

            // Then I expect the Stripe parameters to contain the correct values
            Assert.AreEqual(isTest ? StripeSecrets.STRIPE_APIKEY_TEST : StripeSecrets.STRIPE_APIKEY_LIVE, StripeConfiguration.ApiKey);
            Assert.AreEqual($"{(isTest ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE)}{"Shop/Index"}", stripeData.CancelUrl);
            Assert.AreEqual($"{(isTest ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE)}{"Shop/OrderThankYou"}", stripeData.SuccessUrl);
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
                Product = testProduct,
                StripeCheckoutSession = testStripeCheckoutSession
            };
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var mockDateTimeFactory = new Mock<IDateTimeFactory>();
            mockDateTimeFactory.Setup(m => m.UtcNow).Returns(TestData.TestDateTime);
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
            var processor = new InsertStripeSession(mockStripeSessionAccessor.Object, mockDateTimeFactory.Object, stripeData, chainData, mockSlackManager.Object);
            var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

            // When I run the processor with a successor
            await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
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
            Assert.AreEqual(nameof(TestProcessor), chainData.StackTrace.Last());
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
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var processor = new RedirectToStripe(stripeData, chainData, mockSlackManager.Object);
            var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

            // When I run the processor with a successor
            await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(RedirectToStripe),
                typeof(TestProcessor));

            // Then I expect the shopdata response header to contain the correct location stripe url
            Assert.AreEqual(testStripeCheckoutSessionUrl, stripeData.HttpResponse.Headers[StripeSecrets.HEADER_STRIPE_REDIRECT].Single());

            // And I expect the chaindata to contain the correct actionresult
            Assert.AreEqual(new StatusCodeResult(303).StatusCode, (chainData.ActionResult as StatusCodeResult).StatusCode);

            // And I expect the second processor to be called
            Assert.AreEqual(nameof(TestProcessor), chainData.StackTrace.Last());
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
            httpContext.Request.Headers.Add(StripeSecrets.HEADER_STRIPE_SIGNATURE, "testSignature");
            var stripeData = new StripeData
            {
                StripeJson = testJson,
                HttpRequest = httpContext.Request,
                IsTest = isTest
            };
            var chainData = new ChainData();

            // And I have a ConvertStripeCallbackJson processor with mocked injections
            var mockSlackManager = new Mock<ISlackManager>();
            var mockStripeEventUtility = new Mock<IStripeEventUtility>();
            mockStripeEventUtility.Setup(m => m.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(testEvent);
            var processor = new ConvertStripeCallbackJson(mockStripeEventUtility.Object, stripeData, chainData, mockSlackManager.Object);

            // When I run the processor
            await new Chain(chainData, new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(ConvertStripeCallbackJson));

            // Then I expect the Stripe EventType and CheckoutSession to contain the correct values
            Assert.AreEqual(testType, stripeData.StripeEventType);
            Assert.AreEqual(testCheckoutSessionId, stripeData.StripeCheckoutSession.Id);

            // And I expect StripeEventUtility.ConstructEvent to be called once with the correct parameters
            mockStripeEventUtility.Verify(m => m.ConstructEvent(
                It.Is<string>(s => s.Equals(testJson)),
                It.Is<string>(s => s.Equals(httpContext.Request.Headers[StripeSecrets.HEADER_STRIPE_SIGNATURE])),
                It.Is<string>(s => s.Equals(isTest ? StripeSecrets.STRIPE_SECRET_LIVE : StripeSecrets.STRIPE_SECRET_TEST))),
                Times.Once);

            // And I expect the chainData exception to be null
            Assert.IsNull(chainData.Exception);
        }
    }
}
