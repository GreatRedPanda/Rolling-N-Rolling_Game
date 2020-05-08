using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathSpline : MonoBehaviour
{

    public bool loop = true;
    public int Count { get { return Positions.Count; } }
    public float Length { get { return GetLengthApproximately(0f, 1f); } }
    public List<Vector3> Positions;
    float[]  arcs ;
    float[] timeVals;

    float totalLength;
    public Vector3 this[int index]
    {
        get
        {
            if (index < Count)
                return Positions[index];

       
            return Vector3.zero;
        }
    }



    public void InitialiazePath(Vector3[] positions)
    {

        Positions = new List<Vector3>(positions);

        arcs = new float[positions.Length];
        timeVals=new float[positions.Length];
        float cLen = 0;

       
        Vector3 prevPoint = GetPoint(0);
        arcs[0] = 0;
        for (int i = 1; i <positions.Length; i++)
        {
            timeVals[i] = (float)i / (positions.Length - 1);
            Vector3 nextP = GetPoint(timeVals[i]);

            Vector3 dP = nextP-prevPoint;
            cLen += dP.magnitude;
          
            arcs[i] = cLen;

         //   Debug.Log(i * 1f / positions.Length + "    " + cLen + "      " + dP + "     " + nextP + "     " + prevPoint);
            prevPoint = nextP;
        }
        totalLength = cLen;
    }



    public float GetParametrizedTime(float[] _arcLengths, float u)
    {
        float t;
        float targetArcLength = u * _arcLengths[_arcLengths.Length - 1];

        // the next function would be a binary search, for efficiency
        int index = Array.BinarySearch(_arcLengths, targetArcLength);
        if (index < 0)
            index = ~index;

      //  Debug.Log(index+"   "+u+"    "+ targetArcLength+"    "+_arcLengths[_arcLengths.Length - 1]);
        // if exact match, return t based on exact index
        if (_arcLengths[index] == targetArcLength)
        {
            t = index / (float)(_arcLengths.Length );//- 1
        }
        else  // need to interpolate between two points
        {
            float lengthBefore = _arcLengths[index];
            float lengthAfter = _arcLengths[index + 1];
            float segmentLength = lengthAfter - lengthBefore;

            // determine where we are between the 'before' and 'after' points.
            float segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

            // add that fractional amount to t 
            t = (index + segmentFraction) / (float)(_arcLengths.Length );//- 1
        }

        //Debug.Log(index+"    index");
     //   float t1 = Mathf.Lerp(timeVals[index - 1], timeVals[index],(targetArcLength - _arcLengths[index - 1]) / (_arcLengths[index] - _arcLengths[index - 1]));
        return t;
    }
    public int IndexOf(Vector3 point)
    {
        return Positions.IndexOf(point);
    }
    public Vector3 MoveAlongSplineByArcs(ref float distanceProgress, float deltaMovement,Vector3 position, int accuracy = 3)
    {

        float deltaMovePercent = deltaMovement / totalLength;
        float newDist = distanceProgress + deltaMovePercent;

        float time = newDist * deltaMovePercent;

        float startSpeed = 0;
        float endSpeed = distanceProgress;
        float acceleration = endSpeed - startSpeed;
        float pos = 0.5f * acceleration* deltaMovement * deltaMovement + startSpeed* deltaMovement;
        float maxPos = 0.5f * acceleration +startSpeed;
        float newT = pos /( maxPos );
  
        //Debug.Log(time);

        float paramT = GetParametrizedTime(arcs, time);
        Vector3 point = GetPoint(paramT);
    
        distanceProgress = newDist;
        return point;
    }
    public Vector3 MoveAlongSpline(ref float normalizedT, float deltaMovement, int accuracy = 3)
    {

        float _1OverCount = 1f / Positions.Count;

        for (int i = 0; i < accuracy; i++)
            normalizedT += deltaMovement * _1OverCount / (accuracy);

        return GetPoint(normalizedT);
    }
    public Vector3 GetPoint(float normalizedT)
    {
        if (normalizedT <= 0f)
            return Positions[0];
        else if (normalizedT >= 1f)
        {
            if (loop)
                return Positions[0];

            return Positions[Positions.Count - 1];
        }

        float t = normalizedT * (loop ? Positions.Count : (Positions.Count - 1));

        Vector3 startPoint, endPoint;

        int startIndex = (int)t;
        int endIndex = startIndex + 1;

        if (endIndex == Positions.Count)
            endIndex = 0;

        startPoint = Positions[startIndex];
        endPoint = Positions[endIndex];

        float localT = t - startIndex;
        float oneMinusLocalT = 1f - localT;
        
        return oneMinusLocalT * oneMinusLocalT * oneMinusLocalT * startPoint +
                 3f * oneMinusLocalT * oneMinusLocalT * localT * startPoint +
              3f * oneMinusLocalT * localT * localT * endPoint +
               localT * localT * localT * endPoint;
    }


    public float GetLengthApproximately(float startNormalizedT, float endNormalizedT, float accuracy = 50f)
    {
        if (endNormalizedT < startNormalizedT)
        {
            float temp = startNormalizedT;
            startNormalizedT = endNormalizedT;
            endNormalizedT = temp;
        }

        if (startNormalizedT < 0f)
            startNormalizedT = 0f;

        if (endNormalizedT > 1f)
            endNormalizedT = 1f;

        float step = AccuracyToStepSize(accuracy) * (endNormalizedT - startNormalizedT);

        float length = 0f;
        Vector3 lastPoint = GetPoint(startNormalizedT);
        for (float i = startNormalizedT + step; i < endNormalizedT; i += step)
        {
            Vector3 thisPoint = GetPoint(i);
            length += Vector3.Distance(thisPoint, lastPoint);
            lastPoint = thisPoint;
        }

        length += Vector3.Distance(lastPoint, GetPoint(endNormalizedT));

        return length;
    }

    public Vector3 FindNearestPointTo(Vector3 worldPos, float accuracy = 100f)
    {
        float normalizedT;
        return FindNearestPointTo(worldPos, out normalizedT, accuracy);
    }

    public Vector3 FindNearestPointTo(Vector3 worldPos, out float normalizedT, float accuracy = 100f)
    {
        Vector3 result = Vector3.zero;
        normalizedT = -1f;

        float step = AccuracyToStepSize(accuracy);

        float minDistance = Mathf.Infinity;
        for (float i = 0f; i < 1f; i += step)
        {
            Vector3 thisPoint = GetPoint(i);
            float thisDistance = (worldPos - thisPoint).sqrMagnitude;
            if (thisDistance < minDistance)
            {
                minDistance = thisDistance;
                result = thisPoint;
                normalizedT = i;
            }
        }

        return result;
    }
    private float AccuracyToStepSize(float accuracy)
    {
        if (accuracy <= 0f)
            return 0.2f;

        return Mathf.Clamp(1f / accuracy, 0.001f, 0.2f);
    }

}
