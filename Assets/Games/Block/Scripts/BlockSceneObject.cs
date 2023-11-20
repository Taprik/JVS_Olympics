using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tool;
using System.Threading.Tasks;
using static Unity.VisualScripting.Member;
using UnityEngine.UI;

public class BlockSceneObject : GameSceneObject
{
    public GameBlock GameBlock => _gameBlock;
    [SerializeField]
    GameBlock _gameBlock;

    #region HomePage

    public GameObject HomePage => _homePage;
    [SerializeField, Header("HomePage")]
    GameObject _homePage;

    async Task HomeAwake()
    {

    }

    async Task HomeStart()
    {

    }

    void HomeUpdate()
    {

    }

    #endregion

    #region GamePage

    public GameObject GamePage => _gamePage;
    [SerializeField, Header("GamePage")]
    GameObject _gamePage;

    async Task GameAwake()
    {

    }

    async Task GameStart()
    {

    }

    void GameUpdate()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SplitImage("C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Photoblock\\medaille.jpg", _spritesRoot, _nbDivision);
        }
    }

    #region SplitImage

    [SerializeField]
    GameObject _partPrefab;

    [SerializeField]
    GameObject _spritesRoot;

    [SerializeField]
    List<Sprite> _splitedSprites;

    [SerializeField, Tooltip("That is the RootSquare of the numbre enter :")]
    int _nbDivision;

    [SerializeField]
    Vector3 _scaleImage;

    public async void SplitImage(string path, GameObject parent, int nbDivision)
    {
        Task<Texture2D> taskTex2D = ToolBox.CreateTextureFromPath(path);
        Texture2D texture = await taskTex2D;

        for (int i = 0; i < nbDivision; i++)
        {
            for (int j = 0; j < nbDivision; j++)
            {
                float h = texture.height / nbDivision;
                float w = texture.width / nbDivision;
                Sprite newSprite = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
                GameObject n = Instantiate(_partPrefab, parent.transform);
                Image sr = n.GetComponent<Image>();
                RectTransform rt = n.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2((parent.transform as RectTransform).rect.width / nbDivision, (parent.transform as RectTransform).rect.height / nbDivision);
                sr.sprite = newSprite;
                sr.transform.localScale = _scaleImage;
                float imageWidth = sr.rectTransform.sizeDelta.x;
                float imageHeight = sr.rectTransform.sizeDelta.y;
                (n.transform as RectTransform).localPosition = new Vector3(
                    i * imageWidth * _scaleImage.x + imageWidth * _scaleImage.x / 2 - (parent.transform as RectTransform).rect.width / 2, 
                    j * imageHeight * _scaleImage.y + imageHeight * _scaleImage.y / 2 - (parent.transform as RectTransform).rect.height / 2,
                    0);
            }
        }
    }

    #endregion

    #endregion

    #region ScorePage

    public GameObject ScorePage => _scorePage;
    [SerializeField, Header("ScorePage")]
    GameObject _scorePage;

    async Task ScoreAwake()
    {

    }

    async Task ScoreStart()
    {

    }

    void ScoreUpdate()
    {

    }

    #endregion

    #region Unity Func

    public override async void Awake()
    {
        base.Awake();
        await HomeAwake();
        await GameAwake();
        await ScoreAwake();
    }

    private async void Start()
    {
        await HomeStart();
        await GameStart();
        await ScoreStart();
    }

    private void Update()
    {
        HomeUpdate();
        GameUpdate();
        ScoreUpdate();
    }

    #endregion
}