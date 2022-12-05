using SchoderChain;

namespace SchoderShopUnitTests.TestHelpers
{
    public class TestProcessor : Processor
    {
        public TestProcessor(ChainData chainData, ISlackManager slackManager) : base(chainData, slackManager) { }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            return true;
        }
    }
}
