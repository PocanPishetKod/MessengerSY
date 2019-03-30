using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class ChatMap : EntityConfiguration<Chat>
    {
        public override void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable(nameof(Chat));

            builder.Property(chat => chat.Title).HasMaxLength(50);

            builder.HasMany(chat => chat.Messages)
                .WithOne(message => message.Chat)
                .HasForeignKey(message => message.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(chat => chat.Participants)
                .WithOne(participant => participant.Chat)
                .HasForeignKey(participant => participant.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(chat => chat.LastMessage)
                .WithOne(message => message.ThisMessageLastInChat)
                .HasForeignKey<Message>(message => message.ThisMessageLastInChatId)
                .OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }
}
