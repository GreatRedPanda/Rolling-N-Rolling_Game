using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraRotationBehaviour : MonoBehaviour
{
    public int rotationDeltaRatio = 2;
    public float HRotateSpeed=1.6f;
    public float VRotateSpeed = 0.04f;
    public Vector2 ScaleClamp = new Vector2(2.5f, 20);
    public Vector2 AngleClamp = new Vector2(2.5f, 20);

    public float XSmoothDampSpeed = 10;
    public float YSmoothDampSpeed = 10;
    public float ScaleSpeed = 0.002f;
    public GameObject RotateObj;
  public float Distance { get { return radius; } }



    static public float turnAngleDelta;
    const float pinchTurnRatio = Mathf.PI / 2;
    const float minTurnAngle = 0;
    static public float turnAngle;

    Vector2 prevInput;

    Vector2 newInput;
    bool inputStart = false;

  
    Camera cam;

    float radius = 10;


    Quaternion targetLocalRotation;
    Vector3 targetLocalPosition;


    Vector3 targetEulers;
    float currentYRotation;

    float angleX = 0;
    float angleY = 0;

    float scale = 1;
  
    Vector2 endTouch;

   
    Vector2 endTouchT1;
  
    Vector2 endTouchT2;

 float dir = 1;

    bool dirSwitched = false;


    void Start()
    {
     
        cam = GetComponentInChildren<Camera>();
        SetToPosition(Vector3.zero);

    }

    public void SetToPosition(Vector3 pos, float maxDistance=10)

    {
        cam = GetComponentInChildren<Camera>();
   
        transform.position = pos;
        radius = maxDistance;
        scale = ScaleClamp.y;
        transform.localScale = Vector3.one * ScaleClamp.y;


        angleX = Mathf.Clamp(angleX, AngleClamp.x, AngleClamp.y);
        var localPos = new Vector3(Mathf.Cos(angleX) * radius, Mathf.Sin(angleX) * radius,
            Mathf.Cos(angleX) * radius);

        var posCam = transform.TransformPoint(localPos);
        var lookPos = transform.position - posCam;
        var rotation = Quaternion.LookRotation(lookPos);

        cam.transform.rotation = rotation;
        cam.transform.localPosition = localPos;

        targetLocalRotation = rotation;
        targetLocalPosition = localPos;
        //rotationAroundTarget = transform.localRotation;
    }


  



    public void StartRotation(Touch t)
    {
       
        endTouch = t.position;
    }

    public void StartRotation(Vector3 position)
    {

        endTouch = position;
    }
    public void StartRotation(Touch t1, Touch t2)
    {
        dir = 0;
       
        endTouchT2 =   t2.position; //

        endTouchT1 =  t1.position;  //

        //Debug.Log(t2.position.y + "AGENTP " + t1.position.y);
        float diffY = Mathf.Abs(t2.position.y - t1.position.y);
        float div = t1.position.y / t2.position.y;

        if (t2.position.y > t1.position.y)
        {
            div = t2.position.y / t1.position.y;
        }
        //Debug.Log("DIVISION" + div);
        if (div >= 1.3f)
        { dir = -1; }
        else if(div <= 1.2f)
            dir = 1;
    }
   
    public void RotateCamera(Touch t)
    {
        Vector2 TouchDirection = t.deltaPosition;
        float difX = Mathf.Abs(endTouch.x - t.position.x);
        float difY = Mathf.Abs(endTouch.y - t.position.y);

        if (difY > difX)
        {
            float newAngle = angleX + -TouchDirection.y * Time.deltaTime * VRotateSpeed * RollingLogic.Instance.CurrentSpeed;       
            int prevYangle = Mathf.RoundToInt(cam.transform.eulerAngles.y);
            var localPos = new Vector3(Mathf.Cos(newAngle) * radius, Mathf.Sin(newAngle) * radius,
                Mathf.Cos(newAngle) * radius);
            var pos = transform.TransformPoint(localPos);
            var lookPos = transform.position - pos;
            var rotation = Quaternion.LookRotation(lookPos);  
            int newYangle = Mathf.RoundToInt(rotation.eulerAngles.y);
                angleX = newAngle;
            targetLocalRotation = rotation;
            targetLocalPosition = localPos;
        }
        else
        {
            targetEulers += Vector3.up * TouchDirection.x * HRotateSpeed * RollingLogic.Instance.CurrentSpeed * Time.deltaTime;
        }
    }
    public void RotateCamera(float hInput, float vInput)
    {
        rotate(hInput, vInput);
    }
    public void RotateCamera(Vector3 mousePos, Vector2  deltaPosition)
    {
 
        Vector2 TouchDirection = deltaPosition;
        rotate(deltaPosition.x, deltaPosition.y);
    }
    void rotate(float dx, float dy)
    {
        float difX = Mathf.Abs(dx);
        float difY = Mathf.Abs(dy);
        if (difY > difX)
        {
            float newAngle = angleX + -(dy) * Time.deltaTime * VRotateSpeed * RollingLogic.Instance.CurrentSpeed;// *0.4f;

            newAngle = Mathf.Clamp(newAngle, AngleClamp.x, AngleClamp.y);
            var localPos = new Vector3(Mathf.Cos(newAngle) * radius, Mathf.Sin(newAngle) * radius, Mathf.Cos(newAngle) * radius);

            var pos = transform.TransformPoint(localPos);
            var lookPos = transform.position - pos;
            var rotation = Quaternion.LookRotation(lookPos);
            int newYangle = Mathf.RoundToInt(rotation.eulerAngles.y);
            angleX = newAngle;
            targetLocalRotation = rotation;
            targetLocalPosition = localPos;
        }
        else
        {
            targetEulers += Vector3.up * dx * HRotateSpeed * RollingLogic.Instance.CurrentSpeed * Time.deltaTime;
                  
        }
    }




    public float ZoomOrRotate(Touch touchZero, Touch touchOne, ref bool zoom)
    {
        float value = 0;

        float dif1 = Mathf.Abs(endTouchT1.x - touchZero.position.x);
        float dif2 = Mathf.Abs(endTouchT1.y - touchZero.position.y);
        float dif3 = Mathf.Abs(endTouchT2.x - touchOne.position.x);
        float dif4 = Mathf.Abs(endTouchT2.y - touchOne.position.y);
        bool rotate = false;


 
        if (dir == 1)
            {          
                value = Zoom(touchZero, touchOne);
            zoom = true;
            }
       else if (dir == -1)
            {
             value=   Rotate(touchZero, touchOne);
            zoom = false;
            }
        return value;
    }

    public float Zoom(Touch touchZero, Touch touchOne)
    {

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
        //cam.fieldOfView += deltaMagnitudeDiff * 0.2f;
        scale += deltaMagnitudeDiff * ScaleSpeed;

        // Clamp the field of view to make sure it's between 0 and 180.
        //cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
        scale = Mathf.Clamp(scale, ScaleClamp.x, ScaleClamp.y);
        transform.localScale = Vector3.one * scale;
        return scale;
    }

    public float Zoom(float mouseScrollDelta)
    {



        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = mouseScrollDelta;
        //cam.fieldOfView += deltaMagnitudeDiff * 0.2f;
        scale += deltaMagnitudeDiff * ScaleSpeed;

        // Clamp the field of view to make sure it's between 0 and 180.
        //cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
        scale = Mathf.Clamp(scale, ScaleClamp.x, ScaleClamp.y);
        transform.localScale = Vector3.one * scale;
       // cam.transform.LookAt(transform.position);
        return scale;
    }



    float Rotate(Touch touch1, Touch touch2)
    {
        Quaternion desiredRotation = transform.rotation;
        turnAngle = turnAngleDelta = 0;

        turnAngle = Angle(touch1.position, touch2.position);
        float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                               touch2.position - touch2.deltaPosition);
        turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

        // ... if it's greater than a minimum threshold, it's a turn!
        if (Mathf.Abs(turnAngleDelta) > minTurnAngle)
        {
            turnAngleDelta *= pinchTurnRatio;
        }
        else
        {
            turnAngle = turnAngleDelta = 0;
        }
        if (Mathf.Abs(turnAngleDelta) > 0)
        { // rotate
            Vector3 rotationDeg = Vector3.zero;
            rotationDeg.z = -turnAngleDelta;
            desiredRotation *= Quaternion.Euler(rotationDeg);
        }

        targetEulers += Vector3.up * turnAngleDelta * HRotateSpeed * RollingLogic.Instance.CurrentSpeed * Time.deltaTime;
       // transform.RotateAround(transform.position, Vector3.up, turnAngleDelta * HRotateSpeed * RollingLogic.Instance.currentSpeed);

        return turnAngleDelta;
    }


    private void Update()
    {
        smoothCamera();
    }


    void smoothCamera()
    {

        float rotationYVelocity = 0;
        Vector3 vel = Vector3.zero;
        cam.transform.LookAt(transform.position);
        cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, targetLocalPosition, ref vel, Time.deltaTime* XSmoothDampSpeed);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, targetEulers.y, ref rotationYVelocity, Time.deltaTime * YSmoothDampSpeed);
        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
      
    }
    static private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }
}


