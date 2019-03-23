using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class MessageMap : EntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable(nameof(Message));

            builder.Property(message => message.MessageText)
                .IsRequired()
                .HasMaxLength(500);

            base.Configure(builder);
        }
    }
}
