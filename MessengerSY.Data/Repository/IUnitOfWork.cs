using MessengerSY.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Data.Repository
{
    public interface IUnitOfWork
    {
        Task<int> Commit();
    }
}
