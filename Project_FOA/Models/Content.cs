using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project_FOA.Models.DAL;

namespace Project_FOA.Models
{
    public class Content
    {
        int id;
        string idStrTweet;
        string textTweet;
        string urlTweet;
        string idStrUser;

        public string IdStrTweet { get => idStrTweet; set => idStrTweet = value; }
        public string TextTweet { get => textTweet; set => textTweet = value; }
        public string UrlTweet { get => urlTweet; set => urlTweet = value; }
        public string IdStrUser { get => idStrUser; set => idStrUser = value; }
        public int Id { get => id; set => id = value; }

        public Content(int id, string idStrTweet, string textTweet, string urlTweet, string idStrUser)
        {
            this.id = id;
            this.idStrTweet = idStrTweet;
            this.textTweet = textTweet;
            this.urlTweet = urlTweet;
            this.idStrUser = idStrUser;
        }

        public Content() { }

        public void InsertContent(List<Content> contents)
        {
            DBservices dbs = new DBservices();
            dbs.InsertContent(contents);
        }

      

    }

}