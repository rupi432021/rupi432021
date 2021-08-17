using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class HashtagsToUser
    {
        int idHashUser;
        int idUser;
        string hashtag;
        List<string> hashtags;

        public HashtagsToUser(int idHashUser, int idUser, string hashtag, List<string> hashtags)
        {
            IdHashUser = idHashUser;
            IdUser = idUser;
            Hashtag = hashtag;
            Hashtags = hashtags;
        }

        public int IdHashUser { get => idHashUser; set => idHashUser = value; }
        public int IdUser { get => idUser; set => idUser = value; }
        public string Hashtag { get => hashtag; set => hashtag = value; }
        public List<string> Hashtags { get => hashtags; set => hashtags = value; }

        public HashtagsToUser() { }

        public void InsertHashtagsToUser(HashtagsToUser hashtagsToUser) //insert hashtags to manager - table ExploredHashtagsToManager_2021
        {
            for (int i = 0; i < hashtagsToUser.Hashtags.Count; i++)
            {
                hashtagsToUser.Hashtags[i] = "#" + hashtagsToUser.Hashtags[i];
            }
            DBservices dbs = new DBservices();
            dbs.InsertHashtagsToUser(hashtagsToUser);
        }

        public List<HashtagsToUser> getExploredHashtags(int idUser)
        {
            DBservices dbs = new DBservices();
            return dbs.getExploredHashtags(idUser);
        }

        public List<HashtagsToUser> deleteHashtagsToUser(List<HashtagsToUser> hashtagsToUser)
        {
            DBservices dbs = new DBservices();
            return dbs.deleteHashtagsToUser(hashtagsToUser);
        }
        public List<HashtagsToUser> PostFromExploredToPersonalList(List<HashtagsToUser> hashtagsToUser) //insert hashtags to manager - table ExploredHashtagsToManager_2021
        {
           
            DBservices dbs = new DBservices();
            return dbs.PostFromExploredToPersonalList(hashtagsToUser);
        }
        

    }
}