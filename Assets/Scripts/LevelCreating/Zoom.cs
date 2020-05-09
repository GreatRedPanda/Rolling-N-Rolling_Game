using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Zoom : MonoBehaviour
{

    public CanvasScaler Scaler;
   

    public float maxScale;
    public float minScale;

    public RectTransform imageToZoom;

    public GridSet GridSet;

    public float mouseSensitivity = 1;
    private Vector3 lastPosition;

   
    public float GridZoomSpeed = 1;
    public float GridPanSpeed = 1;

    float prevScale = 1;
    [SerializeField]  float currentScale = 1;
    Vector3 lastImgPos;
    bool startPanning = false;



    void Zooming(ref float increment)
    {
        currentScale += increment;
        if (currentScale >= maxScale)
        {
            increment = 0;
            currentScale = maxScale;
        }
        else if (currentScale <= minScale)
        {
            increment = 0;
            currentScale = minScale;
        }
        imageToZoom.localScale = new Vector2(currentScale, currentScale);
        
      
    }
    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Zooming(ref scrollInput);

        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0) && !isOverUI)
        {
            lastPosition = Input.mousePosition;
            startPanning = true;
        }

        if (Input.GetMouseButton(0) && startPanning)
        {
            var delta = Input.mousePosition - lastPosition;
            imageToZoom.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
            lastImgPos = imageToZoom.transform.position;
        }
        else
            startPanning = false;


        GridSet.SetGridSize(scrollInput * GridZoomSpeed, (imageToZoom.anchoredPosition));// + new Vector2(Screen.width/2, Screen.height/ 2)));
        prevScale = currentScale;
    }
    public void ToCenter()
    {
        imageToZoom.localPosition = Vector2.zero;

    }


 
}
