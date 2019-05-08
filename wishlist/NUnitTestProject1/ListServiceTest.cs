using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using wishlist.Models;
using wishlist.Services;
using wishlist.Interfaces;
using Lists = wishlist.Models.List;
using wishlist.Helpers;
using System;

namespace wishlist.Tests
{
    public class ListsRepositoryTests
    {
        private IListService repository;
        private Mock<DbSet<Item>> mockItem;
        private Mock<DbSet<UserList>> mockUserList;
        private Mock<DbSet<Lists>> mockSet;
        private Mock<DbSet<Users>> mockUser;
        private Mock<WishlistDBContext> mockContext;
        private Mock<ContextMockHelper> mockHelper;

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
                    ListId = 3,
                    ListName = "Hamlet2",
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
                    ListName = "test2"
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockSet = new Mock<DbSet<Lists>>();
            mockSet.As<IQueryable<Lists>>().Setup(m => m.Provider).Returns(Lists.Provider);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.Expression).Returns(Lists.Expression);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.ElementType).Returns(Lists.ElementType);
            mockSet.As<IQueryable<Lists>>().Setup(m => m.GetEnumerator()).Returns(Lists.GetEnumerator());

            IQueryable<UserList> UserLists = new List<UserList>
            {
                new UserList
                {
                    UserId = 1,
                    ListId = 1
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockUserList = new Mock<DbSet<UserList>>();
            mockUserList.As<IQueryable<UserList>>().Setup(m => m.Provider).Returns(UserLists.Provider);
            mockUserList.As<IQueryable<UserList>>().Setup(m => m.Expression).Returns(UserLists.Expression);
            mockUserList.As<IQueryable<UserList>>().Setup(m => m.ElementType).Returns(UserLists.ElementType);
            mockUserList.As<IQueryable<UserList>>().Setup(m => m.GetEnumerator()).Returns(UserLists.GetEnumerator());

            IQueryable<Item> Items = new List<Item>
            {
                new Item
                {
                    ItemId = 1,
                    ListId = 1
                }
            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockItem = new Mock<DbSet<Item>>();
            mockItem.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(Items.Provider);
            mockItem.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(Items.Expression);
            mockItem.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(Items.ElementType);
            mockItem.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(Items.GetEnumerator());

            IQueryable<Users> Users = new List<Users>
            {
                new Users
                {
                    UserId = 1
                },
                new Users
                {
                    UserId = 2
                },
                new Users
                {
                    UserId = 3
                }
            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockUser = new Mock<DbSet<Users>>();
            mockUser.As<IQueryable<Users>>().Setup(m => m.Provider).Returns(Users.Provider);
            mockUser.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(Users.Expression);
            mockUser.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(Users.ElementType);
            mockUser.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(Users.GetEnumerator());

            mockContext = new Mock<WishlistDBContext>();
            mockHelper = new Mock<ContextMockHelper>();
            
            mockContext.Setup(c => c.List).Returns(mockSet.Object);
            mockContext.Setup(c => c.UserList).Returns(mockUserList.Object);
            mockContext.Setup(c => c.Item).Returns(mockItem.Object);
            mockContext.Setup(c => c.Users).Returns(mockUser.Object);
            mockHelper.Setup(c => c.RunStoredProcedure(It.IsAny<WishlistDBContext>(), It.IsAny<int>(), It.IsAny<Lists>())).Returns(true);
            mockHelper.Setup(m => m.SetPropertyIsModified(It.IsAny<WishlistDBContext>(), It.IsAny<Lists>())).Returns(true);

            mockContext.Setup(c => c.List).Returns(mockSet.Object);
            mockContext.SetupSequence(c => c.SaveChanges())
                .CallBase()
                .Throws(new Exception());

            repository = new ListService(mockContext.Object, mockHelper.Object);
        }

        [Test]
        public void Create_List_Successfully()
        {
            var answer = repository.Create(new Lists
            {
                ListName = "a"
            }, 1);

            Assert.IsFalse(answer.HasMessage());
            mockHelper.Verify(m => m.RunStoredProcedure(It.IsAny<WishlistDBContext>(), It.IsAny<int>(), It.IsAny<Lists>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Create_List_DbErrorTest()
        {
            var answer1 = repository.Create(new Lists
            {
                ListName = "a"
            }, 1);

            var answer2 = repository.Create(new Lists
            {
                ListName = "a"
            }, 1);

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockHelper.Verify(m => m.RunStoredProcedure(It.IsAny<WishlistDBContext>(), It.IsAny<int>(), It.IsAny<Lists>()), Times.Exactly(2));
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Delete_List_Successfully()
        {
            var answer = repository.Delete(1);

            Assert.IsFalse(answer.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Lists>()), Times.Once);
            mockItem.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Item>>()), Times.Once);
            mockUserList.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<UserList>>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Delete_ListThatDoesntExist_NotFoundErrorMessage()
        {
            var answer = repository.Delete(10);

            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Lists>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Delete_ListWithoutOwner_InvalidListErrorMessage()
        {
            var answer = repository.Delete(2);

            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Remove(It.IsAny<Lists>()), Times.Once);
            mockItem.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Item>>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Edit_Item_Successfully()
        {
            var answer = repository.Edit(1,
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
            });

            Assert.IsFalse(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Edit_Item_DbErrorMessage()
        {
            var answer1 = repository.Edit(1,
            new Lists
            {
                ListId = 1,
                ListName = "Hamlet1",
                UserList = new List<UserList>
                    {
                        new UserList
                        {
                            ListId = 1,
                            UserId = 1
                        }
                    }
            });

            var answer2 = repository.Edit(1,
            new Lists
            {
                ListId = 1,
                ListName = "Hamlet2",
                UserList = new List<UserList>
                    {
                        new UserList
                        {
                            ListId = 1,
                            UserId = 1
                        }
                    }
            });

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Edit_ItemWithWrongId_WrongIdErrorMessage()
        {
            var answer = repository.Edit(2,
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
            });

            Assert.IsTrue(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Edit_ItemThatDoesntExist_NotFoundErrorMessage()
        {
            var answer = repository.Edit(10,
            new Lists
            {
                ListId = 10,
                ListName = "Hamlet",
                UserList = new List<UserList>
                    {
                        new UserList
                        {
                            ListId = 1,
                            UserId = 1
                        }
                    }
            });

            Assert.IsTrue(answer.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void GetAll_ListsFilteredByUser_Successfully()
        {
            
            var answer = repository.GetAll(1, new ObjectPagination {Size = 10, Page = 1 });

            Assert.IsFalse(answer.HasMessage());
            Assert.AreEqual(2, answer.Value.Result.Count());
            Assert.AreEqual("Hamlet", answer.Value.Result.First().ListName);

        }

        [Test]
        public void GetAll_WhenUserHasZeroLists_EmptyList()
        {

            var answer = repository.GetAll(5, new ObjectPagination { Size = 10, Page = 1 });

            Assert.IsFalse(answer.HasMessage());
            Assert.AreEqual(0, answer.Value.Result.Count());

        }

        [Test]
        public void GetById_List_Successfully()
        {
            var answer = repository.GetById(1, new ObjectPagination { Size = 10, Page = 1 });

            Assert.IsFalse(answer.HasMessage());
            Assert.AreEqual(1, answer.Value.paginatedItems.Result.Count());
            Assert.AreEqual("Hamlet", answer.Value.list.ListName);
        }

        [Test]
        public void GetById_ListWithInvalidId_NotFoundErrorMessage()
        {
            var answer = repository.GetById(5, new ObjectPagination { Size = 10, Page = 1 });

            Assert.IsTrue(answer.HasMessage());
        }

        [Test]
        public void Share_List_Successfully()
        {
            var answer = repository.Share(1, 2);

            Assert.IsFalse(answer.HasMessage());
            Assert.AreEqual("Hamlet", answer.Value.ListName);
        }

        [Test]
        public void Share_ListWithAnInvalidUser_InvalidUserErrorMessage()
        {
            var answer = repository.Share(1, 4);

            Assert.IsTrue(answer.HasMessage());
        }

        [Test]
        public void Share_ListWithInvalidListId_InvalidListErrorMessage()
        {
            var answer = repository.Share(5,1);

            Assert.IsTrue(answer.HasMessage());
        }

        [Test]
        public void Share_ListWithUserThatAlreadyHasAccess_UserAlreadyHasAccessErrorMessage()
        {
            var answer = repository.Share(1, 1);

            Assert.IsTrue(answer.HasMessage());
        }

        [Test]
        public void Share_List_DBErrorMessage()
        {
            var answer1 = repository.Share(1, 2);
            var answer2 = repository.Share(1, 3);

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
        }

    }
}