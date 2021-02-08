using System;
using System.Collections.Generic;
using System.Net;
using CodeNameTwang.Model;
using CodeNameTwang.Services.RestAPI;
using CodeNameTwang.ViewModels.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class TestWebServicIntegration
    {

        /**
         * test connection
         * test simple get request
         * test simple post request
         * test simple put request
         * test simple delete request
         * test Json Decoder Works
         */

        string token = "";

        [TestMethod]
        public void TestJsonDecode() {
            JsonObject obj = JsonParser.Parse<JsonObject>("[{\"hello\":\"world\"}]")[0];
            Assert.IsTrue(obj.GetProperty<string>("hello") == "world", "should contain a variable");
        }

        [TestMethod]
        public void TestConnection()
        {
            HTTPRequester.APILocation = "http://127.0.0.1/";
            Token t = RequestFactory.Get<Token>("login", "uname", "Elsa Duncan", "id", "11630816").GetAwaiter().GetResult()[0];
            Assert.IsNotNull(t.token);
            token = t.token;
        }
        [TestMethod]
        public void TestGet()
        {
            if (token == "")
            {
                TestConnection();
            }
            JsonObject j = RequestFactory.Get<JsonObject>($"DATA/{token}/Twang.Users/11630816").GetAwaiter().GetResult()[0];
            Assert.IsNotNull(j);
        }
        [TestMethod]
        public void TestPut()
        {
            if (token == "")
            {
                TestConnection();
            }
            //token does not have insert rights
            Assert.ThrowsException<WebException>(()=> { RequestFactory.Put($"DATA/{token}/Twang.Users", "uname", "Random User", "id", "42424242", "pkey", "randomkey").GetAwaiter().GetResult(); });
        }
        [TestMethod]
        public void TestPost()
        {
            if (token == "")
            {
                TestConnection();
            }
            //token does not have update rights
            Assert.ThrowsException<WebException>(() => { RequestFactory.Post($"DATA/{token}/Twang.Users/42424242", "uname", "Random User2", "id", "42424242", "pkey", "randomkey2").GetAwaiter().GetResult(); });
        }

        [TestMethod]
        public void TestDelete()
        {
            if (token == "")
            {
                TestConnection();
            }
            //token does not have delete rights
            Assert.ThrowsException<WebException>(() => { RequestFactory.Delete($"DATA/{token}/Twang.Users/42424242").GetAwaiter().GetResult(); });
        }
    }
}
