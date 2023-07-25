using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sc.Credits.DrivenAdapters.SqlServer
{
    /// <summary>
    /// DbContext for credits database
    /// </summary>
    public class CreditsContext : DbContext
    {
        /// <summary>
        /// Credits context
        /// </summary>
        protected CreditsContext()
        {
        }

        /// <summary>
        /// Credits context
        /// </summary>
        /// <param name="options"></param>
        public CreditsContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// On configuring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            FieldInfo field = typeof(ServiceProviderCache).GetField("_configurations", BindingFlags.NonPublic | BindingFlags.Instance);
            ConcurrentDictionary<long, (IServiceProvider ServiceProvider, IDictionary<string, string> DebugInfo)> configurations =
                (ConcurrentDictionary<long, (IServiceProvider ServiceProvider, IDictionary<string, string> DebugInfo)>)field.GetValue(ServiceProviderCache.Instance);

            if (configurations.Count > 10)
            {
                configurations.Clear();
            }

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreditsContext).Assembly);

            var decimalFields = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in decimalFields)
            {
                if (string.IsNullOrEmpty(property.Relational().ColumnType))
                {
                    property.Relational().ColumnType = "decimal(26, 6)";
                }
            }

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}