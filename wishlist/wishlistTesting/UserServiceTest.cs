using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using wishlist.Models;
using wishlist.Services;
using wishlist.Interfaces;
using System;
using Microsoft.AspNetCore.Identity;

namespace wishlist.Tests
{ 
    public class UserServiceTest
    {
        private IUserService repository;
        private Mock<DbSet<Users>> mockSet;
        private Mock<WishlistDBContext> mockContext;

        [SetUp]
        public void Setup()
        {
            // Arrange - We're mocking our dbSet & dbContext
            // in-memory data
            IQueryable<Users> Users = new List<Users>
            {
                new Users
                {
                    UserId = 1,
                    Username = "Hamlet",
                    Pswd = "AQAAAAEAACcQAAAAEPGmmTCHomNYn5AeaowIyhBZjdeop7yQzBfcRc+79vrruuMicxiuvX2Et1xprI+CIg=="
                },
                new Users
                {
                    UserId = 2,
                    Username = "test2",
                    Pswd = "test"
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            mockSet = new Mock<DbSet<Users>>();
            mockSet.As<IQueryable<Users>>().Setup(m => m.Provider).Returns(Users.Provider);
            mockSet.As<IQueryable<Users>>().Setup(m => m.Expression).Returns(Users.Expression);
            mockSet.As<IQueryable<Users>>().Setup(m => m.ElementType).Returns(Users.ElementType);
            mockSet.As<IQueryable<Users>>().Setup(m => m.GetEnumerator()).Returns(Users.GetEnumerator());

            mockContext = new Mock<WishlistDBContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.SetupSequence(c => c.SaveChanges())
                .CallBase()
                .Throws(new Exception());

            // Act - fetch Users
            repository = new UserService(mockContext.Object, new PasswordHasher<String>());
        }

        [Test]
        public void GetAll_Users_Successfully()
        {
            
            var actual = repository.GetAll().Values;

            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual("Hamlet", actual.First().Username);
            Assert.Null(actual.First().Pswd);
        }

        [Test]
        public void Create_User_Successfully()
        {
            var answer = repository.Create(new Users
            {
                Username = "a",
                Pswd = "b"
            });

            Assert.IsFalse(answer.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Users>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void Create_RepeatUser_RepeatUsernameErrorMessage()
        {
            var answer = repository.Create(new Users
            {
                Username = "Hamlet",
                Pswd = "b"
            });
   
            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Users>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Create_UserWithoutPassword_NoPasswordErrorMessage()
        {
            var answer = repository.Create(new Users
            {
                Username = "User",
                Pswd = ""
            });

            Assert.IsTrue(answer.HasMessage());
            mockSet.Verify(m => m.Add(It.IsAny<Users>()), Times.Never);
            mockContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Test]
        public void Create_User_DbErrorMessage()
        {
            var answer1 = repository.Create(new Users
            {
                Username = "User7",
                Pswd = "oiii"
            });

            var answer2 = repository.Create(new Users
            {
                Username = "User7",
                Pswd = "oiii"
            });

            Assert.IsFalse(answer1.HasMessage());
            Assert.IsTrue(answer2.HasMessage());
            mockContext.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Authenticate_User_Successfully()
        {
            var answer = repository.Authenticate("Hamlet", "123");

            Assert.IsFalse(answer.HasMessage());
            Assert.Null(answer.Value.Token);
        }

        [Test]
        public void Authenticate_WrongPassword_WrongUsernameOrPasswordErrorMessage()
        {
            var answer = repository.Authenticate("Hamlet", "wrong");

            Assert.IsTrue(answer.HasMessage());
        }

        [Test]
        public void GetById_User_Successfully()
        {
            var answer = repository.GetById(1);

            Assert.IsFalse(answer.HasMessage());
            Assert.Null(answer.Value.Pswd);
        }
    }
}