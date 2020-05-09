using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using BezierSolution;
using System;

public class PathGenerator : MonoBehaviour
{

    public LineRenderer Path;
    public LineRenderer TrailPainter;
    public BoxCollider Box;



    public void CreateLevelPath(BezierSpline Spline, LevelData level, float[] radiuses, float deltaY)
    {
        Vector3[] positions = GetPositionsIn3D(Box.bounds, level, radiuses, deltaY).ToArray();
        Path.positionCount = positions.Length;
        Path.SetPositions(positions);
       
        Spline.Initialize(positions.Length); 
        Spline.loop = true;     
        for (int i = 0; i < positions.Length; i++)
        {
            BezierPoint bp = Spline[i];
            positions[i]+=Vector3.up* Path.transform.position.y;

            bp.position = positions[i];
        }

        Spline.ConstructLinearPath();
        
    }


    List<Vector3> GetPositionsIn3D(Bounds bounds, LevelData level, float[] radiuses, float deltaY)
    {

        float[] angles = new float[level.StartAngles.Length];// (level.StartAngles);
        Array.Copy(level.StartAngles, angles, angles.Length);
        int[] speeds = level.Speeds;
        Vector3 coef;
        Vector3 size = bounds.size;
        Vector3 center = bounds.center;
        List<Vector3> positions = new List<Vector3>();
        float delta = 0.002f;
        float Y = 0;
        float maxRadius = 0;

        for (int i = 0; i < radiuses.Length; i++)
        {
           
               maxRadius += radiuses[i];
        }
         coef = size / (2*maxRadius);
        coef.y = size.y/2 * delta;///(Mathf.Abs(delta) *2* Mathf.PI /10);
        for (float t = 0; t <= 2 * Mathf.PI/10; t += delta)
        {
            float x = 0;
            float z = 0;
            int prevSpeed = 0;
            for (int i = 0; i < radiuses.Length; i++)
            {
                prevSpeed += speeds[i];
                angles[i] += (prevSpeed)  * delta*10;// * 2 * Mathf.PI * 10 / (360);

                x += radiuses[i] * Mathf.Cos(angles[i]);
                z += radiuses[i] * Mathf.Sin(angles[i]);
            
            }

            Vector3 position = new Vector3(x*coef.x, Y * coef.y, z * coef.z);

            positions.Add(position);
            if (t >= Mathf.PI / (10)-delta)
                deltaY = 5;
            Y += deltaY;

          
        }
        return positions;

    }

    public static Vector3[] GetPositions(int[] speeds, float[] radiuses, float[] startAngles)
    {
        float[] angles = new float[startAngles.Length] ;
        Array.Copy(startAngles, angles, angles.Length);
        List<Vector3> positions = new List<Vector3>();
        float delta = 0.005f;
        float prevX = 0;
        float prevY = 0;

        for (float t = 0; t <= 2 * Mathf.PI ; t += delta * Rotator.AllSpeed * Mathf.PI / (180))
        {
            float x = 0;
            float y = 0;
            int prevSpeed = 0;
            for (int i = 0; i < radiuses.Length; i++)
            {
                prevSpeed += speeds[i];
                angles[i] += (prevSpeed)  * delta * Rotator.AllSpeed * Mathf.PI / (180);

                x += radiuses[i] * Mathf.Cos(angles[i]);
                y += radiuses[i] * Mathf.Sin(angles[i]);
            }
            prevX = x;
            prevY = y;

            positions.Add(new Vector3(x, y, 0));
        }


        return positions.ToArray();

    }
        public static    Vector3[] GetPositions(LevelData level, float[] radiuses)
    {

        return GetPositions(level.Speeds, radiuses, level.StartAngles);

    }



 public static Texture2D MakeTexture(Vector3[] positions, float[] radiuses, int side=512)
    {

      
        int half = side / 2-1;
        
        Texture2D texture2D = new Texture2D(side, side, TextureFormat.ARGB32, false, true);
       
        texture2D.filterMode = FilterMode.Bilinear;

        Color[] colors = new Color[side * side];


        for (int i = 0; i < side*side; i++)
        {
            colors[i] = Color.clear;
        }

        texture2D.SetPixels(colors);
        float maxRadius = 0;
        for (int i = 0; i < radiuses.Length; i++)
        {
            maxRadius += radiuses[i];
        }
        float XCoef = (side-1) / (2 * maxRadius);
        //	texture.format=TextureFormat.
        for (int i = 0; i < positions.Length; i++)
        {
            Vector2 pos = new Vector2(positions[i].x, positions[i].y);
            pos *= XCoef;
            pos += new Vector2(half, half);
            int x = (int)pos.x;
            int y = (int)pos.y;



            for (int k = x - 2; k < x + 2; k++)
            {
                for (int n = y - 2; n < y + 2; n++)
                    if (k < side - 1 && k >= 0 )
                    {
                        //&& Mathf.Sqrt(k*k+n*n)<=4
                        texture2D.SetPixel(k, n, Color.white);
                    }
            }
            //if (i < positions.Length - 1)
            //{

            //    Vector2 posNext = new Vector2(positions[i + 1].x, positions[i + 1].y);
            //    posNext *= XCoef;
            //    posNext += new Vector2(half, half);
            //    int xN = (int)posNext.x;
            //    int yN = (int)posNext.y;

            //     DrawLine(texture2D, new Vector2(x, y), new Vector2(xN, yN), Color.white);
            //    //setLineOnTexture(texture2D, new Vector2Int(x, y), new Vector2Int(xN, yN), Color.white);
            //}
        }
        texture2D.Apply();

        return texture2D;
    }

    public static void DrawLine( Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2,ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }

    static void setLineOnTexture(Texture2D tex, Vector2Int p1, Vector2Int p2, Color col)
    {
        int dx =Mathf.Abs(p2.x - p1.x);
        int stepX = (p1.x < p2.x) ?1 : -1;
        int dy = Mathf.Abs(p2.y - p1.y);
        int stepY = (p1.y < p2.y) ? 1 : -1;

        int error = (dx > dy ? dx : -dy) / 2;



        while (true)
        {
            for (int k = p1.x - 2; k < p1.x + 2; k++)
            {
                for (int n = p1.y - 2; n < p1.y + 2; n++)
                    if (k < tex.width - 1 && k >= 0)
                        tex.SetPixel(k, n, col);
            }

            
            if (p1.x == p2.x && p1.y == p2.y)
                break;
            int e2 = error ;
            if (e2 < dy)
            {

              error += dx;
                p1.y += stepY;
            }

            if (e2 > -dx)
            {   error -= dy;
                 p1.x += stepX;
              
            }
        }

    }
}
