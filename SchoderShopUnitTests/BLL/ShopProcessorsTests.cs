using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using SchoderChain;
using SchoderShop.BLL;
using SchoderShop.Helpers;
using SchoderShopUnitTests.TestHelpers;
using Stripe;
using Stripe.Checkout;

namespace SchoderShopUnitTests.BLL
{
    [TestClass]
    public class ShopProcessorsTests
    {
        // OK typeof(GetDonationProductId),
        // OK typeof(SetStripeParameters),
        //typeof(CreateStripeCheckoutSession),
        // OK typeof(RedirectToStripe));
        //--------------------------------------
        // OK typeof(ConvertStripeCallbackJson),
        //typeof(ProcessStripePayment)); LATER

        //[DataTestMethod]
        //[DynamicData(nameof(GetDonationProductIdTestData), DynamicDataSourceType.Method)]
        //public async Task GetDonationProductId_Returns_Correct_ProductId(bool isDonation, string donationType, Guid expectedProductId)
        //{
        //    // Given I have a SetStripeUrls processor with mocked injections
        //    var shopData = new ShopData
        //    {
        //        IsDonation = isDonation,
        //        DonationType = donationType,
        //        ProductId = isDonation ? Guid.Empty : TestData.TestGuid
        //    };
        //    var chainData = new ChainData();
        //    var mockSlackManager = new Mock<ISlackManager>();
        //    var processor = new GetDonationProductId(shopData, chainData, mockSlackManager.Object);
        //    var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

        //    // When I run the processor
        //    await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
        //        typeof(GetDonationProductId),
        //        typeof(TestProcessor));

        //    // Then I expect the product id to contain the correct Guid
        //    Assert.AreEqual(expectedProductId, shopData.ProductId);

        //    // And I expect the second processor to be called if a product id was found
        //    Assert.AreEqual(expectedProductId == Guid.Empty ? nameof(GetDonationProductId) : nameof(TestProcessor), chainData.StackTrace.Last());
        //}

        //private static IEnumerable<object[]> GetDonationProductIdTestData()
        //{
        //    return new List<object[]>
        //    {
        //        new object[]{ true, Constants.CAPTION_DONATIONBUTTON1, Constants.DONATION1_GUID },
        //        new object[]{ true, Constants.CAPTION_DONATIONBUTTON2, Constants.DONATION2_GUID },
        //        new object[]{ true, Constants.CAPTION_DONATIONBUTTON3, Constants.DONATION3_GUID },
        //        new object[]{ true, string.Empty, Guid.Empty },
        //        new object[]{ false, Constants.CAPTION_DONATIONBUTTON1, TestData.TestGuid }
        //    };
        //}

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        [Obsolete]
        public async Task SetStripeParameters_Returns_Correct_Parameters(bool isTest)
        {
            // Given I have a SetStripeUrls processor with mocked injections
            var webHostEnvironment = new TestWebHostEnvironment { EnvironmentName = isTest ? Environments.Development : Environments.Production };
            var shopData = new ShopData(); // !!!! new ShopData { IsDonation = isDonation };
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var processor = new SetStripeParameters(webHostEnvironment, shopData, chainData, mockSlackManager.Object);

            // When I run the processor
            await new Chain(chainData, new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(SetStripeParameters));

            // Then I expect the Stripe parameters to contain the correct values
            Assert.AreEqual(isTest ? StripeSecrets.STRIPE_APIKEY_TEST : StripeSecrets.STRIPE_APIKEY_LIVE, StripeConfiguration.ApiKey);
            Assert.AreEqual($"{(isTest ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE)}{"Shop/Index"}", shopData.CancelUrl);
            Assert.AreEqual($"{(isTest ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE)}{"Shop/OrderThankYou"}", shopData.SuccessUrl);
        }

        //[TestMethod]
        //public async Task RedirectToStripe_Returns_Correct_Parameters()
        //{
        //    // Given I have a SetStripeUrls processor with mocked injections
        //    var httpContext = new DefaultHttpContext();
        //    var testStripeCheckoutSessionUrl = "testStripeCheckoutSessionUrl";
        //    var shopData = new ShopData
        //    {
        //        HttpResponse = httpContext.Response,
        //        StripeCheckoutSession = new Session { Url = testStripeCheckoutSessionUrl }
        //    };
        //    var chainData = new ChainData();
        //    var mockSlackManager = new Mock<ISlackManager>();
        //    var processor = new RedirectToStripe(shopData, chainData, mockSlackManager.Object);
        //    var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

        //    // When I run the processor with a successor
        //    await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
        //        typeof(RedirectToStripe),
        //        typeof(TestProcessor));

        //    // Then I expect the shopdata response header to contain the correct location stripe url
        //    Assert.AreEqual(testStripeCheckoutSessionUrl, shopData.HttpResponse.Headers[StripeSecrets.HEADER_STRIPE_REDIRECT].Single());

        //    // And I expect the chaindata to contain the correct actionresult
        //    Assert.AreEqual(new StatusCodeResult(303).StatusCode, (chainData.ActionResult as StatusCodeResult).StatusCode);

        //    // And I expect the second processor to be called
        //    Assert.AreEqual(nameof(TestProcessor), chainData.StackTrace.Last());
        //}

        //[DataTestMethod]
        //[DataRow(false)]
        //[DataRow(true)]
        //public async Task ConvertStripeCallbackJson_Works_Correctly(bool isLive)
        //{
        //    // Given I have test data
        //    var testCheckoutSessionId = TestData.TestGuid.ToString();
        //    var testCheckoutSession = new Session { Id = testCheckoutSessionId };
        //    var testType = "testType";
        //    var testJson = "testJson";
        //    var testEvent = new Event
        //    {
        //        Type = testType,
        //        Data = new EventData { Object = testCheckoutSession }
        //    };
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.Request.Headers.Add(StripeSecrets.HEADER_STRIPE_SIGNATURE, "testSignature");
        //    var shopData = new ShopData
        //    {
        //        StripeJson = testJson,
        //        HttpRequest = httpContext.Request,
        //        IsLive = isLive
        //    };
        //    var chainData = new ChainData();

        //    // And I have a ConvertStripeCallbackJson processor with mocked injections
        //    var mockSlackManager = new Mock<ISlackManager>();
        //    var mockStripeEventUtility = new Mock<IStripeEventUtility>();
        //    mockStripeEventUtility.Setup(m => m.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(testEvent);
        //    var processor = new ConvertStripeCallbackJson(mockStripeEventUtility.Object, shopData, chainData, mockSlackManager.Object);

        //    // When I run the processor
        //    await new Chain(chainData, new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(ConvertStripeCallbackJson));

        //    // Then I expect the Stripe EventType and CheckoutSession to contain the correct values
        //    Assert.AreEqual(testType, shopData.StripeEventType);
        //    Assert.AreEqual(testCheckoutSessionId, shopData.StripeCheckoutSession.Id);

        //    // And I expect StripeEventUtility.ConstructEvent to be called once with the correct parameters
        //    mockStripeEventUtility.Verify(m => m.ConstructEvent(It.Is<string>(s => s.Equals(testJson)), It.Is<string>(s => s.Equals(httpContext.Request.Headers[StripeSecrets.HEADER_STRIPE_SIGNATURE])), It.Is<string>(s => s.Equals(isLive ? StripeSecrets.STRIPE_SECRET_LIVE : StripeSecrets.STRIPE_SECRET_TEST))), Times.Once);

        //    // And I expect the chainData exception to be null
        //    Assert.IsNull(chainData.Exception);
        //}
    }
}
