﻿using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data.Repository
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        private ApplicationDbContext _db;
        public ModuleRepository(ApplicationDbContext db) : base(db)
        {
            _db= db;

        }

        public async Task<bool> CheckProjectExists(int Id)
        {
            return await _db.modules.AnyAsync(d => d.Id == Id);
        }

       
    }
}