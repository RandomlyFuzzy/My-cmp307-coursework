using System;
using CodeNameTwang.ViewModels.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class TestAccessLog
    {

        [TestMethod]
        public void testID()
        {
            AccessLog al = new AccessLog();
            al.id = 1;
            Assert.AreEqual(al.GetProperty<int>("id"), 1);
        }

        [TestMethod]
        public void testUID()
        {
            AccessLog al = new AccessLog();
            al.uid = 1;
            Assert.AreEqual(al.GetProperty<int>("uid"), 1);
        }
        [TestMethod]
        public void testaccessip()
        {
            AccessLog al = new AccessLog();
            al.accessip = "1.1.1.1";
            Assert.AreEqual(al.GetProperty<string>("accessip"), "1.1.1.1");
        }
        [TestMethod]
        public void testLogonTime()
        {
            AccessLog al = new AccessLog();
            DateTime dt = DateTime.Now;
            al.LogonTime = dt;
            Assert.AreEqual(al.GetProperty<DateTime>("addtime"), dt);
        }
        [TestMethod]
        public void testLogoffTime()
        {
            AccessLog al = new AccessLog();
            DateTime dt = DateTime.Now;
            al.LogoffTime = dt;
            Assert.AreEqual(al.GetProperty<DateTime>("Logofftime"), dt);
        }







    }
}
