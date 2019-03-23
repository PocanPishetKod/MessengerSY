using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public interface IMappangConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
