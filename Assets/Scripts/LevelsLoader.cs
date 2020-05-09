using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsLoader : MonoBehaviour
{
    [HideInInspector]
    public List<LevelData> DefaultLevels;

    public int LevelsOnPage = 5;

    public RectTransform PagesParent;
    public RectTransform PagePrefab;

    public LevelButton LevelBtnPrefab;

    public RectTransform DeletelevelDialogPanel;

    public Button DeleteBtn;


    public RectTransform LevelsParent;


    public LevelButton LastLevelImg;

    public TMPro.TextMeshProUGUI chosenLevelStatsTxt;

  public void LoadLevelsUI()
    {
        DefaultLevels = FindObjectOfType<GameDataSaver>().LoadLevels();
        LevelData lastLevel = GameDataSaver.LastLevel;
        Load(PagesParent, PagePrefab, LevelBtnPrefab, DefaultLevels, lastLevel, LevelsOnPage);
        if (lastLevel != null)
            loadLevel(lastLevel, LastLevelImg);
        showStats(lastLevel);
    }

    void getLevelImageSize(RectTransform panel, int levelsOnPage, int colomns = 2)
    {
        int row = levelsOnPage / colomns;
        GridLayoutGroup grid = PagePrefab.GetComponent<GridLayoutGroup>();


        Vector2 spacing = grid.spacing;
        float paddingX = grid.padding.left + grid.padding.right;

        float paddingY = grid.padding.top + grid.padding.bottom;

        float cellSizeX = (panel.rect.width - paddingX - spacing.x * (colomns - 1)) / colomns;



        float cellSizeY = (panel.rect.height - paddingY - spacing.y * (row - 1)) / row;


        float cellSize = (cellSizeX < cellSizeY) ? cellSizeX : cellSizeY;
        grid.cellSize = new Vector2(cellSize, cellSize);
    }

    public void Load(RectTransform pagesParent, RectTransform pagePrefab, LevelButton levelBtnPrefab, List<LevelData> levels, LevelData lastlevel, int levelsOnPage)
    {
        for (int i = 0; i < levels.Count; i++)
        {          
            LevelButton btn = Instantiate(levelBtnPrefab);

            btn.transform.SetParent(LevelsParent);
            btn.transform.localScale = Vector3.one;
            int levelNumber = i;
            loadLevel(levels[levelNumber], btn);
         
        }
    }


    private void loadLevel(LevelData level, LevelButton btn)
    {
        Image image = btn.GetComponentInChildren<Image>();
        Sprite levelSpr = null;

        if (image != null)
        {
            Texture2D texture = null;

            if (level is LevelDataLoaded)
                texture = GameDataSaver.LoadTexture((level as LevelDataLoaded).TexturePath);
            else if (level is LevelDataDefault)
            {
                texture = (level as LevelDataDefault).Texture;
            }


            if (texture != null)
            {
                levelSpr = Sprite.Create(texture,
                new Rect(0, 0, 512, 512),
                  Vector2.zero, 100);
            }
            image.sprite = levelSpr;

        }

        btn.onClick.AddListener(() =>
        {
            if (LevelButton.LastBtnClicked != btn)
            {
                LevelButton.LastBtnClicked = btn;
                LevelButton.ClickCount = 0;

            }
        

                LevelButton.ClickCount ++;
            
           
            if (LevelButton.ClickCount == 1)
            {
                showStats(level);
            }
            else if (LevelButton.ClickCount == 2)
            {
                LevelButton.LastBtnClicked = null;
                LevelButton.ClickCount = 0;
                FindObjectOfType<GameDataSaver>().LoadLevel(level);
            }

        });
        if (level is LevelDataLoaded)
        {
            btn.onPressing.AddListener(() =>
            {
                DeletelevelDialogPanel.position = btn.transform.position;
                DeletelevelDialogPanel.sizeDelta = btn.GetComponent<RectTransform>().sizeDelta;
                DeletelevelDialogPanel.gameObject.SetActive(true);

                DeleteBtn.onClick.RemoveAllListeners();
                DeleteBtn.onClick.AddListener(() =>
                {
                FindObjectOfType<GameDataSaver>().DeleteLevel(level);
                DeletelevelDialogPanel.gameObject.SetActive(false);                            
                Destroy(btn.gameObject);
                loadLevel(GameDataSaver.LastLevel, LastLevelImg);
                });
            });
        }
    }

    void showStats(LevelData level)
    {

        bool isDefault = level is LevelDataDefault;
        string def = (isDefault) ? "Default\n" : "";
        chosenLevelStatsTxt.text =
            $"level data\n"+def+"best score: " + level.BestScore + "\ncircles amount: " + level.Speeds.Length;
    }

    void lastLevelLoad(LevelData level, Sprite sp, System.Action loadMethod)
    {

        Image im = LevelBtnPrefab.GetComponent<Image>();
        im.sprite = sp;


    }
}
