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
        private float color;
        private Color colorRGB = new Color(0,1,0);
        public Point(float x, float y, float z, string name, float color)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.name = name;
            this.color = color;
        }

        public Point(string line)
        {
            string[] parameters = line.Split('\t');
            name = parameters[0];
            x = float.Parse(parameters[1]);
            y = float.Parse(parameters[2]);
            z = float.Parse(parameters[3]);
            color = float.Parse(parameters[4]);
        }

        public void InitialiseRGB(int current, int total)
        {
            float step = total / 15f;
            float h = current * step / 360f;
            Hsv hsv = new Hsv(h, 1, 1);
            Rgb rgb = hsv.To<Rgb>();
            colorRGB = new Color((float)rgb.R/255f, (float)rgb.G/255f, (float)rgb.B/255f);
            Debug.Log(colorRGB.ToString());
        }

        public Vector3 Position()
        {
            return new Vector3(x, y, z);
        }

        public Color ColorRGB
        {
            get { return colorRGB; }
        }

        public float X
        {
            get;
        }

        public float Y
        {
            get;
        }

        public float Z
        {
            get;
        }

        public string Name
        {
            get;
        }

        public float Color
        {
            get;
        }

    }
}
