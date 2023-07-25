using Moq;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Tests.Entities.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Credits.Queries.Reading;
using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Credits
{
    public class CreditRequestAgentAnalysisServiceTest
    {
        private readonly Mock<ICreditRequestAgentAnalysisQueryRepository> _creditRequestAgentAnalysisRepository = new Mock<ICreditRequestAgentAnalysisQueryRepository>();
        private CreditRequestAgentAnalysisService _creditRequestAgentAnalysisService;

        public CreditRequestAgentAnalysisServiceTest()
        {
            _creditRequestAgentAnalysisService = new CreditRequestAgentAnalysisService(_creditRequestAgentAnalysisRepository.Object);
        }

        [Fact]
       public async Task GetCreditByCustomerTransaction()
       {
            //Arrange
            string transactionId = "00BF5060-0247-42F3-BEBD-9B8AFF393A5E";
            CreditRequestAgentAnalysis creditRequestAgentAnalysis = new CreditRequestAgentAnalysisTestBuilder().Build();
            IEnumerable<Field> fields =  CreditRequestAgentAnalysisReadingFields.ResultInfo;

            _creditRequestAgentAnalysisRepository.Setup(mock => mock.GetCreditByCustomerTransaction(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(creditRequestAgentAnalysis).Verifiable();

            //Act
            var response = await _creditRequestAgentAnalysisService.GetCreditRequestById(transactionId, fields);

            //Assert
            _creditRequestAgentAnalysisRepository.Verify(mock => mock.GetCreditByCustomerTransaction(It.IsAny<string>(), It.IsAny<IEnumerable<Field>>()), Times.Once);
        }
    }
}
