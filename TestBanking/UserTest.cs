using System;
using Xunit;
using Banking.Interfaces;
using Banking.Implementations;

namespace TestBanking
{
    public class UserTest
    {
        string login = "TEST";

        [Fact]
        public void Constructor_Null_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User(null));
        }

        [Fact]
        public void Constructor_EmptyString_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User(""));
        }

        [Fact]
        public void ToString_EqualStrings()
        {
            IUser user = new User(login);
            IUser anotherUser = new User(login);

            Assert.Equal(user.ToString(), anotherUser.ToString());
        }

        [Fact]
        public void Equals_Object_False()
        {
            IUser user = new User(login);

            Assert.False(user.Equals(new object()));
        }

        [Fact]
        public void Equals_User_True()
        {
            IUser user = new User(login);
            IUser anotherUser = new User(login);

            Assert.Equal(user, anotherUser);
        }

        [Fact]
        public void Equals_AnotherUser_False()
        {
            IUser user = new User(login);
            IUser anotherUser = new User("test");

            Assert.NotEqual(user, anotherUser);
        }
    }
}
