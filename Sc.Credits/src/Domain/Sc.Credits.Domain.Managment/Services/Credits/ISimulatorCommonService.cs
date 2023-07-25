using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    public interface ISimulatorCommonService
    {

        /// <summary>
        /// <see cref="ICommons"/>
        /// </summary>
        ICommons Commons { get; }
        /// <summary>
        /// <see cref="IStoreService"/>
        /// </summary>
        IStoreService StoreService { get; }


        /// <summary>
        /// Saves the record request asynchronous.
        /// </summary>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        Task SaveRecordRequestAsync(InitialValuesForIndependentSimulation initialValues);
    }
}

