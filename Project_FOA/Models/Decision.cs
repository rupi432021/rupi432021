using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class Decision
    {
        int idDecision;
        string reason;
        int decisionBit; //1 or 0 - decide to report the tweet or not

        public Decision(int idDecision, string reason, int decisionBit)
        {
            IdDecision = idDecision;
            Reason = reason;
            DecisionBit = decisionBit;
        }

        public int IdDecision { get => idDecision; set => idDecision = value; }
        public string Reason { get => reason; set => reason = value; }
        public int DecisionBit { get => decisionBit; set => decisionBit = value; }


        public Decision() { }

        public List<Decision> getDecisions()
        {
            DBservices dbs = new DBservices();
            List<Decision> DecisionsList = dbs.getDecisions();
            return DecisionsList;
        }    
    }
}
