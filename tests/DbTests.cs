using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainbowSerialization;
using Sitecore.FakeDb;

namespace RainbowSerialization.Tests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void Default_HabitatHome_NotNull()
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
        public void Default_HabitatHome_CorrectID()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsTrue(home.ID == new Sitecore.Data.ID(Ids.Habitat));
            }
        }

        [TestMethod]
        public void Default_HabitatHome_CorrectTitle()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var home = db.GetItem(Ids.Habitat);

                // Assert
                Assert.IsTrue(home["Title"] == "About Habitat");
            }
        }

        [TestMethod]
        public void Default_HabitatHomeTemplate_NotNull()
        {
            // Arrange
            using (var db = Dbs.Default())
            {

                // Act
                var homeTemplate = db.GetItem(Ids.HabitatTemplate);

                // Assert
                Assert.IsNotNull(homeTemplate);
            }
        }

        [TestMethod]
        public void ContentOnly_Login_NotNull()
        {
            // Arrange
            using (var db = Dbs.ContentOnly())
            {

                // Act
                var login = db.GetItem(Ids.Login);

                // Assert
                Assert.IsNotNull(login);
            }
        }

        [TestMethod]
        public void ContentOnly_Login_CorrectTitle()
        {
            // Arrange
            using (var db = Dbs.ContentOnly())
            {

                // Act
                var login = db.GetItem(Ids.Login);

                // Assert
                Assert.IsTrue(login["Title"] == "Login");
            }
        }

        [TestMethod]
        public void TemplatesOnly_HomeTemplate_NotNull()
        {
            // Arrange
            using (var db = Dbs.TemplatesOnly())
            {

                // Act
                var template = db.GetItem(Ids.HomeTemplate);

                // Assert
                Assert.IsNotNull(template);
            }
        }

        [TestMethod]
        public void TemplatesOnly_HomeTemplate_CorrectName()
        {
            // Arrange
            using (var db = Dbs.TemplatesOnly())
            {

                // Act
                var template = db.GetItem(Ids.HomeTemplate);

                // Assert
                Assert.IsTrue(template.Name == "Home");
            }
        }

        [TestMethod]
        public void ContentFile_AboutHabitat_NotNull()
        {
            // Arrange
            using (var db = Dbs.ContentFile())
            {

                // Act
                var page = db.GetItem(Ids.AboutHabitat);

                // Assert
                Assert.IsNotNull(page);
            }
        }

        [TestMethod]
        public void ContentFile_AboutHabitat_NoRenderings()
        {
            // Arrange
            using (var db = Dbs.ContentFile())
            {

                // Act
                var page = db.GetItem(Ids.AboutHabitat);

                // Assert
                Assert.IsTrue(string.IsNullOrEmpty(page[Sitecore.FieldIDs.LayoutField]));
            }
        }

        [TestMethod]
        public void ContentAndTemplateFiles_AboutHabitat_HasRenderings()
        {
            // Arrange
            using (var db = Dbs.ContentAndTemplateFiles())
            {

                // Act
                var page = db.GetItem(Ids.AboutHabitat);

                // Assert
                Assert.IsFalse(string.IsNullOrEmpty(page[Sitecore.FieldIDs.LayoutField]));
            }
        }
    }
}