using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class UIElementEventData
{


    public Button Button;
    public string Type;
    public string Method;


}
public class UIEventsSetter : MonoBehaviour
{

    public Button LevelBackBtn;
    public string PrevScene;
   // public UIElementEventData[] uIElementEvents;

    void Start()
    {

        //for (int i = 0; i < uIElementEvents.Length; i++)
        //{

        //    string type = "";
        //    System.Type t = System.Type.GetType(type);

        //    Object o = FindObjectOfType(t);

        //    if (o != null)
        //    {


        //    }

        //}


        LevelBackBtn.onClick.AddListener(()=> { FindObjectOfType<GameDataSaver>().LoadScene(PrevScene); });

    }


}
