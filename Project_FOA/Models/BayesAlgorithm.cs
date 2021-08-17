using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class BayesAlgorithm
    {
        int isAntisemitic;
        double antisemitismPercentage;

        public BayesAlgorithm(int isAntisemitic, double antisemitismPercentage)
        {
            IsAntisemitic = isAntisemitic;
            AntisemitismPercentage = antisemitismPercentage;
        }

        public int IsAntisemitic { get => isAntisemitic; set => isAntisemitic = value; }
        public double AntisemitismPercentage{ get => antisemitismPercentage; set => antisemitismPercentage = value; }

        public BayesAlgorithm() { }
    }
}