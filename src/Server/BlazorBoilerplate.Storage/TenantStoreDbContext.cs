﻿using BlazorBoilerplate.Shared;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BlazorBoilerplate.Storage
{
    public class TenantStoreDbContext : EFCoreStoreDbContext
    {
        public IConfiguration Configuration { get; }        
        public static readonly TenantInfo DefaultTenant = new TenantInfo(Settings.DefaultTenantId, Settings.DefaultTenantId, Settings.DefaultTenantId, null, null);

        public TenantStoreDbContext(DbContextOptions<TenantStoreDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TenantInfo>()
                .Property(t => t.ConnectionString)
                .IsRequired(false);
            modelBuilder.Entity<TenantInfo>()
                .HasData(DefaultTenant);
        }
    }
}