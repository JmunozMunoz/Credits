using Sc.Credits.Domain.Model.Sequences;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Sequence Helper Test
    /// </summary>
    public static class SequenceHelperTest
    {
        /// <summary>
        /// Get Sequence List
        /// </summary>
        /// <returns></returns>
        public static List<Sequence> GetSequenceList()
        {
            return new List<Sequence>
            {
                new Sequence(Guid.NewGuid(), 1, "55ftges586s5f45s5a", "Credits"),
                new Sequence(Guid.NewGuid(), 2, "55ftges586s5f45s5a", "Credits"),
                new Sequence(Guid.NewGuid(), 1, "5ce5a1ae195f4428203a1d9e", "Credits")
            };
        }
    }
}