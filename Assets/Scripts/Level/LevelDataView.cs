using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
public class LevelDataView: MonoBehaviour
{

    public Image CurrentBallImage;
    public Image NextBallImage;
    public Image NextNextBallImage;
    public RectTransform LostDialog;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI BestScoreText;

    Dictionary<string, Color> Colors = new Dictionary<string, Color>();

    private void Awake()
    {
        
    }

    void fillDictionary()
    {
        Pooler pooler = FindObjectOfType<Pooler>();

        for (int i = 0; i < pooler.pools.Count; i++)
        {

            if (pooler.pools[i].PooledPrefab.GetComponent<Ball>() != null)
            {

                Renderer r = pooler.pools[i].PooledPrefab.GetComponent<Renderer>();
            
                Colors.Add(pooler.pools[i].PoolName, r.sharedMaterial.GetColor("_Color"));
            }

        }

    }
    public void SetData(Queue<string> ballsPools)
    {
        if (Colors.Count == 0)
            fillDictionary();
        int i = 0;
        foreach (var b in ballsPools)
        {
            if (i == 0)
            {

              //  setImageColor(CurrentBallImage, b);
                CurrentBallImage.color = Colors[b];
            }
            else if (i == 1)
            {
               // setImageColor(NextBallImage, b);
               NextBallImage.color = Colors[b];

            }
            else if (i == 2)
            {


                NextNextBallImage.color = Colors[b] ;

               // setImageColor(NextNextBallImage, b);
            }


            i++;

        }


    }



    void setImageColor(Image im, string pool)
    {
        if (pool == "Gray")
            im.color = Color.gray;
        else if(pool == "Yellow")
            im.color = Color.yellow;
        else if (pool == "Red")
            im.color = Color.red;
        else if (pool == "Blue")
            im.color = Color.blue;

    }




    public void ShowLostDialog()
    {


        LostDialog.gameObject.SetActive(true);

   }


    public void SetScore(int score)
    {
        ScoreText.text = "+" + score;


    }

    public void SetBestScore(int score, int bestScore)
    {

        ScoreText.text = "+" + score;
        BestScoreText.text = "+" + bestScore;
    }
}

