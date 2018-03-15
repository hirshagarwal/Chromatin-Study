using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Point : IComparable<Point>
    {
        private float x;
        private float y;
        private float z;
        private string name;
        private string chromosome;
        private int start;
        private int end;
        private float color;
        private bool isRed = false;
        private bool isBlue = false;
        private bool isYellow = false;
        private Color colorRGB = Design.GetClosestColor(0.5f);
        private Vector3 position;

        public Point(float x, float y, float z, string name, float color)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            position = new Vector3(x, y, z);
            this.name = name;
            string[] nameparts = name.Split('-');
            chromosome = (nameparts[0].Substring(3));
            start = Int32.Parse(nameparts[1]);
            end = Int32.Parse(nameparts[2]);
            this.color = color;
        }

        public Point(float x, float y, float z, string name, float color, Color colorRGB)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            position = new Vector3(x, y, z);
            this.name = name;
            string[] nameparts = name.Split('-');
            chromosome = (nameparts[0].Substring(3));
            start = Int32.Parse(nameparts[1]);
            end = Int32.Parse(nameparts[2]);
            this.color = color;
            this.colorRGB = colorRGB;
        }

        public Point(string line)
        {
            string[] parameters = line.Split('\t');

            name = parameters[0];
            x = float.Parse(parameters[1]);
            y = float.Parse(parameters[2]);
            z = float.Parse(parameters[3]);
            position = new Vector3(x, y, z);
            string[] nameparts = name.Split('-');
            chromosome = (nameparts[0].Substring(3));
            start = Int32.Parse(nameparts[1]);
            end = Int32.Parse(nameparts[2]);
            color = float.Parse(parameters[4]);
        }

        public Point Interpolate(Point other)
        {
            return new Point(
                (x + other.X) / 2f,
                (y + other.Y) / 2f,
                (z + other.Z) / 2f,
                name,
                (color + other.Color) / 2f
                );
        }

        public Point Displaced(Vector3 displacement)
        {
            return new Point(
                x + displacement.x,
                y + displacement.y,
                z + displacement.z,
                name,
                color,
                colorRGB);
        }

        public void InitialiseRGBSequence(int current, int total)
        {
            float step = current / (float)total;
            colorRGB = new UnityEngine.Color(step, 1, (1 - step));
            Debug.Log(colorRGB.ToString());
        }

        public void InitialiseRGBValue(int current, int total)
        {
            colorRGB = Design.GetClosestColor(color);
        }

        int IComparable<Point>.CompareTo(Point other)
        {
            if (this < other)
            {
                return -1;
            }
            if (this > other)
            {
                return 1;
            }
            if (this <= other)
            {
                return -1;
            }
            if (other <= this)
            {
                return 1;
            }
            return 0;
        }

        public static bool operator <(Point p1, Point p2)
        {
            return p1.End < p2.Start;
        }

        public static bool operator >(Point p1, Point p2)
        {
            return p1.Start > p2.End;
        }

        public static bool operator >=(Point p1, Point p2)
        {
            return p1.End > p2.End;
        }

        public static bool operator <=(Point p1, Point p2)
        {
            return p1.Start < p2.Start;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return (p1.Start == p2.Start) && (p1.End == p2.End);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return (p1.Start != p2.Start) || (p1.End != p2.End);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + start.GetHashCode();
            hash = (hash * 7) + end.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
            {
                var otherPoint = obj as Point;
                return (start == otherPoint.Start) && (end == otherPoint.End);
            }
            return false;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Color ColorRGB
        {
            get { return colorRGB; }
            set { colorRGB = value; }
        }

        public float Color
        {
            get { return color; }
        }

        public int Start
        {
            get { return start; }
        }

        public int End
        {
            get { return end; }
        }

        public float X
        {
            get { return x; }
        }

        public float Y
        {
            get { return y; }
        }

        public float Z
        {
            get { return z; }
        }

        public void MakeYellow()
        {
            isYellow = true;
        }

        public void MakeRed()
        {
            isRed = true;
        }

        public void MakeBlue()
        {
            isBlue = true;
        }

        public String Name
        {
            get { return name; }
        }

        public bool HasColor()
        {
            return isRed || isBlue || isYellow;
        }

        public bool IsRed
        {
            get
            {
                return isRed;
            }
        }

        public bool IsBlue
        {
            get
            {
                return isBlue;
            }
        }

        public bool IsYellow
        {
            get
            {
                return isYellow;
            }
        }
    }
}