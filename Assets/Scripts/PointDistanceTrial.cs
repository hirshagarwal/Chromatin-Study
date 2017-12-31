﻿using System;

namespace Assets.Scripts
{
    internal class PointDistanceTrial : IGenericTrial
    {
        private int chromosome;
        private string redA;
        private string redB;
        private string blueA;
        private string blueB;
        private Boolean redIsShorter;

        public PointDistanceTrial(int chromosome, string redA, string redB, string blueA, string blueB, bool redIsShorter)
        {
            this.chromosome = chromosome;
            this.redA = redA;
            this.redB = redB;
            this.blueA = blueA;
            this.blueB = blueB;
            this.redIsShorter = redIsShorter;
        }

        public int Chromosome
        {
            get
            {
                return chromosome;
            }
        }

        public string RedA
        {
            get
            {
                return redA;
            }
        }

        public string RedB
        {
            get
            {
                return redB;
            }
        }

        public string BlueA
        {
            get
            {
                return blueA;
            }
        }

        public string BlueB
        {
            get
            {
                return blueB;
            }
        }

        public Boolean Correct(object subjectChoseRed)
        {
            return (Boolean)subjectChoseRed == redIsShorter;
        }

        public String ToCSV()
        {
            return chromosome.ToString() + ", " +
                redA + ", " +
                redB + ", " +
                blueA + ", " +
                blueB + ", " +
                redIsShorter.ToString();
        }
    }
}