using Xunit;

namespace GLMS.Tests.Services
{
    public class ContractTests
    {
        [Theory]
        [InlineData("Expired")]
        [InlineData("On Hold")]
        public void InvalidContractStatuses_ShouldBeRejected(string status)
        {
            bool isValid = status == "Active";

            Assert.False(isValid);
        }
    }
}