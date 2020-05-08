using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public RectTransform RotationParent;
    public RectTransform ArrowPrefab;
    public LevelCreatingEffect LevelCreatingEffect;
  protected  List<Rotator> Rotators = new List<Rotator>();

    Vector2 baseSize;
    public float decreasePercent;
    public float ArrowOnScreenMax = 5;


    int addingDir = 0;
    int addingSpeed = 0;


    public Vector2 childArrowOffset = new Vector2(-1, 1);


    
    public GameObject TrailPainter;
    public LineRenderer ShowTrailPainter;

   
    [SerializeField]
     RectTransform SaveDialog;

    [SerializeField]
    TextMeshProUGUI SaveResultTxt;
    void Start()
    {
        TrailPainter.transform.localEulerAngles = new Vector3(0, 0, 135 );// +Rotators[0].transform.localEulerAngles;
        ShowTrailPainter.transform.localEulerAngles = new Vector3(0, 0, 135);// + Rotators[0].transform.localEulerAngles;

        CalculateArrowSize(Vector2.one);
        AddArrow(1, 1, false, Rotators);
    }


    protected  void CalculateArrowSize(Vector2 gapPercent)
    {


        Vector2 size = RotationParent.rect.size;

        Vector2 gap = new Vector2(gapPercent.x * size.x, gapPercent.y * size.y) / 100;
        size.x -= gap.x * 2;
        size.y -= gap.y * 2;

        size /= ArrowOnScreenMax*2;
        float minSide = (size.x < size.y) ? size.x : size.y;


        baseSize = new Vector2(minSide, minSide);
    }


    public void ShowPanel(GameObject panelToShow)
    {
        panelToShow.SetActive(true);

    }
    public void ClosePanel(GameObject panelToClose)
    {
        panelToClose.SetActive(false);

    }

    public void SetDirection(int direction)
    {
        addingDir = direction;
    }

    public void SetSpeed(int speed)
    {
        addingSpeed = speed;

        AddArrow(addingDir, addingSpeed, false, Rotators);
    }

   
    public void AddArrow(int direction, int speed, bool forSolutio, List<Rotator> arrows)
    {
        
        ShowTrailPainter.positionCount = 0;
        RectTransform r = Instantiate(ArrowPrefab);
        Rotator rr;

             rr = r.gameObject.AddComponent<Rotator>();
        if (arrows.Count == 0)
        {
           

           r.transform.SetParent(RotationParent);
           r.transform.localScale = Vector3.one;
           r.transform.localPosition = Vector2.zero;
            r.sizeDelta = new Vector2(baseSize.x, baseSize.y);
            //Debug.Log(r.sizeDelta +"  b  "+  r.sizeDelta.magnitude);
        }
        else
        {
            RectTransform prevRo = arrows[arrows.Count - 1].GetComponent<RectTransform>();
            r.transform.SetParent(prevRo.transform);
            r.transform.localScale = Vector3.one;
            //r.transform.localRotation = prevRo.localRotation;
            r.transform.localPosition = Vector2.zero+new Vector2( prevRo.sizeDelta.x* childArrowOffset.x, prevRo.sizeDelta.y* childArrowOffset.y);
            r.sizeDelta = prevRo.sizeDelta*decreasePercent;
            Debug.Log(r.sizeDelta + "  1  " + r.sizeDelta.magnitude);

        }
        //if (forSolutio)
        //    r.GetComponent<Image>().enabled = false;
        rr.Number = arrows.Count;
        rr.Direction = direction;
        rr.Speed = speed;
        arrows.Add(rr);
     
        angles.Add(0);
        radiuses.Add(rr.GetComponent<RectTransform>().sizeDelta.magnitude);
        speeds.Add((int)(rr.Speed * rr.Direction));
    
          TrailPainter.GetComponent<LineDrawer>().ClearPositions();
          getAngleSum(arrows);
      
  circleFill = 0;



    }
    Vector3 getAngleSum(List<Rotator> arrows)
    {
        Vector3 res = Vector3.zero;
        startAngles = new float[arrows.Count];

        for (int i = 0; i < arrows.Count; i++)
        {

            startAngles[i] = angles[i];

        }



        return res;
    }


    float circleFill = 0;

    List<float> angles=new List<float>();
    List<float> radiuses = new List<float>();
    List<int> speeds = new List<int>();


    float[] startAngles;
    public void GetPosition()
    {
        if (Rotators.Count == 0 )
            return;

        float     X = 0;
            float       Y = 0;
        int prevSpeed = 0;
        circleFill += Time.deltaTime * Rotator.AllSpeed * Mathf.PI / (180);
        for (int i = 0; i < radiuses.Count; i++)
        {
            prevSpeed += speeds[i];

            angles[i] += (prevSpeed) * Time.deltaTime * Rotator.AllSpeed * Mathf.PI / (180);
    
            X += radiuses[i] * Mathf.Cos(angles[i]);
            Y += radiuses[i] * Mathf.Sin(angles[i]);


        }

        if ( circleFill >= 2 * Mathf.PI)
            return;
        TrailPainter.GetComponent<LineDrawer>().AddPosition(new Vector2(X, Y));
    }


    public void ShowPath()
    {
        if (radiuses.Count != 0 && speeds.Count!=0)
        {
            Vector3[] positions = PathGenerator.GetPositions(speeds.ToArray(), radiuses.ToArray(), startAngles);

            ShowTrailPainter.positionCount = positions.Length;
            ShowTrailPainter.SetPositions(positions);
        }
    }

   void Update()
    {


        GetPosition();

    }
    

    public void Clear()
    {

        for (int i = 0; i < Rotators.Count; i++)
        {
        Rigidbody2D r=    Rotators[i].gameObject.AddComponent<Rigidbody2D>();
            Rotators[i].Clear();
         Rotators[i].enabled = false;
            r.AddTorque(speeds[i]*Random.Range(50, 100));
            Destroy(Rotators[i].gameObject, 2f);
        }
        Rotators.Clear();

        radiuses.Clear();
        angles.Clear();
        speeds.Clear();
        TrailPainter.GetComponent<LineDrawer>().ClearPositions();
        ShowTrailPainter.positionCount = 0;
      //  LevelCreatingEffect.Rotating(false);
        LevelCreatingEffect.ArrowClear();
    }



    public void Save()
    {

        GameDataSaver gds= FindObjectOfType<GameDataSaver>();
        bool success = false;
        if (gds != null && Rotators.Count >= 1)
        {

            success= gds.AddLevel(speeds.ToArray(), startAngles);
        }

        SaveDialog.gameObject.SetActive(true);
        if (success)
        {
            SaveResultTxt.text = "level saved";
        }
        else
        {
            SaveResultTxt.text = "failed to save";
        }
        
    }


}
