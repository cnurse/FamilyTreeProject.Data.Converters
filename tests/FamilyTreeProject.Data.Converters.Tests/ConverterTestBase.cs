using System.IO;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Data.GEDCOM;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace FamilyTreeProject.Data.Converters.Tests
{
    public class ConverterTestBase
    {
        protected IConfigurationRoot _configuration;

        protected string ConnectionString => _configuration["sqlServerConnectionString"];
        protected string FilePath { get; private set; }
        protected string CosmosDB_Endpoint => _configuration["CosmosDB:Endpoint"];
        protected string CosmosDB_Key => _configuration["CosmosDB:Key"];
        protected string CosmosDB_DatabaseId => _configuration["CosmosDB:DatabaseId"];
        protected string CosmosDB_CollectionId => _configuration["CosmosDB:CollectionId"];
        
        [OneTimeSetUp]
        public void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json");

            var solutionDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory)));

           
            _configuration = builder.Build();
            
            FilePath = Path.Combine(solutionDir, _configuration["filePath"]);
        }

        protected string GetFullPath(string file)
        {
            return Path.Combine(FilePath, file);
        }
        
        protected IFileStore CreateStore(string file)
        {
            return new GEDCOMFileStore(GetFullPath(file));
        }
    }
}