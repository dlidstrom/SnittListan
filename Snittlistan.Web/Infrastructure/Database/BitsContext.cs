﻿#nullable enable

using Npgsql.NameTranslation;

namespace Snittlistan.Web.Infrastructure.Database;

public class BitsContext : DbContext, IBitsContext
{
    public BitsContext()
    {
        Configuration.AutoDetectChangesEnabled = false;
        Configuration.LazyLoadingEnabled = false;
    }

    public IDbSet<Bits_Team> Team { get; set; } = null!;

    public IDbSet<Bits_HallRef> HallRef { get; set; } = null!;

    public IDbSet<Bits_Hall> Hall { get; set; } = null!;

    public IDbSet<Bits_Match> Match { get; set; } = null!;

    public IDbSet<Bits_TeamRef> TeamRef { get; set; } = null!;

    public IDbSet<Bits_OilProfile> OilProfile { get; set; } = null!;

    public IDbSet<Bits_VMatchHeadInfo> VMatchHeadInfo { get; set; } = null!;

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        System.Data.Entity.Database.SetInitializer(new NullDatabaseInitializer<BitsContext>());
        NpgsqlSnakeCaseNameTranslator mapper = new();
        _ = modelBuilder.HasDefaultSchema("bits");
        modelBuilder.Properties().Configure(x => x.HasColumnName(
            mapper.TranslateMemberName(x.ClrPropertyInfo.Name)));
        modelBuilder.Types().Configure(x => x.ToTable(
            mapper.TranslateMemberName(x.ClrType.Name.Replace("Bits_", string.Empty))));

        //.WithRequiredPrincipal(x => x.Hall!);
        //            .Map(x => x.MapKey("hall_id"));
        //_ = modelBuilder.Entity<Bits_HallRef>()
        //    .HasOptional(x => x.Hall)
        //    .WithOptionalDependent(x => x!.HallRef)
        //    .Map(x => x.MapKey("hall_id"));

        //_ = modelBuilder.Entity<Bits_Hall>
    }
}
