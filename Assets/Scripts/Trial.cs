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
                int trialID = Int32.Parse(trialConditions[6]);
                String[] trialValues = PointDistance.data[trialID].Split(',');
                trialDetails = new PointDistanceTrial(
                    trialValues[0],
                    trialValues[1],
                    trialValues[2],
                    trialValues[3],
                    trialValues[4],
                    trialValues[5],
                    Boolean.Parse(trialValues[6]),
                    format
                    );
            }
            else if (Tasks.SegmentDistance.ToString() == trialConditions[3])
            {
                throw new NotImplementedException("Need to rewrite where this error is thrown");
                //task = Tasks.SegmentDistance;

                //trialDetails = new SegmentDistanceTrial(
                //    trialConditions[6],
                //    trialConditions[7],
                //    trialConditions[8],
                //    trialConditions[9],
                //    trialConditions[10],
                //    Boolean.Parse(trialConditions[11]),
                //    format
                //    );
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
                throw new NotImplementedException("Need to rewrite where this error is thrown");
                //task = Tasks.AttributeUnderstanding;
                //trialDetails = new AttributeUnderstandingTrial(
                //    trialConditions[6],
                //    trialConditions[7],
                //    trialConditions[8],
                //    trialConditions[9],
                //    Boolean.Parse(trialConditions[10]),
                //    format
                //    );
            }
            else if (Tasks.TouchingSegments.ToString() == trialConditions[3])
            {
                int trialID = Int32.Parse(trialConditions[6]);
                String[] trialValues = TouchingSegments.data[trialID].Split(',');
                task = Tasks.TouchingSegments;
                //UnityEngine.Debug.Log("trialConditions=" + trialConditions[1]);
                //UnityEngine.Debug.Log(trialValues[0]);
                //UnityEngine.Debug.Log(trialValues[1]);
                //UnityEngine.Debug.Log(trialValues[2]);
                //UnityEngine.Debug.Log(trialValues[3]);
                //UnityEngine.Debug.Log(trialValues[4]);
                //UnityEngine.Debug.Log(trialValues[5]);
                //UnityEngine.Debug.Log(trialValues[6]);
                //UnityEngine.Debug.Log(trialValues[11]);

                trialDetails = new TouchingPointsTrial(
                    trialValues[0],
                    trialValues[1],
                    trialValues[2],
                    trialValues[3],
                    format, 
                    trialValues[4],
                    trialValues[5],
                    trialValues[6],
                    trialValues[7],
                    trialValues[11]
                );
            }
            else if (Tasks.LargerTad.ToString() == trialConditions[3])
            {
                int trialID = Int32.Parse(trialConditions[6]);
                String[] trialValues = LargerTad.data[trialID].Split(',');
                task = Tasks.LargerTad;
                trialDetails = new LargerTadTrial(
                    trialValues[0],
                    trialValues[1],
                    trialValues[2],
                    format
                    );
            }
            else if (Tasks.Triple.ToString() == trialConditions[3])
            {
                int trialID = Int32.Parse(trialConditions[6]);
                String[] trialValues = Triple.data[trialID].Split(',');
                task = Tasks.Triple;
                trialDetails = new TripleTrial(
                    trialValues[0],
                    trialValues[1],
                    trialValues[2],
                    trialValues[3],
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