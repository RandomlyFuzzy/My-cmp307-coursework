using System;
using CodeNameTwang.ViewModels.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class TestRoom
    {
        [TestMethod]
        public void testID()
        {
            Room usr = new Room();
            usr.id = 1;
            Assert.AreEqual(usr.GetProperty<int>("id"), 1);
        }

        [TestMethod]
        public void testname()
        {
            Room usr = new Room();
            usr.rname = "hello";
            Assert.AreEqual(usr.GetProperty<string>("rname"), "hello");
        }
        [TestMethod]
        public void testpkey()
        {
            Room usr = new Room();
            usr.capcaity = 123456;
            Assert.AreEqual(usr.GetProperty<int>("capacity"), 123456);
        }
    }
}
