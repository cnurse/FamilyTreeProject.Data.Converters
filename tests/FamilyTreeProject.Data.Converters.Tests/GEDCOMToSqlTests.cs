using FamilyTreeProject.Common.Data;
using FamilyTreeProject.Common.Models;
using FamilyTreeProject.Data.Converters.Tests.Helpers;
using FamilyTreeProject.Data.EntityFramework;
using FamilyTreeProject.DomainServices;
using NUnit.Framework;

namespace FamilyTreeProject.Data.Converters.Tests
{
    [TestFixture]
    public class GEDCOMToSqlTests : ConverterTestBase
    {
//        [TestCase("FamilyTreeProject", "Cardwell", "The Cardwell family of Lytham")]
//        [TestCase("FamilyTreeProject", "Carter", "The Carter family of Kingswood")]
//        [TestCase("FamilyTreeProject", "Nurse", "The Nurse family of Hanham")]
//        [TestCase("FamilyTreeProject", "Harman", "The Harman family of Icklesham")]
//        [TestCase("FamilyTreeProject", "Taylor", "The Taylor family of Winsley")]
//        [Ignore("Ignore Integration Tests")]
//        [TestCase("FamilyTreeProject", "Test", "A subset of the Nurse family")]
        public void Convert_GEDCOM_To_SqlLite(string dbName, string name, string title)
        {
            using (var factory = new FamilyTreeSqlLiteContextFactory())
            {
                //Arrange
                IFileStore db = CreateStore(string.Format("{0}.ged", name));

                // Get a context
                using (var context = factory.CreateContext(GetFullPath(string.Format("{0}.db", dbName))))
                {
                    IUnitOfWork unitOfWork = new EFUnitOfWork(context);
                    IFamilyTreeServiceFactory familyTreeServiceFactory = new FamilyTreeServiceFactory(unitOfWork);

                    var gedcomConverter = new GEDCOMConverter(familyTreeServiceFactory);

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

            }
        }
        
//        [TestCase("Cardwell", "The Cardwell family of Lytham")]
//        [TestCase("Carter", "The Carter family of Kingswood")]
//        [TestCase("Nurse", "The Nurse family of Hanham")]
//        [TestCase("Harman", "The Harman family of Icklesham")]
//        [TestCase("Taylor", "The Taylor family of Winsley")]
//        [Ignore("Ignore Integration Tests")]
//        [TestCase("Test", "A subset of the Nurse family")]
        public void Convert_GEDCOM_To_SqlServer(string name, string title)
        {
            using (var factory = new FamilyTreeSqlServerContextFactory())
            {
                //Arrange
                IFileStore db = CreateStore(string.Format("{0}.ged", name));

                // Get a context
                using (var context = factory.CreateContext(ConnectionString))
                {
                    IUnitOfWork unitOfWork = new EFUnitOfWork(context);
                    IFamilyTreeServiceFactory familyTreeServiceFactory = new FamilyTreeServiceFactory(unitOfWork);

                    var gedcomConverter = new GEDCOMConverter(familyTreeServiceFactory);

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

            }
        }

    }
}