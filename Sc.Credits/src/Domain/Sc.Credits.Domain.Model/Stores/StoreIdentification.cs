namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// Store identification
    /// </summary>
    public class StoreIdentification
    {
        /// <summary>
        /// Gets the store id
        /// </summary>
        public string StoreId { get; private set; }

        /// <summary>
        /// Gets the code on legacy system
        /// </summary>
        public string ScCode { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="StoreIdentification"/>
        /// </summary>
        protected StoreIdentification()
        {
            //Need to reflection
        }

        /// <summary>
        /// Creates a new instance of <see cref="StoreIdentification"/>
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="scCode"></param>
        public StoreIdentification(string storeId, string scCode)
        {
            StoreId = storeId;
            ScCode = scCode;
        }

        /// <summary>
        /// Determines if the current identification is updated.
        /// </summary>
        /// <param name="storeRequest"></param>
        public bool SetUpdated(StoreRequest storeRequest)
        {
            bool matchScCode = ScCode == storeRequest.ScCode;
            bool update = !matchScCode;
            if (update)
            {
                ScCode = storeRequest.ScCode;
            }

            return update;
        }
    }
}