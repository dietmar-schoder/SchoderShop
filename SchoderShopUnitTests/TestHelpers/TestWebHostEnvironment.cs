using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace SchoderShopUnitTests.TestHelpers
{
    [Obsolete]
    public class TestWebHostEnvironment : IHostingEnvironment, Microsoft.Extensions.Hosting.IHostingEnvironment, IWebHostEnvironment
    {
        public string EnvironmentName { get; set; }

        public string ApplicationName { get; set; }

        public string WebRootPath { get; set; }

        public IFileProvider WebRootFileProvider { get; set; }

        public string ContentRootPath { get; set; }

        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
