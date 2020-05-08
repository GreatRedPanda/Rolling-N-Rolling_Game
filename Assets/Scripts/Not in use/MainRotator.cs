using UnityEngine;
using System.Collections;

public class MainRotator : Rotator
{

    public event System.Action OnMadeFullCircle;
    public Mesh TrailMesh;
    public LineRenderer LineRenderer;
    float startAngle;
    int rotCount = 0;

    public bool ChevkingSolution;
    // Use this for initialization
    void Start()
    {

    }

    float prevAngle;
    // Update is called once per frame
   protected override void Update()
    {
        float angle = Time.deltaTime * Direction * Speed  * AllSpeed;


      
      
       // transform.Rotate(0, 0, angle);
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z+angle);
        if (ChevkingSolution)
        {
            startAngle += angle;


            if (Mathf.Abs((int)startAngle) / 360 > 0)
            {
                rotCount++;
                startAngle = 0;

            }


            if (Speed != 0)
                if ((Speed % 2 != 0 && rotCount == Speed) || (Speed % 2 == 0 && rotCount == (Speed / 2)))
                {
                    //CycleEnd();
                    //gameObject.SetActive(false);

                  //  OnMadeFullCircle?.Invoke();
                }
        }
        
    }


    void CycleEnd()
    {

        TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
     //   tr.BakeMesh(TrailMesh);
       
       Vector3[] positions =new Vector3[tr.positionCount];
       tr.GetPositions(positions);
        //Debug.Log(positions.Length);
        LineRenderer.positionCount = positions.Length;
        LineRenderer.SetPositions(positions);
       // LineRenderer.
    }
}


//x =RSin
