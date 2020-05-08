using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPlayer : MonoBehaviour
{
    public Pooler Pooler;
    public string[] PoolsNames;

    public Renderer LevelBounds;
    public Color NormalColor;
    public Color DangerColor;
    public Color LostColor;
    public Color DangerColor2;
    Color currentColor;
    public Renderer Portal;
    public List<Color> possibleCOlors = new List<Color>();
    int currentCOlor = 0;
    public float Speed;
    float dt;
    float dt_DangerColorLerp;

    float lineCompletePercent;

    public AudioSource RollSound;

    public string DestroySound;
    public string ShootSound;
    public string DefeatSound;
    public string CollisionSound;


    public SoundEffectsPlayer SoundEffectsPlayer;
    void Start()
    {
        currentColor = DangerColor;
    }

    void ChangeColor()
    {
        if (dt >= 1)
        {
            dt = 0;

            currentCOlor++;
            if(currentCOlor==possibleCOlors.Count)
                currentCOlor = 0;
        }
        int next = currentCOlor + 1;
        if (next == possibleCOlors.Count)
            next = 0;
        Color cur = possibleCOlors[currentCOlor];
        Color nextCol = possibleCOlors[next];
        Color newColor = Color.Lerp(cur, nextCol, dt);
        dt += Time.deltaTime* Speed;

        Portal.material.SetColor("_Color", newColor);
    }

    public void SetLost(float completePercent)
    {
        lineCompletePercent = completePercent;
    }


    void playLostEffect()
    {
        if (lineCompletePercent > 0.95f)
        {
            LevelBounds.material.SetColor("_GridColour", LostColor);
            playAudioEffect(DefeatSound);
        }
        else if (lineCompletePercent > 0.8f)
        {
            Color newColor;//= Color.Lerp(cur, nextCol, dt);
      
            if (dt_DangerColorLerp >= 1)
            {
                dt_DangerColorLerp = 0;
                if (currentColor == DangerColor)
                    currentColor = DangerColor2;
                else
                    currentColor = DangerColor;


            }
            if (currentColor==DangerColor)
                newColor = Color.Lerp(currentColor, DangerColor2, dt_DangerColorLerp);
            else
                newColor = Color.Lerp(currentColor, DangerColor, dt_DangerColorLerp);
            LevelBounds.material.SetColor("_GridColour", newColor);
            dt_DangerColorLerp += Time.deltaTime * Speed;
        }
        else
        {
            LevelBounds.material.SetColor("_GridColour", NormalColor);
        }
     
    }


    public void  playRollSound(bool play, float currentSpeed)
    {
        if (play)
            StartCoroutine(rollSoundPlay(currentSpeed));
        else
            StopAllCoroutines();


    }

    IEnumerator  rollSoundPlay(float speed)
    {

        while (true)
        {

            if (SoundEffectsPlayer.SoundLevel != 0)
            {

                RollSound.Play();
            }
            yield return new WaitForSeconds(1/ speed * 10);

        }

    }
    private void FixedUpdate()
    {
        ChangeColor();
     
            playLostEffect();
    }

    public void PlayEffect(Vector3 position)
    {

        GameObject go = Pooler.GetPooledObject(PoolsNames[0]);

        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        go.SetActive(true);
        if (ps != null)
        {

            ps.transform.position = position;
            ps.Play();
        }
    }

    public void PlayEffect(List<Ball> destroyedBalls)
    {
        for (int i = 0; i < destroyedBalls.Count; i++)
        {


            GameObject go = Pooler.GetPooledObject(PoolsNames[0]);

            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = destroyedBalls[i].GetComponent<Renderer>().material.GetColor("_Color");

            go.SetActive(true);
            if (ps != null)
            {

                ps.transform.position = destroyedBalls[i].transform.position;
                ps.Play();
            }
        }

        playAudioEffect(DestroySound);
    }

    public void SpheresCollided()
    {
        playAudioEffect(CollisionSound);

    }
    public void PlayerHit()
    {
        playAudioEffect(ShootSound);


    }
    void  playAudioEffect(string eventType)
    {
        GameObject go = Pooler.GetPooledObject(eventType);

        if (go != null)
        {
         

            AudioSource audioSource= go.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                go.SetActive(true);
                SoundEffectsPlayer.PlaySoundAndDisable(audioSource);
            }
        }
    }



}
