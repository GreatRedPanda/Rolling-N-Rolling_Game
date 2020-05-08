using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
public class GameDataSaver : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);


    public DefaultLevelsContainer DefaultLevelsContainer;
    public static List<LevelDataLoaded> levels = new List<LevelDataLoaded>();
    public string LevelCreatingScene;
    public string LevelScene;
    public string MainMenuScene;

    public float startRaduis;
    public float raduisDecreasePercent;

    //public  static GameDataSaver Instance;
    public static LevelData CurrentLevel;
    public string FileName = "levels.dscrp";


    static bool levelsWasLoaded = false;


    string BestScoresDefaultLevels;

    static string lastLevelKey;
    public static LevelData LastLevel;
    private void Awake()
    {
        GamePause(false);
        loadPlayerPrefs();
        LoadLevels();
        if (CurrentLevel == null)
        {

            setPlayingLevel(DefaultLevelsContainer.Levels[0]);
            if (SceneManager.GetActiveScene().name == LevelScene)
            {

                FindObjectOfType<RollingLogic>().InitializeLevel(CurrentLevel, GetRadiuses(CurrentLevel.Speeds.Length, startRaduis, raduisDecreasePercent));
            }
        }


        LevelsLoader lLoader = FindObjectOfType<LevelsLoader>();
        if (lLoader != null)
            lLoader.LoadLevelsUI();

    }

    void loadPlayerPrefs()
    {

        if (PlayerPrefs.HasKey(nameof(lastLevelKey)))
        {

            lastLevelKey = PlayerPrefs.GetString((nameof(lastLevelKey)));
        }

    }

    void savePrefs()
    {
        PlayerPrefs.SetString((nameof(lastLevelKey)), CurrentLevel.Key);
    }
    public List<LevelData> LoadLevels()
    {

        List<LevelData> levelsLoaded = new List<LevelData>();

        if (DefaultLevelsContainer != null && DefaultLevelsContainer.Levels != null)
            levelsLoaded.AddRange(DefaultLevelsContainer.Levels);
        if (levelsWasLoaded)
        {
            levelsLoaded.AddRange(levels);
            return levelsLoaded;
        }



        levelsWasLoaded = true;
        string path = getFilePath();
        if (File.Exists(path))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                levels = (List<LevelDataLoaded>)bf.Deserialize(file);
                file.Close();
            }
            catch
            { }
        }
        levelsLoaded.AddRange(levels);

        LastLevel = levelsLoaded.Find(x => x.Key == lastLevelKey);
        CurrentLevel = LastLevel;
        return levelsLoaded;
    }


    void SaveLevels()
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = getFilePath();

        if (File.Exists(path))
        {
            FileStream file = File.OpenWrite(path);
            bf.Serialize(file, levels);
            file.Close();
        }
        else
        {
            FileStream file = File.Create(path);
            bf.Serialize(file, levels);
            file.Close();
        }

    }



    public bool AddLevel(int[] speeds, float[] startAngles)
    {

        bool success = false;


        string key = "";
        LevelDataLoaded level = new LevelDataLoaded();
        level.Speeds = new int[speeds.Length];
        level.StartAngles = new float[speeds.Length];
        for (int i = 0; i < speeds.Length; i++)
        {
            int speed = speeds[i];
            key += speed.ToString() + startAngles[i].ToString("0.00");
            level.Speeds[i] = speed;
            level.StartAngles[i] = startAngles[i];
        }


        LevelDataLoaded sd = levels.Find(x => x.Key == key);

        if (sd == null)
        {
            level.Key = key;
            levels.Add(level);
            success = true;
            float[] radiuses = GetRadiuses(level.Speeds.Length, startRaduis, raduisDecreasePercent);
            Vector3[] positions = PathGenerator.GetPositions(level, radiuses);
            Texture2D texture2D = PathGenerator.MakeTexture(positions, radiuses);
            string name = "level_" + System.DateTime.Now.ToFileTime();
            string path = getTextureFilePath(name);
            if (path == "")
                return false;
            level.TexturePath = name;
            File.WriteAllBytes(path, (byte[])texture2D.EncodeToPNG());


#if UNITY_WEBGL  && !UNITY_EDITOR
            SyncFiles();
#endif

        }
        return success;


    }

    string getFilePath()
    {

        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        return filePath;

    }

    static string getTextureFilePath(string imgName)
    {

        string filePath = Path.Combine(Application.persistentDataPath, imgName + ".png");
        return filePath;

    }

    public static Texture2D LoadTexture(string imgName)
    {
        var filePath = getTextureFilePath(imgName);

        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            //int size = (int)Mathf.Sqrt(bytes.Length);
            // GraphicsFormat format = new GraphicsFormat();
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, true);

            tex.LoadImage(bytes);
            //     tex.Resize(256, 256);
            tex.Apply();
            return tex;
        }
        return null;
    }
    public void LoadScene(string scene)
    {

        SceneManager.LoadScene(scene);
    }
    public void LoadLevel(LevelData level)
    {
        GamePause(false);

        setPlayingLevel(level);
        SceneManager.LoadScene(LevelScene);

    }

    public void LoadLevelCreation()
    {
        SceneManager.LoadScene(LevelCreatingScene);
    }
    public void LoadMaiMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == LevelScene)
        {

            FindObjectOfType<RollingLogic>().InitializeLevel(CurrentLevel, GetRadiuses(CurrentLevel.Speeds.Length, startRaduis, raduisDecreasePercent));
        }



#if UNITY_WEBGL && !UNITY_EDITOR
        SaveLevels();

        SyncFiles();
#endif
    }


    public static float[] GetRadiuses(int count, float startRadius, float decreasePercent)
    {

        float[] radiuses = new float[count];
        radiuses[0] = startRadius;
        for (int i = 1; i < count; i++)
        {
            radiuses[i] = radiuses[i - 1] * decreasePercent;
        }
        return radiuses;

    }

    public void DeleteLevel(LevelData levelToDelete)
    {
        LevelDataLoaded levelDataLoaded = levelToDelete as LevelDataLoaded;
        if (levelDataLoaded != null)
        {
            string path = getTextureFilePath(levelDataLoaded.TexturePath);

            File.Delete(path);
            levels.Remove(levelDataLoaded);
        }

        setPlayingLevel(DefaultLevelsContainer.Levels[0]);
    }
    private void OnApplicationPause(bool pause)
    {
        GamePause(pause);
        if (pause)
        {
            savePrefs();

            SaveLevels();
        }
    }




    public void GamePause(bool pause)
    {
        if(pause)
        Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    private void OnApplicationQuit()
    {
        SaveLevels();
    }



    public void Lose(int score)
    {
        if (score > CurrentLevel.BestScore)
            CurrentLevel.BestScore = score;
    }

    void setPlayingLevel(LevelData level)
    {
        CurrentLevel = level;
        LastLevel = CurrentLevel;
        lastLevelKey = LastLevel.Key;
        savePrefs();
    }



    private static void PlatformSafeMessage(string message)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WindowAlert(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
