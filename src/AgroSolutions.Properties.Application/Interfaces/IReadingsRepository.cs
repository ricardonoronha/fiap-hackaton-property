public interface IReadingsRepository
{
    Task<List<SensorReadingDto>> FetchReadingsAsync(
        IReadOnlyCollection<string> fieldIds,
        int LookbackInSeconds,
        CancellationToken ct);
}