using Microsoft.AspNetCore.Http;
using Moq;
using SchoderChain;
using SchoderShop.BLL;
using SchoderShop.BLL.StripeCallback;
using SchoderShop.BLL.StripeSession;
using SchoderShop.DAL;
using SchoderShop.Helpers;
using SchoderShopUnitTests.TestHelpers;
using Stripe.Checkout;
using System.Text;

namespace SchoderShopUnitTests.BLL.StripeCallback
{
    [TestClass]
    public class StripeCallbackTests
    {
        [TestMethod]
        public async Task InsertStripeCallback_Calls_StripeCallbackAccessor_With_Correct_Parameters()
        {
            // Given I have an InsertStripeCallback processor with mocked injections and a Stripe Json callback content
            var testJson = "testJson";
            var httpContext = new DefaultHttpContext();
            byte[] byteArray = Encoding.ASCII.GetBytes(testJson);
            httpContext.Request.Body = new MemoryStream(byteArray);
            var stripeData = new StripeData { HttpRequest = httpContext.Request };
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var mockStripeCallbackAccessor = new Mock<IStripeCallbackAccessor>();
            mockStripeCallbackAccessor.Setup(m => m.InsertAsync(It.IsAny<string>())).Verifiable();
            var processor = new InsertStripeCallback(mockStripeCallbackAccessor.Object, stripeData, chainData, mockSlackManager.Object);

            // When I run the processor
            await new Chain(chainData, new List<IProcessor> { processor }).ProcessAsync(string.Empty, typeof(InsertStripeCallback));

            // Then I expect the StripeCallbackAccessor.AddStripeCallbackAsync to be called with the Stripe Json callback content
            mockStripeCallbackAccessor.Verify(m => m.InsertAsync(It.Is<string>(s => s.Equals(testJson))), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStripeSession_Calls_StripeSessionAccessor_With_Correct_Parameters_And_Returns_Correct_Result()
        {
            // Given I have an UpdateStripeSession processor with mocked injections and ShopData
            var testStripeCheckoutSessionId = TestData.TestGuid.ToString();
            var testStripeCheckoutSession = new Session { Id = testStripeCheckoutSessionId };
            var testStripeEventType = "StripeEventType";

            var stripeData = new StripeData
            {
                StripeCheckoutSession = testStripeCheckoutSession,
                StripeEventType = testStripeEventType
            };
            var chainData = new ChainData();
            var mockSlackManager = new Mock<ISlackManager>();
            var mockDateTimeFactory = new Mock<IDateTimeFactory>();
            mockDateTimeFactory.Setup(m => m.UtcNow).Returns(TestData.TestDateTime);
            var mockStripeSessionAccessor = new Mock<IStripeSessionAccessor>();
            mockStripeSessionAccessor.Setup(m => m.UpdateEventTypeAsync(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var processor = new UpdateStripeSession(mockStripeSessionAccessor.Object, stripeData, chainData, mockSlackManager.Object);
            var processor2 = new TestProcessor(chainData, mockSlackManager.Object);

            // When I run the processor with a successor
            await new Chain(chainData, new List<IProcessor> { processor, processor2 }).ProcessAsync(string.Empty,
                typeof(UpdateStripeSession),
                typeof(TestProcessor));

            // Then I expect StripeSessionAccessor.UpdateEventTypeAsync to be called with the correct parameters
            mockStripeSessionAccessor.Verify(m => m.UpdateEventTypeAsync(
                It.Is<string>(s => s.Equals(testStripeCheckoutSessionId)),
                It.Is<string>(s => s.Equals(testStripeEventType))
                ), Times.Once);

            // And I expect the second processor to be called
            Assert.AreEqual(nameof(TestProcessor), chainData.StackTrace.Last());
        }
    }
}
