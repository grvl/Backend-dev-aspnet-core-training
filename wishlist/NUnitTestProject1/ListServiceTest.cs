using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using wishlist.Models;
using wishlist.Services;
using wishlist.Interfaces;
using Lists = wishlist.Models.List;
namespace Wishlist.Tests
{
    public class ListsRepositoryTests
    {
        private IListService repository;
        private Mock<DbSet<Lists>> mockSet;
        private Mock<WishlistDBContext> mockContext;

        [SetUp]
        public void Setup()
        {
            // Arrange - We're mocking our dbSet & dbContext
            // in-memory data
            IQueryable<Lists> Lists = new List<Lists>
            {
                new Lists
                {
                    ListId = 1,
                    ListName = "Hamlet",
                    UserList = new List<UserList>
                    {
                        new UserList
                        {
                            ListId = 1,
                            UserId = 1
                        }
                    }
                },
                new Lists
                {
                    ListId = 2,
                    ListName = "test2",
                    UserList = new List<UserList>
                    {
                        new UserList
                        {
                            ListId = 2,
                            UserId = 2
                        }
                    }
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockSet = new Mock<DbSet<Lists>>();
            mockSet.As<IQueryable<Lists>>().Setup(m => m.Provider).Returns(Lists.Provider);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.Expression).Returns(Lists.Expression);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.ElementType).Returns(Lists.ElementType);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.GetEnumerator()).Returns(Lists.GetEnumerator());

            mockContext = new Mock<WishlistDBContext>();
            mockContext.Setup(c => c.List).Returns(mockSet.Object);

            // Act - fetch Lists
            repository = new ListService(mockContext.Object);
        }

        [Test]
        public void GetAllListsTest()
        {
            
            var actual = repository.GetAll(1,1,1).Values;

            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual("Hamlet", actual.First().ListName);
        }

        //[Test]
        //public void GetByIdTest()
        //{
        //    var actual = repository.GetById(1,1,1).Value;

        //    Assert.AreEqual("Hamlet", actual.ListName);
        //}
    }
}