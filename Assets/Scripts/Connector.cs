using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    class Connector
    {
        private Vector3 startPoint;
        private Vector3 endPoint;
        private Color interpolatedColor;

        public Connector(Point start, Point end)
        {
            startPoint = start.Position;
            endPoint = end.Position;
            interpolatedColor = (start.ColorRGB + end.ColorRGB) / 2;
        }

        public Vector3 StartPoint
        {
            get { return startPoint; }
        }

        public Vector3 EndPoint
        {
            get { return endPoint; }
        }

        public Color InterpolatedColor
        {
            get { return interpolatedColor; }
        }
    }
}
