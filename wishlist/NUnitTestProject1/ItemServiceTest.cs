using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using wishlist.Models;
using wishlist.Services;
using wishlist.Interfaces;
using wishlist.Helpers;
using System.Security.Principal;
using System;

namespace wishlist.Tests
{
    public class ItemServiceTests
    {
        private IItemService repository;
        private Mock<DbSet<Item>> mockSet;
        private Mock<WishlistDBContext> mockContext;
        private Mock<ContextMockHelper> mockModifier;
        IQueryable<Item> items;

        [SetUp]
        public void Setup()
        {
            // Arrange - We're mocking our dbSet & dbContext
            // in-memory data
            items = new List<Item>
            {
                new Item
                {
                    ItemId = 1,
                    ListId = 1,
                    ItemName = "Hamlet"
                },
                new Item
                {
                    ItemId = 2,
                    ListId = 1,
                    ItemName = "test2",

                },
                new Item
                {
                    ItemId = 3,
                    ListId = 2,
                    ItemName = "item3",

                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockSet = new Mock<DbSet<Item>>();
            mockSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(items.Provider);
            mockSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(items.Expression);
            mockSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(items.ElementType);
            mockSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            mockContext = new Mock<WishlistDBContext>();
            mockContext.Setup(c => c.Item).Returns(mockSet.Object);
            mockContext.SetupSequence(c => c.SaveChanges())
                .CallBase()
                .Throws(new Exception());

            mockModifier = new Mock<ContextMockHelper>();
            mockModifier.Setup(m => m.SetPropertyIsModified(It.IsAny<WishlistDBContext>(), It.IsAny<Item>())).Returns(true);

            repository = new ItemService(mockContext.Object, mockModifier.Object);

        }

        [Test]
        public void Create_Item_Successfully()
        {
            var answer = repository.Create(new Item
            {
                ListId = 2,
                ItemName = "item4",

            });

            Assert.IsFalse(answer.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Item>()), Times.Once);
            mockContext.Verify(m =>  m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Create_Item_DbErrorMessage()
        {
            var answer1 = repository.Create(new Item
            {
                ListId = 2,
                ItemName = "item4",

            });

            var answer2 = repository.Create(new Item
            {
                ListId = 2,
                ItemName = "item5",

            });

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Item>()), Times.Exactly(2));
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Create_RepeatedItem_RepeatedItemErrorMessage()
        {
            var answer = repository.Create(new Item
            {
                ItemId = 1,
                ListId = 1,
                ItemName = "Hamlet"
            });

            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Item>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Delete_Item_Successfully()
        {
            var answer = repository.Delete(1);

            Assert.IsFalse(answer.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Item>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Delete_Item_DbErrorMessage()
        {
            var answer1 = repository.Delete(1);
            var answer2 = repository.Delete(2);

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Item>()), Times.Exactly(2));
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Delete_NotExistingItem_NotFoundErrorMessage()
        {
            var answer = repository.Delete(10);

            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Item>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Edit_Item_Successfully()
        {
            var answer = repository.Edit(1, new Item
            {
                ItemId = 1,
                ListId = 1,
                ItemName = "Hamlet2"
            });

            Assert.IsFalse(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Edit_Item_DbErrorMessage()
        {
            var answer1 = repository.Edit(1, new Item
            {
                ItemId = 1,
                ListId = 1,
                ItemName = "Hamlet2"
            });

            var answer2 = repository.Edit(1, new Item
            {
                ItemId = 1,
                ListId = 1,
                ItemName = "Hamlet3"
            });

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Edit_ItemWithWrongId_WrongIdErrorMessage()
        {
            var answer = repository.Edit(1, new Item
            {
                ItemId = 2,
                ListId = 1,
                ItemName = "Hamlet2"
            });

            Assert.IsTrue(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Edit_ItemThatDoesntExist_NotFoundErrorMessage()
        {
            var answer = repository.Edit(20, new Item
            {
                ItemId = 20,
                ListId = 1,
                ItemName = "Hamlet2"
            });

            Assert.IsTrue(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void GetById_Item_Successfully()
        {
            var answer = repository.GetById(1);

            Assert.IsFalse(answer.HasMessage());
            Assert.IsTrue(answer.Value.ItemName.Equals("Hamlet"));
        }

        [Test]
        public void GetById_InvalidItemId_InvalidItemErrorMessage()
        {
            var answer = repository.GetById(10);

            Assert.IsTrue(answer.HasMessage()); 
        }
    }
}