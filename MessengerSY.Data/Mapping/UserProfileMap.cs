using MessengerSY.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Mapping
{
    public class UserProfileMap : EntityConfiguration<UserProfile>
    {
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable(nameof(UserProfile));

            builder.Property(userProfile => userProfile.PhoneNumber).IsRequired();
            builder.Property(userProfile => userProfile.Nickname).HasMaxLength(30);

            builder.HasMany(userProfile => userProfile.Contacts)
                .WithOne(contact => contact.FirstSide)
                .HasForeignKey(contact => contact.FirstSideId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(userProfile => userProfile.AddedMe)
                .WithOne(contact => contact.SecondSide)
                .HasForeignKey(contact => contact.SecondSideId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(userProfile => userProfile.Chats)
                .WithOne(chat => chat.UserProfile)
                .HasForeignKey(chat => chat.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(userProfile => userProfile.CreatedChats)
                .WithOne(createdChat => createdChat.Creator)
                .HasForeignKey(createdChat => createdChat.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(userProfile => userProfile.SendMessages)
                .WithOne(sendMessage => sendMessage.Sender)
                .HasForeignKey(sendMessage => sendMessage.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(userProfile => userProfile.RefreshTokens)
                .WithOne(refreshToken => refreshToken.UserProfile)
                .HasForeignKey(refreshToken => refreshToken.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
