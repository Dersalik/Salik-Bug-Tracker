﻿using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository.IRepository
{
    public interface IBugDeveloperRepository:IRepository<BugDeveloper>
    {
        Task<IEnumerable<ApplicationUser>> GetDevelopersByBugId(int bugId);

    }
}
