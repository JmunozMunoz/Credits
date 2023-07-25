namespace Sc.Credits.Helper.Test.Model
{
    using Sc.Credits.Domain.Model.Credits;
    using Sc.Credits.Domain.Model.Enums;

    /// <summary>
    ///Profile helper test
    /// </summary>
    public static class StatusHelperTest
    {
        /// <summary>
        /// Get active status
        /// </summary>
        /// <returns></returns>
        public static Status GetActiveStatus()
        {
            return new Status("Active").SetId((int)Statuses.Active);
        }

        /// <summary>
        /// Get canceled status
        /// </summary>
        /// <returns></returns>
        public static Status GetCalceledStatus()
        {
            return new Status("Canceled").SetId((int)Statuses.Canceled);
        }

        /// <summary>
        /// Get paid status
        /// </summary>
        /// <returns></returns>
        public static Status GetPaidStatus()
        {
            return new Status("Paid").SetId((int)Statuses.Paid);
        }

        /// <summary>
        /// Get cancel request status
        /// </summary>
        /// <returns></returns>
        public static Status GetCancelRequestStatus()
        {
            return new Status("Cancel Request").SetId((int)Statuses.CancelRequest);
        }

        /// <summary>
        /// Get status by type
        /// </summary>
        /// <returns></returns>
        public static Status GetStatusByType(Statuses status)
        {
            return new Status(GetStatusName((int)status)).SetId((int)status);
        }

        /// <summary>
        /// Get status name
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public static string GetStatusName(int statusId)
        {
            if (statusId == (int)Statuses.Active) return "Active";
            if (statusId == (int)Statuses.CancelRequest) return "Cancel Request";
            return string.Empty;
        }
    }
}