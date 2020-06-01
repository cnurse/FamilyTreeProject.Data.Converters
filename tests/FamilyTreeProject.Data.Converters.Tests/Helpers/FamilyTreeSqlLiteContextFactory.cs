using System;
using System.Data.Common;
using FamilyTreeProject.Data.EntityFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeProject.Data.Converters.Tests.Helpers
{
    public class FamilyTreeSqlLiteContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<FamilyTreeContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<FamilyTreeContext>()
                .UseSqlite(_connection).Options;
        }

        public FamilyTreeContext CreateContext(string fileName)
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection($"Data Source={fileName}");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new FamilyTreeContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new FamilyTreeContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}