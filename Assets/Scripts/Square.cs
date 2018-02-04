using System;

namespace Assets.Scripts
{
    internal class Square
    {
        private Range a;
        private Range b;
        private float strength;

        public Square(string line)
        {
            string[] parameters = line.Split('\t');
            int aStart = Int32.Parse(parameters[1]);
            int aEnd = Int32.Parse(parameters[2]);
            a = new Range(aStart, aEnd);
            int bStart = Int32.Parse(parameters[4]);
            int bEnd = Int32.Parse(parameters[5]);
            b = new Range(bStart, bEnd);
            strength = float.Parse(parameters[6]);
        }

        public Range A
        {
            get { return a; }
        }

        public Range B
        { get { return b; } }

        public float Strength
        {
            get
            {
                return strength;
            }
        }
    }
}