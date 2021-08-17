using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tweetinvi.Parameters;

namespace Project_FOA.Models
{
    public class PieSearch
    {
        string searchKey;
        int tweetsCount;

        public PieSearch(string searchKey, int tweetsCount)
        {
            SearchKey = searchKey;
            TweetsCount = tweetsCount;
        }

        public string SearchKey { get => searchKey; set => searchKey = value; }
        public int TweetsCount { get => tweetsCount; set => tweetsCount = value; }

        public PieSearch() { }

        public List<PieSearch> getPieSearch() 
        {
            DBservices dbs = new DBservices();
            return dbs.getPieSearch();
         
        }

    }
}