using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public RollingLogic RollingLogic;
    public LayerMask HitMask;
    public Transform CameraRot;
    public Material pointerTraceMaterial;

    public EffectsPlayer EffectsPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
            return;
        //Добавить игнор на обхекты
        bool fire = Input.GetButtonDown("Fire1") ;
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (fire  && !isOverUI)
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);


                    RollingLogic.GetNewBallPlace(r);
            EffectsPlayer.PlayerHit();



        }
    }
}
