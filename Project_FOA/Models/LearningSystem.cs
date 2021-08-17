using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class LearningSystem
    {
        int idSearch;
        int countTweet;
        string searchKey;

        public LearningSystem(int idSearch, int countTweet,string searchKey)
        {
            IdSearch = idSearch;
            CountTweet = countTweet;
            SearchKey = searchKey;
        }

        public int IdSearch { get => idSearch; set => idSearch = value; }
        public int CountTweet { get => countTweet; set => countTweet = value; }
        public string SearchKey { get => searchKey; set => searchKey = value; }


        public LearningSystem() { }

        public List<LearningSystem> getLearningSystemCustomized() //  get count of reported tweets for each hashtage.
        {
            DBservices dbs = new DBservices();
            return dbs.getLearningSystemCustomized();
        }

        public List<LearningSystem> GetLearningSystem() //  get count of reported tweets for each hashtage.
        {
            DBservices dbs = new DBservices();
            return dbs.getLearningSystem();
        }

        public List<string> GetMeassagesManager() //  get count of reported tweets for each hashtage.
        {
            DBservices dbs = new DBservices();
            return dbs.GetMeassagesManager();
        }
        public List<string> GetMeassagesUser() //  //get list of words that removed from customer hashtags list, and was added to the system list. we found that this word is important to our search engine
        {
            DBservices dbs = new DBservices();
            return dbs.GetMeassagesUser();
        }

    }
}