using System.IO;
using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using FamilyTreeProject.Data.Json;
using FamilyTreeProject.DomainServices;
using NUnit.Framework;

namespace FamilyTreeProject.Data.Converters.Tests
{
    [TestFixture]
    //[Ignore("Ignore Integration Tests")]
    public class GEDCOMToJsonTests : ConverterTestBase
    {
        [TestCase("Test", "A subset of the Nurse family")]
        public void Convert_GEDCOM_To_Json(string name, string title)
        {
            //Arrange
            IUnitOfWork unitOfWork = CreateUnitOfWork(string.Format("{0}.json", name));
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

        private IUnitOfWork CreateUnitOfWork(string file)
        {
            return new JsonUnitOfWork(Path.Combine(FilePath, file));
        }
    }
}