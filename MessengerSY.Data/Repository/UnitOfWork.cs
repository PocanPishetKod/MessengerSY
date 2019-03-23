using MessengerSY.Core;
using MessengerSY.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MessengerDbContext _context;

        public UnitOfWork(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
