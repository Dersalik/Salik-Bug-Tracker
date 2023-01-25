using Salik_Bug_Tracker_API.Data.Repository.IRepository;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            bugDeveloperRepository=new BugDeveloperRepository(db);
            bugRepository=new BugRepository(db);
            moduleRepository=new ModuleRepository(db);  
            projectRepository=new ProjectRepository(db);
            skillRepository=new SkillRepository(db);
            userRepository=new UserRepository(db);
            moduleUserRepository=new ModuleUserRepository(db);
        }

        public IBugDeveloperRepository bugDeveloperRepository { get; private set; }
        public IBugRepository bugRepository { get; private set; }
        public IModuleRepository moduleRepository { get; private set; }
        public IProjectRepository projectRepository { get; private set; }
        public ISkillRepository skillRepository { get; private set; }
        public IUserRepository userRepository { get; private set; }
        public IModuleUserRepository moduleUserRepository { get; private set; }

        public async Task<int> Save()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
