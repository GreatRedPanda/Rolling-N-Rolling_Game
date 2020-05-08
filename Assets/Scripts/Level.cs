using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
  public  class LevelData
  {

    public string Key;
    public int[] Speeds;
    public float[] StartAngles;
    public  virtual int BestScore { get; set; }
    
   


  }



[Serializable]
public class LevelDataLoaded: LevelData
{
    public string TexturePath;

}

[Serializable]
public class LevelDataDefault : LevelData
{
    public Texture2D Texture;

    public string BestScoreSave;

    int bestScore = 0;
    public override int BestScore {

        get
        {

            if (!bestScoreLoaded)
            {
                if (PlayerPrefs.HasKey(Key+nameof(BestScoreSave)))
                {
                    bestScore = PlayerPrefs.GetInt(Key + nameof(BestScoreSave));
                    bestScoreLoaded = true;
                }
            }
            return bestScore;

        }
        set {

            bestScore = value;
            PlayerPrefs.SetInt(Key + nameof(BestScoreSave), bestScore);
        }

    }

    bool bestScoreLoaded = false;

}



