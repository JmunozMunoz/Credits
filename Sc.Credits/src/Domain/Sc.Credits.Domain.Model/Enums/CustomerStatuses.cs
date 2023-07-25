namespace Sc.Credits.Domain.Model.Enums
{
    /// <summary>
    /// Customer statuses enumerator
    /// </summary>
    public enum CustomerStatuses
    {
        Approved = 1,
        Study = 2,
        Pending = 3,
        Denied = 4,
        StudyAgent = 5,
        BusinessException = 6,
        IncompleteRequest = 7
    }
}