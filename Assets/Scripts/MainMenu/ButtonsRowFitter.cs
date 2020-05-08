using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonsRowFitter : MonoBehaviour
{

    RectTransform[] childrenElements;
    void Awake()
    {
        childrenElements = new RectTransform [transform.childCount];
        for (int i = 0; i < childrenElements.Length; i++)
        {
            childrenElements[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
       // CalculateSize();

      //  Debug.Log(childrenElements.Length); 
    }

 public   void CalculateSize()
 {

        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();


        Vector2 size = GetComponent<RectTransform>().rect.size;

        gridLayoutGroup.cellSize= Vector2.one* size.x / childrenElements.Length;




 }


    private void OnEnable()
    {
        CalculateSize();
    }
}
