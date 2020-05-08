using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuEffects : MonoBehaviour
{
    [HideInInspector]
    public float[] RandomRadius;
    public int[] RandomSpeed;
    public float BaseSize;
    public float DecreasePercent;
    // Start is called before the first frame update
    void Start()
    {
       // CalculateArrowSize(Vector2.one);
        GenerateRandom();
    }


    void GenerateRandom()
    {

        //int count = Random.Range(RandomRadius.x, RandomRadius.y + 1);

        //for (int i = 0; i < count; i++)
        //{

        //    int direction = Random.Range(-1, 1);
        //    int speed= Random.Range(RandomSpeed.x, RandomSpeed.y);
        //    AddArrow(direction, speed, false, Rotators);
        //}

        RandomRadius= GameDataSaver.GetRadiuses(RandomSpeed.Length, BaseSize, DecreasePercent);
     Vector3[] positions=   PathGenerator.GetPositions(RandomSpeed, RandomRadius, new float[RandomSpeed.Length] );

      Texture2D texture2D=  PathGenerator.MakeTexture(positions, RandomRadius);

        string name = "level_" + System.DateTime.Now.ToFileTime();
        string path = Path.Combine(Application.streamingAssetsPath, name + ".png");
        if (path != "")       
        File.WriteAllBytes(path, (byte[])texture2D.EncodeToPNG());
    }


    //void Update()
    //{


    //    GetPosition();

    //}

}
