using MessengerSY.Core.Domain;
using MessengerSY.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MessengerSY.Data.Context
{
    public class MessengerDbContext : DbContext
    {
        private readonly string connectLocal =
            "Server=(localdb)\\mssqllocaldb;Database=messengerSY;Trusted_Connection=True;";

        private readonly string connectHosting = "Data Source=localhost;Database=u0685177_messengerSY;Integrated Security=False;User ID=u0685177_messengerSY;Password=Vfhcbr";

        private readonly string _connectionString;

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserProfileChat> UserProfileChats { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public MessengerDbContext(DbContextOptions<MessengerDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var configurationTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType.GetGenericTypeDefinition() == typeof(EntityConfiguration<>));

            foreach (var configurationType in configurationTypes)
            {
                var configuration = (IMappangConfiguration) Activator.CreateInstance(configurationType);
                configuration.ApplyConfiguration(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
