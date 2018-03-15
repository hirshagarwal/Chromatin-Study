using System;

namespace Assets.Scripts
{
    public class Trial
    {
        private int row;
        private Tasks task;
        private int trialNumber;
        private Boolean training;
        private IGenericTrial trialDetails;

        public Trial(String s, Formats format)
        {
            //0             1   	  2         	3    	 4     	     5        6          7    8    9     10    11
            //SUBJECTNUMBER,ROWNUMBER,TECHNIQUENAME,TASKNAME,TRIALNUMBER,TRAINING,CHROMOSOME,REDA,REDB,BLUEA,BLUEB,REDISSHORTER
            String[] trialConditions = s.Split(',');
            row = Int32.Parse(trialConditions[1]);
            trialNumber = Int32.Parse(trialConditions[4]);

            if (Tasks.PointDistance.ToString() == trialConditions[3])
            {
                task = Tasks.PointDistance;
                trialDetails = new PointDistanceTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    trialConditions[9],
                    trialConditions[10],
                    Boolean.Parse(trialConditions[11]),
                    format
                    );
            }
            else if (Tasks.SegmentDistance.ToString() == trialConditions[3])
            {
                task = Tasks.SegmentDistance;
                trialDetails = new SegmentDistanceTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    trialConditions[9],
                    trialConditions[10],
                    Boolean.Parse(trialConditions[11]),
                    format
                    );
            }
            else if (Tasks.CurveComparison.ToString() == trialConditions[3])
            {
                task = Tasks.CurveComparison;
                trialDetails = new CurveComparisonTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    Boolean.Parse(trialConditions[9]),
                    format
                    );
            }
            else if (Tasks.AttributeUnderstanding.ToString() == trialConditions[3])
            {
                task = Tasks.AttributeUnderstanding;
                trialDetails = new AttributeUnderstandingTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    trialConditions[9],
                    Boolean.Parse(trialConditions[10]),
                    format
                    );
            }
            else if (Tasks.TouchingSegments.ToString() == trialConditions[3])
            {
                task = Tasks.TouchingSegments;
                trialDetails = new TouchingPointsTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    format
                    );
            }
            else if (Tasks.LargerTad.ToString() == trialConditions[3])
            {
                task = Tasks.LargerTad;
                trialDetails = new LargerTadTrial(
                    trialConditions[6],
                    trialConditions[7],
                    format
                    );
            }
            else if (Tasks.Triple.ToString() == trialConditions[3])
            {
                task = Tasks.Triple;
                trialDetails = new TripleTrial(
                    trialConditions[6],
                    trialConditions[7],
                    trialConditions[8],
                    format
                    );
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
    }
}