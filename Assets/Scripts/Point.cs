using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ColorMine.ColorSpaces;

namespace Assets.Scripts
{
    class Point
    {
        private float x;
        private float y;
        private float z;
        private string name;
        private int chromosome;
        private int start;
        private int end;
        private float color;
        private Color colorRGB = new Color(0,1,0);
        private Vector3 position;
        public Point(float x, float y, float z, string name, float color)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            position = new Vector3(x, y, z);
            this.name = name;
            string[] nameparts = name.Split('-');
            chromosome = Int32.Parse(nameparts[0].Substring(3));
            start = Int32.Parse(nameparts[1]);
            end = Int32.Parse(nameparts[2]);
            this.color = color;
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
            chromosome = Int32.Parse(nameparts[0].Substring(3));
            start = Int32.Parse(nameparts[1]);
            end = Int32.Parse(nameparts[2]);
            color = float.Parse(parameters[4]);
        }

        public void InitialiseRGBSequence(int current, int total)
        {
            float step = current / (float) total;
            colorRGB = new UnityEngine.Color(step, 1, (1-step));
            Debug.Log(colorRGB.ToString());
        }

        public void InitialiseRGBValue(int current, int total)
        {
            colorRGB = new Color(color, 1, (1 - color));
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

        public String Name
        {
            get { return name; }
        }
    }
}
