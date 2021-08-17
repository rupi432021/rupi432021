using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class PieAntiFriend
    {
        string tweeterName;
        int antisemiticFriends;

        public PieAntiFriend(string tweeterName, int antisemiticFriends)
        {
            TweeterName = tweeterName;
            AntisemiticFriends = antisemiticFriends;
        }

        public string TweeterName { get => tweeterName; set => tweeterName = value; }
        public int AntisemiticFriends { get => antisemiticFriends; set => antisemiticFriends = value; }

        public PieAntiFriend() { }

        public List<PieAntiFriend> getPieAntiFriends()
        {
            DBservices dbs = new DBservices();
            return dbs.getPieAntiFriends();

        }

    }
}