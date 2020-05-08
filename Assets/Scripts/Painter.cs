using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Painter : MonoBehaviour
{
    public RectTransform ArrowsPArent;
    public RectTransform LastArrow;
    public Image BackGround;   
 public   Texture2D background;
 
    public Color PaintColor;

     Texture2D canva;


    Queue<Vector2Int> prevPoses = new Queue<Vector2Int>();
    public Texture2D Brush;

    int countToDelete=20;
    Vector3 startPos;

    bool hasStartPos = false;
    bool draw = true;
    // Use this for initialization
    void Start()
    {
        canva = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        for (int i = 0; i < Screen.width; i++)
        {
            for (int j = 0; j < Screen.height; j++)
            {
               // pixels[i * radius * 2 + j] = PaintColor;
                canva.SetPixel(i, j, Color.clear);
            }
        }
        canva.Apply();
        BackGround.sprite=Sprite.Create(canva, new Rect(0,0, Screen.width, Screen.height), Vector2.zero);
       
    }

    // Update is called once per frame
    void Update()
    {
     
          
            Vector3 pos =(LastArrow.position)+(new Vector3(LastArrow.rect.width, LastArrow.rect.height,0))/2;
       // if(draw)
                 Paint(pos, 2);
        //if (!hasStartPos)
        //{
        //    startPos = LastArrow.position;
        //    hasStartPos = true;
        //}
        //else
        //{

        //    if (Vector2.Distance(pos, startPos) < 1.05f)

        //    {
        //        ArrowsPArent.gameObject.SetActive(false);
        //        draw = false;
        //    }

        //}


    }


    void Paint(Vector3 center, int radius)
    {

        //for (int i = (int)center.x-radius; i < radius+(int)center.x; i++)
        //{
        //    for (int j = (int)center.y-radius; j < radius + (int)center.y; j++)
        //    {
        //        // pixels[i * radius*2 + j] = PaintColor;
        //        Color c = PaintColor;
        //        c.a = Vector2.Distance(center, new Vector2(i,j))/radius;
        //        canva.SetPixel(i,j, c);
        //        //prevPoses.Enqueue(new Vector2Int(i,j));
        //    }
        //}
        for (int i = 0; i < Brush.width; i++)
        {
            for (int j = 0; j < Brush.height; j++)
            {
                int k = (int)center.x  + i;
                int n = (int)center.y + j;
                 Color c2 = canva.GetPixel(k, n);


                //if (c2==Color.clear)
                { Color c = Brush.GetPixel(i, j);

                    Color c3 = c + c2;
                    // if(c.a>0.1f)
                    canva.SetPixel(k, n, c3);
                }
            }
        }

        //if (prevPoses.Count >= countToDelete)
        //{

        //    Vector2Int pos1=prevPoses.Dequeue();
        //   // canva.SetPixel(pos1.x, pos1.y, Color.black);
        //}
        canva.Apply();     
    }
}
