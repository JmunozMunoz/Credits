namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit builder contract
    /// </summary>
    public interface ICreditBuilder : ICreditInitBuilder, ICreditPostInitBuilder, ICreditCompleteBuilder
    {
    }
}