using SchoderChain;

namespace SchoderShopUnitTests.TestHelpers
{
    public class TestProcessor : Processor
    {
        public TestProcessor(ISlackManager slackManager) : base(slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            return true;
        }
    }
}
