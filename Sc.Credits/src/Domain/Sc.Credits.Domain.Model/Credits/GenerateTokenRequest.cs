using Newtonsoft.Json;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The generate token request entity
    /// </summary>
    public class GenerateTokenRequest
    {
        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the months
        /// </summary>
        public int Months { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the request source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets the is refinancing indicator
        /// </summary>

        [JsonIgnore]
        public bool IsRefinancing { get; private set; }

        /// <summary>
        /// Sets the refinancing indicator and store
        /// </summary>
        public GenerateTokenRequest SetRefinancing(CredinetAppSettings credinetAppSettings)
        {
            StoreId = RefinancingParams.StoreId(credinetAppSettings);
            IsRefinancing = true;

            return this;
        }
    }
}