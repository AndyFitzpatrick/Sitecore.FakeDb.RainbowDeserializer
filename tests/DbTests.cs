using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RainbowSerialization.Tests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void DeserializeDefault_HabitatHome_NotNull()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsNotNull(home);
            }
        }

        [TestMethod]
        public void DeserializeDefault_HabitatHome_CorrectID()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsNotNull(home.ID == new Sitecore.Data.ID(Ids.Habitat));
            }
        }

        [TestMethod]
        public void DeserializeDefault_HabitatHome_CorrectTitle()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsNotNull(home["Title"] == "About Habitat");
            }
        }

        [TestMethod]
        public void DeserializeDefault_FindHabitatTemplate()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsNotNull(home.TemplateID == new Sitecore.Data.ID(Ids.HabitatTemplate));
            }
        }
    }
}