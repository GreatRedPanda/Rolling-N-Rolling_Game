using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GridSet : MonoBehaviour
{

    public bool UseFullScreenSize;
   

    public int GridSize;

   public Vector2 offset = Vector2.zero;
    public float LineSize = 0.1f;


    float scaleFactor;
    public float TileCoef = 100;
    Vector2 panningOffset = Vector2.zero;
    Material m;
    Vector2 size;
    RectTransform rectTransform;


    void Start()
    {
         m = GetComponent<Image>().material;
        rectTransform = GetComponent<RectTransform>();
         size = rectTransform.rect.size;
       // Vector2 scale = new Vector2(1, 1) * size/TileCoef;
       /// offset = new Vector2(1 - (scale.x / 2 - (int)(scale.x / 2)), 1 - (scale.y / 2 - (int)(scale.y / 2)));
     ///   SetProperties(offset.x, offset.y, scale.x, scale.y);
    }


    void SetProperties(float offsetX, float offsetY, float cellsCountX, float cellsCounty)
    {

        m.SetFloat("_OffsetX", offsetX);
        m.SetFloat("_OffsetY", offsetY);
        m.SetFloat("_CellCountX", cellsCountX);
        m.SetFloat("_CellCountY", cellsCounty);
    }
    public void SetGridSize(float currentScale, Vector2 coordsDelta)
    {
        size = rectTransform.rect.size;

        coordsDelta += size / 2;
        TileCoef += currentScale;

        Vector2 coordsPercent = coordsDelta / TileCoef;// *size / TileCoef;

        panningOffset = new Vector2(1 - (coordsPercent.x - (int)(coordsPercent.x)),
            1 - (coordsPercent.y - (int)(coordsPercent.y)));

        m.SetFloat("_OffsetX", panningOffset.x);
        m.SetFloat("_OffsetY", panningOffset.y);


        Vector2 scale = new Vector2(1, 1) * size / TileCoef;

        SetProperties(panningOffset.x,  panningOffset.y, scale.x, scale.y);

    }



}
