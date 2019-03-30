using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class ContactMap : EntityConfiguration<Contact>
    {
        public override void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable(nameof(Contact));

            builder.Property(prop => prop.ContactName).HasMaxLength(50);


            base.Configure(builder);
        }
    }
}
