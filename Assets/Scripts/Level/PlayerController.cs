using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{


    public CameraRotationBehaviour RotationBehaviour;
    public event System.Action<int> OnTouch;
  
    float coolDownTime = 0;
    Ray ray;
    [HideInInspector]
    public bool AllowToPlay = false;


    bool swiping = false;

    public float MouseDelta = 1.5f;
    bool isOverUI;


    Vector3 mouseprevPos;


    public Vector2 Speed;
    void Update()
    {

        inputStandalone();

         input();


    }
    bool rotationStarted = false;

    void input()
    {

            if (Input.touchCount == 1)
            {
                rotationStarted = false;
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                {
                    isOverUI = EventSystem.current.IsPointerOverGameObject(t.fingerId);
                    swiping = false;

                    RotationBehaviour.StartRotation(t);



                }
                else if (t.phase == TouchPhase.Moved)
                {
                    swiping = true;

                    RotationBehaviour.RotateCamera(t);

                }
                else if (t.phase == TouchPhase.Ended)
                {
                   
                }


            }
            else if (Input.touchCount == 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(1).phase == TouchPhase.Canceled
                   || Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
                    rotationStarted = false;
                
               // if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    if (!rotationStarted)
                    {
                        rotationStarted = true;
                        RotationBehaviour.StartRotation(Input.GetTouch(0), Input.GetTouch(1));
                    }
                    else
                    {
                        swiping = true;

                        bool zoom = false;
                        RotationBehaviour.ZoomOrRotate(Input.GetTouch(0), Input.GetTouch(1), ref zoom);
                    }
                }
                //}

            }
            else rotationStarted = false;


        

    }
    void inputStandalone()
    {
        float hInput = Input.GetAxis("Horizontal")*Speed.x;
        float vInput = Input.GetAxis("Vertical") * Speed.y;

        if(Mathf.Abs(hInput)>0.02f || Mathf.Abs(vInput) > 0.02f)
        RotationBehaviour.RotateCamera(hInput, vInput);

        if (Input.GetButtonDown("Fire1"))
        {
            isOverUI = EventSystem.current.IsPointerOverGameObject();
        }


        if (Input.GetButton("Fire1"))
        {


        }


        if (Input.GetButtonDown("Fire2"))
        {
            rotationStarted = true;
            RotationBehaviour.StartRotation(Input.mousePosition);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            rotationStarted = false;

        }
        if (Input.GetButton("Fire2"))
        {
            
            RotationBehaviour.RotateCamera(Input.mousePosition, mouseprevPos- Input.mousePosition);


        }

        float msd = Input.GetAxis("Mouse ScrollWheel");
        RotationBehaviour.Zoom(msd*20);
        mouseprevPos = Input.mousePosition;
    }


  


    bool CheckUIClick(Vector3 pos)
    {
        bool clickOnUIElement = false;

        Ray r = Camera.main.ScreenPointToRay(pos);
        if(Physics.Raycast(r, 200, LayerMask.GetMask("UI")))
            clickOnUIElement = true;



        //// bool clickOnUIElement = EventSystem.current.IsPointerOverGameObject();
        //Debug.Log("OverBtn"+ clickOnUIElement);
        return clickOnUIElement;
    }
    

}

