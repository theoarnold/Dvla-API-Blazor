namespace Dvla.Services
{
    public interface IDvlaService
    {
        Task<IEnumerable<DvlaRes>?> GetDvlaAsync(string registration);
    }
}
