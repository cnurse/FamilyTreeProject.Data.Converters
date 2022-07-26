using System;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using FamilyTreeProject.Data.CosmosDB;
using FamilyTreeProject.DomainServices;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace FamilyTreeProject.Data.Converters.Tests
{
    public class GEDCOMToCosmosDBTests : ConverterTestBase
    {
        [TestCase("Cardwell", "The Cardwell Family of Kirkham, Lancashire", "charlesnurse@hotmail.com")]
        [TestCase("Nurse", "The Nurse Family of Hanham, Gloucestershire", "charlesnurse@gmail.com")]
        [TestCase("Taylor", "The Taylor Family of Winsley, Wiltshire", "charlesnurse@gmail.com")]
        public void Convert_GEDCOM_To_CosmosDB(string name, string title, string owner)
        {
            //Arrange
            IUnitOfWork unitOfWork = CreateUnitOfWork();
            IFamilyTreeServiceFactory familyTreeServiceFactory = new FamilyTreeServiceFactory(unitOfWork);
            var gedcomConverter = new GEDCOMConverter(familyTreeServiceFactory);

            IFileStore db = CreateStore(string.Format("{0}.ged", name));
            
            var tree = new Tree()
            {
                Name = name, 
                OwnerId = owner,
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