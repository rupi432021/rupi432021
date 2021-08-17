using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class HashtagsToManager
    {
        int idHashManager;
        string hashtag;

        public HashtagsToManager(int idHashManager, string hashtag)
        {
            IdHashManager = idHashManager;
            Hashtag = hashtag;
        }

        public int IdHashManager { get => idHashManager; set => idHashManager = value; }
        public string Hashtag { get => hashtag; set => hashtag = value; }

        public HashtagsToManager() { }

        public void InsertHashtagsToManager(List<string> hashtagsToManager) //insert hashtags to manager - table ExploredHashtagsToManager_2021
        {
            for (int i = 0; i < hashtagsToManager.Count; i++)
            {
                hashtagsToManager[i] = "#" + hashtagsToManager[i];
            }
            DBservices dbs = new DBservices();
           dbs.InsertHashtagsToManager(hashtagsToManager);
        }

        public List<HashtagsToManager> getHashtagsToManager() //get antisemitic tweets by id tweeter
        {
            DBservices dbs = new DBservices();
            List<HashtagsToManager> hashtagsToManagerList = dbs.getHashtagsToManager();
            return hashtagsToManagerList;
        }
        public void deleteExploredHashtags(int idExploredHashtags) //get search key words
        {
            DBservices dbs = new DBservices();
            dbs.deleteExploredHashtags(idExploredHashtags);
        }
    }
}