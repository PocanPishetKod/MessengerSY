using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class UserProfileChatMap : EntityConfiguration<UserProfileChat>
    {
        public override void Configure(EntityTypeBuilder<UserProfileChat> builder)
        {
            builder.ToTable(nameof(UserProfileChat));

            base.Configure(builder);
        }
    }
}
