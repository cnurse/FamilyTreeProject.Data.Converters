using System;
using System.IO;
using System.Threading.Tasks;
using FamilyTreeProject.Core;
using FamilyTreeProject.Core.Data;
using FamilyTreeProject.Data.Common;
using FamilyTreeProject.Data.CosmosDB;
using FamilyTreeProject.Data.Json;
using FamilyTreeProject.DomainServices;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace FamilyTreeProject.Data.Converters.Tests
{
    public class GEDCOMToCosmosDBTests : ConverterTestBase
    {
        [TestCase("Test", "A subset of the Nurse family")]
        public void Convert_GEDCOM_To_CosmosDB(string name, string title)
        {
            //Arrange
            IUnitOfWork unitOfWork = CreateUnitOfWork();
            IFamilyTreeServiceFactory familyTreeServiceFactory = new FamilyTreeServiceFactory(unitOfWork);
            var gedcomConverter = new GEDCOMConverter(familyTreeServiceFactory);

            IFileStore db = CreateStore(string.Format("{0}.ged", name));
            
            var tree = new Tree()
            {
                Name = name, 
                OwnerId = "test",
                Title = title
            };

            //Act
            gedcomConverter.Import(db, tree, true);
        
            unitOfWork.Commit();
        }

        private IUnitOfWork CreateUnitOfWork()
        {
            return new CosmosDBUnitOfWork(new DocumentClient(new Uri(CosmosDB_Endpoint), CosmosDB_Key), CosmosDB_DatabaseId, CosmosDB_CollectionId);
        }
        
 
    }
}