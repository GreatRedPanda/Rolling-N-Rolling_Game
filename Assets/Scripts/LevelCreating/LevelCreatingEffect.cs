using UnityEngine;
using System.Collections;

public class LevelCreatingEffect : MonoBehaviour
{

    public AudioSource RotatingSound;

    public AudioSource Clear;




    public SoundEffectsPlayer SoundEffectsPlayer;


    public void ArrowClear()
    {
        SoundEffectsPlayer.PlaySound(Clear);
    }

    public void ArrowAdd()
    {



    }


    private void Update()
    {

    }
    public void Rotating(bool rotating)
    {

        //if (rotating)
        //{
        //    RotatingSound.loop = true;
        //    if (!RotatingSound.isPlaying)
        //        SoundEffectsPlayer.PlaySound(RotatingSound);

        //}
        //else
        //{
        //    RotatingSound.loop = false;
        //}
    }
}
