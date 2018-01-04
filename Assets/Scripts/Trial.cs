using System;

namespace Assets.Scripts
{
    public class Trial
    {
        private int row;
        private Tasks task;
        private int trialNumber;
        private Boolean training;
        private string chromosomeFilename;
        private IGenericTrial trialDetails;

        public Trial(String s)
        {
            String[] trialConditions = s.Split(',');
            row = Int32.Parse(trialConditions[1]);
            trialNumber = Int32.Parse(trialConditions[4]);

            if (Tasks.PointDistance.ToString() == trialConditions[3])
            {
                task = Tasks.PointDistance;
                trialDetails = new PointDistanceTrial(
                    Int32.Parse(trialConditions[6]),
                    trialConditions[7],
                    trialConditions[8],
                    trialConditions[9],
                    trialConditions[10],
                    Boolean.Parse(trialConditions[11])
                    );
                chromosomeFilename = trialConditions[12];
            }
            else
            {
                throw new Exception("Unhandled task: " + trialConditions[3]);
            }

            if ("T" == trialConditions[5])
            {
                training = true;
            }
            else if ("R" == trialConditions[5])
            {
                training = false;
            }
            else
            {
                throw new Exception("Unhandled training type: " + trialConditions[5]);
            }

            
        }

        public string ToCSV()
        {
            return trialNumber + ", " +
            task + ", " +
            training.ToString() + "," +
            trialDetails.ToCSV();
        }

        public int Row
        {
            get
            {
                return row;
            }
        }

        public Tasks Task
        {
            get
            {
                return task;
            }
        }

        public int TrialNumber
        {
            get
            {
                return trialNumber;
            }
        }

        public bool Training
        {
            get
            {
                return training;
            }
        }

        public IGenericTrial TrialDetails
        {
            get
            {
                return trialDetails;
            }
        }

        public string ChromosomeFilename
        {
            get
            {
                return chromosomeFilename;
            }
        }
    }
}