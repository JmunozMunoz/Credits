namespace Sc.Credits.Helpers.Commons.Logging
{
    /// <summary>
    /// Logger service
    /// </summary>
    public interface ILoggerService<T> : IAppLogger, IEventsLogger
    {
    }
}