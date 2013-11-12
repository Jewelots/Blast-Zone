using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace VectorGraphs
{
    /// <summary>
    /// makes a 2D graph, for getting a value over time (0.0 > 1.0)
    /// </summary>
    public interface VectorGraph
    {
        /// <summary>
        /// get the Y value for the X point given,
        /// </summary>
        /// <param name="Xval">X point on the graph</param>
        /// <returns>the Y value</returns>
        float GetValue(float Xval);

        void AddPoint(float X, float Y);
        void AddPoint(Vector2 vector);
        void RemovePoint(float X);
    }

    /// <summary>
    /// an empty graph,
    /// </summary>
    public class EmptyVectorGraph : VectorGraph
    {
        public float GetValue(float Xval) { return 1.0f; }

        public void AddPoint(float X, float Y) { }
        public void AddPoint(Vector2 vector) { }
        public void RemovePoint(float X) { }
    }

    /// <summary>
    /// a basic multipoint graph
    /// </summary>
    public class BasicVectorGraph : VectorGraph
    {
        List<Vector2> PointList;

        public BasicVectorGraph() { PointList = new List<Vector2>(); }

        public void AddPoint(float X, float Y) { PointList.Add(new Vector2(X, Y)); }
        public void AddPoint(Vector2 vector) { PointList.Add(vector); }
        //public void RemovePoint(float X) { PointList.Remove(X); }
        public void RemovePoint(float X) { }

        public float GetValue(float Xval)
        {
            float Yval = 0.0f; Vector2 tempVector;
            //get upper and lower points
            Vector2 upper = new Vector2(1.0f), lower = new Vector2(0.0f);
            foreach (Vector2 point in PointList)
                if (point.X >= Xval)
                {
                    upper = point;
                    lower = PointList.ElementAt(PointList.IndexOf(point) - 1);
                    break;
                }

            float mag = (float)Math.Sqrt((lower.X * lower.X) + (upper.Y * upper.Y));

            tempVector = Vector2.Lerp(lower, upper, (Xval - lower.X) / mag);
            Yval = tempVector.Y;

            return Yval;
        }

    }

    /// <summary>
    /// a bezier graph
    /// </summary>
    public class BezierVectorGraph : VectorGraph
    {
        List<Vector2> PointList;

        public BezierVectorGraph() { PointList = new List<Vector2>(); }

        public void AddPoint(float X, float Y) { PointList.Add(new Vector2(X, Y)); }
        public void AddPoint(Vector2 vector) { PointList.Add(vector); }
        public void RemovePoint(float X) {  }

        public float GetValue(float Xval)
        {
            float Yval = 0.0f;

            //make a lerp list, copy points in
            List<Vector2> lerpList = new List<Vector2>();

            foreach(Vector2 vect in PointList)
                lerpList.Add(vect);
            
            //float lowerKey = PointList.Min().X;
            //float upperKey = PointList.Max().X;
            //float mag = (float)Math.Sqrt((lowerKey * lowerKey) + (upperKey * upperKey));
            //float normXval = (Xval - lowerKey) / mag;

            //repeatedly lerp each with its neighbor
            //remove the start one each time
            //remove the start after the loop and go over again
            //repeat unltill only one left
            while (lerpList.Count > 1)
            {
                int count = lerpList.Count;
                for (int i = 0; i < count - 1; ++i)
                {
                    lerpList.Add(Vector2.Lerp(lerpList.ElementAt(0), lerpList.ElementAt(1), Xval));
                    lerpList.RemoveAt(0);
                }
                lerpList.RemoveAt(0);
            }

            Yval = lerpList.Last().Y;


            return Yval;
        }
    }

}
