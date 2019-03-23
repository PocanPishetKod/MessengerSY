using MessengerSY.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class EntityConfiguration<TEntity> : IMappangConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public EntityConfiguration()
        {

        }

        public void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            
        }
    }
}
