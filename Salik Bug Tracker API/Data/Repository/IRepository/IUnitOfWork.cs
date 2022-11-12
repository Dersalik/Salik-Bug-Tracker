namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IUnitOfWork
    {
        Task Save();
        IBugDeveloperRepository bugDeveloperRepository { get; }
        IBugRepository bugRepository { get; }
        IModuleRepository moduleRepository { get; }
        IModuleUserRepository moduleUserRepository { get; }
        IProjectRepository projectRepository { get; }
        ISkillRepository skillRepository { get; }
        IUserRepository userRepository { get; }
    }
}
