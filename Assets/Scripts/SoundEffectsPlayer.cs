using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectsPlayer : MonoBehaviour
{
    public static float SoundLevel = 1;

    public UnityEngine.UI.Image SoundButton;


    public Sprite SoundOn;
    public Sprite SoundOff;


    Queue<AudioSource> playingSounds = new Queue<AudioSource>();


    private void Awake()
    {
        if (PlayerPrefs.HasKey(nameof(SoundLevel)))
            SoundLevel = PlayerPrefs.GetFloat(nameof(SoundLevel));
        switchSoundBtn();
    }


    public void PlaySound(AudioSource sound)
    {
        if (SoundLevel == 1)
        {
            sound.Play();
        }

     }

    public void PlaySoundAndDisable(AudioSource sound)
    {

        StartCoroutine(waitForSound(sound));
    }

    IEnumerator waitForSound(AudioSource sound)
    {

        if (SoundLevel == 1)
        {
            sound.Play();
            float time = sound.clip.length;

            yield return new WaitForSeconds(time * 2);

        }
        sound.gameObject.SetActive(false);
    }

    void switchSoundBtn()
    {


        if (SoundButton != null)
            if (SoundLevel == 1)
            {

                SoundButton.sprite = SoundOn;
            }
            else
            {

                SoundButton.sprite = SoundOff;
            }
    }

    public void SwitchSound()
    {
        if (SoundLevel == 1)
            SoundLevel = 0;
        else
            SoundLevel = 1;

        switchSoundBtn();

        PlayerPrefs.SetFloat(nameof(SoundLevel), SoundLevel);
    }
}
