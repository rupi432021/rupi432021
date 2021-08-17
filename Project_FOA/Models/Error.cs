using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class Error
    {
        string createdAt;
        string text;


        public string CreatedAt { get => createdAt; set => createdAt = value; }
        public string Text { get => text; set => text = value; }

        public Error(string createdAt, string text)
        {
            CreatedAt = createdAt;
            Text = text;
        }
        public Error() { }

        public void insert() //update existing tweeters
        {
            DBservices dbs = new DBservices();
            dbs.insertError(this);
        }

    }
}