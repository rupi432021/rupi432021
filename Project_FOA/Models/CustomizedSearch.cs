using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tweetinvi.Models;

namespace Project_FOA.Models
{
    public class CustomizedSearch
    {
        int idSearch;
        string searchKey;
        int isActive;
        ITweet[] tweetsArray;

        public CustomizedSearch(int idSearch, string searchKey, int isActive, ITweet[] tweetsArray)
        {
            IdSearch = idSearch;
            SearchKey = searchKey;
            IsActive = isActive;
            TweetsArray = tweetsArray;
        }
        
        public int IdSearch { get => idSearch; set => idSearch = value; }
        public string SearchKey { get => searchKey; set => searchKey = value; }
        public int IsActive { get => isActive; set => isActive = value; }
        public ITweet[] TweetsArray { get => tweetsArray; set => tweetsArray = value; }

        public CustomizedSearch() { }

        public string getCustomizedSearch(string word, int idUser, string searchOption)
        {
            DBservices dbs = new DBservices();
            if (searchOption == "Hashtag")
                word = "#" + word;
            else
                word = "”" + word + "”";

            return dbs.getCustomizedSearch(word, idUser);
        }

        public List<string> getCustomizedSearchOfUser(int idUser)
        {
            DBservices dbs = new DBservices();
            return dbs.getCustomizedSearchOfUser(idUser);
        }

        public CustomizedSearch getCustomizedSearchByKey(string searchKey)
        {
            DBservices dbs = new DBservices();
            return dbs.getCustomizedSearchByKey(searchKey);
        }

        public List<string> GetCustomized(int idUser)
        {
            DBservices dbs = new DBservices();
            return dbs.GetCustomized(idUser);
        }
        public void UpdateCustomizedSearch(int idSearch)
        {
            DBservices dbs = new DBservices();
            dbs.UpdateCustomizedSearch(idSearch);
        }
    }
}