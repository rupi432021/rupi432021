using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tweetinvi.Models;

namespace Project_FOA.Models
{
    public class Search
    {
        int idSearch;
        string searchKey;
        ITweet[] tweetsArray;  

        public Search(int idSearch, string searchKey, ITweet[] tweetsArray)
        {
            IdSearch = idSearch;
            SearchKey = searchKey;
            TweetsArray = tweetsArray;
        }

        public int IdSearch { get => idSearch; set => idSearch = value; }
        public string SearchKey { get => searchKey; set => searchKey = value; }
        public ITweet[] TweetsArray { get => tweetsArray; set => tweetsArray = value; }

        public Search() { }

        public List<Search> getSearch() //get search key words
        {
            DBservices dbs = new DBservices();
            List<Search> searchList = dbs.getSearch();
            return searchList;
        }

        public List<Search> getSearchForMonitoring() //get random 2 search key words for monitoring
        {
            DBservices dbs = new DBservices();
            List<Search> searchList = dbs.getSearchForMonitoring();
            return searchList;
        }


        public List<string> GetGeneralHashtagsNotToSearch() //get general hashtags not to search
        {
            DBservices dbs = new DBservices();
            return dbs.GetGeneralHashtagsNotToSearch();
        }

        public List<string> GetSearchSystemKeys() //get search system key
        {
            DBservices dbs = new DBservices();
            return dbs.GetSearchSystemKeys();
        }

        public bool chkContains(string value) //check if the search key contains the help word
        {
            //value = value.Replace("”", "");
            return this.searchKey.ToLower().Contains(value.ToLower());
        }

        public void updateSearch(int idSearch) //get search key words
        {
            DBservices dbs = new DBservices();
           dbs.updateSearch(idSearch);          
        }

        public string postSearch(string searchToPost) //get search key words
        {
            DBservices dbs = new DBservices();
            string searchKey = "";
            var searchKeyArr = searchToPost.Split(',');
            if (searchKeyArr[0] == "Hashtag")
                searchKey = "#" + searchKeyArr[1];
            else
                searchKey = "”" + searchKeyArr[1] + "”";

            return dbs.postSearch(searchKey);

        }

        public void PostFromExploredToSearch(string hashtagsToPost) //post explored hashtags to search_2021
        {
            DBservices dbs = new DBservices();
            dbs.PostFromExploredToSearch(hashtagsToPost);
        }

    }
}