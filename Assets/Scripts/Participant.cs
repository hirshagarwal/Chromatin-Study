using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Participant
    {
        private int participantID;
        private List<string> trials = new List<string>();
        private int nextTrialLocation;
        private Formats format;

        public Participant(int participantID, Formats trialFormat, int firstTrial)
        {
            Debug.Log("Trial format is " + trialFormat.ToString());
            format = trialFormat;
            TextAsset file = Resources.Load("designfile") as TextAsset;
            this.participantID = participantID;
            nextTrialLocation = firstTrial;
            string[] blocks = file.text.Split(StudyManager.LINE_BREAK);
            string[] line;
            //            int participantCount = int.Parse(blocks[blocks.Length - 1].Split(',')[0]);

            for (int i = 1; i < blocks.Length; i++)
            {
                line = blocks[i].Split(',');
                if (line.Length == 0)
                    continue;
                if (int.Parse(line[0]) == participantID
                    && line[2] == trialFormat.ToString()
                )
                    trials.Add(blocks[i]);
            }
        }

        public int TotalTrials
        {
            get
            {
                return trials.Count;
            }
        }

        public Trial NextTrial()
        {
            return new Trial(trials[nextTrialLocation++], format);
        }
    }
}