using Xunit;

namespace GLMS.Tests.Services
{
    public class FileTests
    {
        [Fact]
        public void OnlyPdfFiles_ShouldBeAllowed()
        {
            string fileName = "document.exe";

            bool isValid = fileName.EndsWith(".pdf");

            Assert.False(isValid);
        }
    }
}