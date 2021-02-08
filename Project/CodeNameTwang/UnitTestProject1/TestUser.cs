using System;
using CodeNameTwang.ViewModels.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        public void testID()
        {
            User usr = new User();
            usr.id = 1;
            Assert.AreEqual(usr.GetProperty<int>("id"), 1);
        }

        [TestMethod]
        public void testname()
        {
            User usr = new User();
            usr.name = "hello";
            Assert.AreEqual(usr.GetProperty<string>("uname"), "hello");
        }
        [TestMethod]
        public void testpkey()
        {
            User usr = new User();
            usr.pkey = "123456";
            Assert.AreEqual(usr.GetProperty<string>("pkey"), "123456");
        }
    }
}
