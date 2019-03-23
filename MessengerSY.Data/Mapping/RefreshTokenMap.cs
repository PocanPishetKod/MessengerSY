using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class RefreshTokenMap : EntityConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable(nameof(RefreshToken));

            builder.Property(refreshToken => refreshToken.Token).IsRequired();

            base.Configure(builder);
        }
    }
}
