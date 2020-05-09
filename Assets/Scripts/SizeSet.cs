using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
public class SizeSet : MonoBehaviour
{

    public RectTransform ButtonsMenu;
 
    ParticleSystem particle;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        SetSize();
    }


    void SetSize()
    {

        //Debug.Log(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //Debug.Log(new Vector3(Screen.width / 4, Screen.height / 4, 0));
        //Debug.Log(new Vector3(ButtonsMenu.offsetMax.x, ButtonsMenu.offsetMax.y, 0));

        ParticleSystem.ShapeModule shape= particle.shape;
        Vector3 center = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 side = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 4, Screen.height / 4, 0));
        Vector3 sideOfMenu = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 2+ButtonsMenu.sizeDelta.x/2, Screen.height / 2+ ButtonsMenu.sizeDelta.y/2, 0));
        float dist = Vector3.Distance(center, side);
        float dist1 = Vector3.Distance(side, sideOfMenu);
        //Debug.Log(sideOfMenu);
        shape.radius = dist;
        shape.donutRadius = 0.9f-dist1;

    }
}
