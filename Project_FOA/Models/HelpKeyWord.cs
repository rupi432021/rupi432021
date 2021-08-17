using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class HelpKeyWord
    {
        int idHelpKey;
        string keyWord;

        public HelpKeyWord(int idHelpKey, string keyWord)
        {
            IdHelpKey = idHelpKey;
            KeyWord = keyWord;
        }

        public int IdHelpKey { get => idHelpKey; set => idHelpKey = value; }
        public string KeyWord { get => keyWord; set => keyWord = value; }

        public HelpKeyWord() { }

        public List<HelpKeyWord> getHelpKeyWords() //get key words
        {
            DBservices dbs = new DBservices();
            List<HelpKeyWord> helpKeyWordsList = dbs.getHelpKeyWords();    
            return helpKeyWordsList;           
        }

        public List<string> getPrefixJewsWords() //get key words
        {
            DBservices dbs = new DBservices();
             return dbs.getPrefixJewsWords();    
        }
    }
}