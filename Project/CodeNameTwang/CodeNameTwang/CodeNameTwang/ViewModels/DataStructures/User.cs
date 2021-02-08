using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using CodeNameTwang.Model;

namespace CodeNameTwang.ViewModels.DataStructures
{
    public class User : JsonObject
    {
        public User()
        {
            Settable("Twang.Users");
            SetPK("id");
        }
        public int id { 
            get { 
                return this.GetProperty<int>("id"); 
            } 
            set {
                SetProperty<int>("id", value); 
            } 
        }
        public string name { 
            get {
                return this.GetProperty<string>("uname");
            } 
            set {
                SetProperty<string>("uname", value); 
            } 
        }
        public string pkey { 
            get {
                return this.GetProperty<string>("pkey");
            } 
            set {
                SetProperty<string>("pkey", value); 
            } 
        }

        public bool IsOnline()
        {
            AccessLog log = new AccessLog();
            log.uid = id;
            if (log.PromoteToMostRecent()) {
                return log.LogoffTime.Year == 1753;//tsql datetime.minvalue
            }
            return false;
        }

        public string GetLastAccesIP() {
            AccessLog log = new AccessLog();
            log.uid = id;
            if (log.PromoteToMostRecent()) { 
                return log.accessip;
            }
            return "NONE";
        }


        public DateTime GetLastOnline()
        {
            AccessLog log = new AccessLog();
            log.uid = id;
            if (log.PromoteToMostRecent())
            {
                return log.LogoffTime;
            }
            return new DateTime(1753, 0, 1);
        }

        public string GetEmail() {
            return name.Replace(' ', '.') + "@taymark.scot";
        }
    }
}
