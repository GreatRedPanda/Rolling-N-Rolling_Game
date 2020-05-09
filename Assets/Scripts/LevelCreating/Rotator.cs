using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public  static float AllSpeed = 10;
    public float Direction;

    public float Speed;


    public int Number;
     AudioSource rotatingSound;

    void Start()
    {
        rotatingSound = GetComponent<AudioSource>();

        if(rotatingSound!=null)
        StartCoroutine(playSound());
    }


    protected virtual  void Update()
    {
        float angle = Time.deltaTime * Direction * Speed  * AllSpeed;
        transform.Rotate(0, 0,angle );
    }

    
    public void Clear()
    {

        StopAllCoroutines();
    }


    IEnumerator playSound()
    {
        while (true)
        {

            if (SoundEffectsPlayer.SoundLevel != 0)
            {

                float mulCoef = 1;
                if (Direction == -1)
                    mulCoef = 0.5f;

                rotatingSound.volume = 1 - (Number ) * 0.09f;
                rotatingSound.pitch = mulCoef;
                rotatingSound.Play();
            }
            yield return new WaitForSeconds(1 / Speed * AllSpeed);

        }


    }
}

