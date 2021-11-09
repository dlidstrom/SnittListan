﻿#nullable enable

namespace Snittlistan.Web.Infrastructure.Database
{
    using System.Data.Entity;
    using Npgsql.NameTranslation;

    public class SnittlistanContext : DbContext
    {
        public DbSet<DelayedTask> DelayedTasks { get; set; } = null!;

        public DbSet<PublishedTask> PublishedTasks { get; set; } = null!;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Database.SetInitializer(new NullDatabaseInitializer<SnittlistanContext>());
            NpgsqlSnakeCaseNameTranslator mapper = new();
            _ = modelBuilder.HasDefaultSchema("snittlistan");
            modelBuilder.Properties().Configure(x => x.HasColumnName(mapper.TranslateMemberName(x.ClrPropertyInfo.Name)));
            modelBuilder.Types().Configure(x => x.ToTable(mapper.TranslateMemberName(x.ClrType.Name)));
        }
    }
}
