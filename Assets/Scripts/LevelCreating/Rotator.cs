using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

  public  static float AllSpeed = 10;
    public float Direction;

    public float Speed;


    public int Number;
     AudioSource RotatingSound;
    void Start()
    {
        RotatingSound = GetComponent<AudioSource>();

        if(RotatingSound!=null)
        StartCoroutine(playSound());
    }

    // Update is called once per frame
  protected virtual  void Update()
    {//* Mathf.PI
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

                RotatingSound.volume = 1 - (Number ) * 0.09f;
                RotatingSound.pitch = mulCoef;
                RotatingSound.Play();
            }
            yield return new WaitForSeconds(1 / Speed * AllSpeed);

        }


    }
}

