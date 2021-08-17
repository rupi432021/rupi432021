using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Project_FOA.Models
{
    public class FriendsTweeter
    {
        string idTweeter;
        string idFriendTweeter;
        int suspectedConnection;
        bool tweeterIsNew; 

        public FriendsTweeter() { }

        public FriendsTweeter(string idTweeter, string idFriendTweeter, int suspectedConnection, bool tweeterIsNew)
        {
            IdTweeter = idTweeter;
            IdFriendTweeter = idFriendTweeter;
            SuspectedConnection = suspectedConnection;
            TweeterIsNew = tweeterIsNew;
        }

        public string IdTweeter { get => idTweeter; set => idTweeter = value; }
        public string IdFriendTweeter { get => idFriendTweeter; set => idFriendTweeter = value; }
        public bool TweeterIsNew { get => tweeterIsNew; set => tweeterIsNew = value; }
        public int SuspectedConnection { get => suspectedConnection; set => suspectedConnection = value; }

        public void InsertTweetersFriend(List<FriendsTweeter> AllTweetersFriend) //insert connection of friends 
        {
            DBservices dbs = new DBservices();
            dbs.InsertFriendsOfTweeter(AllTweetersFriend);
        }
    }
}