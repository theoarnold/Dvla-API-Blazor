namespace Dvla.Services
{
    public interface IDvlaService
    {
        Task<IEnumerable<DvlaRes?>> GetGitHubBranchesAsync(string registration);
    }
}
