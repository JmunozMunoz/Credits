using Moq;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.UseCase.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Helper.Test.Model
{
    public static class SimulatorCommonsHelperTest
    {
        public static SimulatorCommons Create(Mock<ICreditUsesCase> simulatorUseCase,
    Mock<ISimulatorCommonService> simulatorCommonsService) =>
    new SimulatorCommons(simulatorUseCase.Object,simulatorCommonsService.Object);
    }
}
