using System;
using FamilyTreeProject.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeProject.Data.Converters.Tests.Helpers
{
    public class FamilyTreeSqlServerContextFactory : IDisposable
    {

        private DbContextOptions<FamilyTreeContext> CreateOptions(string connectionString)
        {
            return new DbContextOptionsBuilder<FamilyTreeContext>()
                .UseSqlServer(connectionString).Options;
        }

        public FamilyTreeContext CreateContext(string connectionString)
        {
            var context = new FamilyTreeContext(CreateOptions(connectionString));
            context.Database.EnsureCreated();
            return context;
        }

        public void Dispose()
        {
        }
    }
}