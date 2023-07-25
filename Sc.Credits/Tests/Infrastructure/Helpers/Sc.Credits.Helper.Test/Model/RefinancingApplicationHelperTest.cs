using Sc.Credits.Domain.Model.Refinancings;
using System;

namespace Sc.Credits.Helper.Test.Model
{
    public static class RefinancingApplicationHelperTest
    {
        public static RefinancingApplication GetDefault(bool allowRefinancingCredits = false) =>
            new RefinancingApplication()
            .Init(name: "Sistema de prueba", allowRefinancingCredits)
            .SetCreationRange(from: DateTime.MinValue, to: DateTime.MaxValue)
            .SetArrearsRange(from: 0, to: int.MaxValue)
            .SetValueRange(from: 0.0M, to: decimal.MaxValue);
    }
}