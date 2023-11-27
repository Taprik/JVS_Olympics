using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Linq;
using Tool;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class QuizSceneObject : GameSceneObject
{
    public GameQuiz GameQuiz => _gameQuiz;
    [SerializeField]
    GameQuiz _gameQuiz;

    #region HomePage

    public GameObject HomePage => _homePage;
    [SerializeField, Header("HomePage")]
    GameObject _homePage;

    public GameObject PlayButtonHolder => _playButtonHolder;
    [SerializeField]
    GameObject _playButtonHolder;

    public GameObject SelectCategoryHolder => _selectCategoryHolder;
    [SerializeField]
    GameObject _selectCategoryHolder;

    public GameObject[] CategoryButtons => _categoryButtons;
    [SerializeField]
    GameObject[] _categoryButtons;


    #endregion

    #region GamePage

    public GameObject GamePage => _gamePage;
    [SerializeField, Header("GamePage")]
    GameObject _gamePage;

    public Image QuestionImage => _questionImage;
    [SerializeField]
    Image _questionImage;

    public TextMeshProUGUI QuestionText => _questionText;
    [SerializeField]
    TextMeshProUGUI _questionText;

    int selectedQuestionID = 0;
    List<Quiz_Question> selectedQuestion;

    public GameObject AnswersHolder => _answersHolder;
    [SerializeField]
    GameObject _answersHolder;

    public GameObject[] AnswersObject => _answersObject;
    [SerializeField]
    GameObject[] _answersObject;

    public TextMeshProUGUI[] AnswersTexts => _answersTexts;
    [SerializeField]
    TextMeshProUGUI[] _answersTexts;

    public QuizTeam[] Teams => _teams;
    [SerializeField]
    QuizTeam[] _teams;

    public Animator FadeAnimator => _fadeAnimator;
    [SerializeField]
    Animator _fadeAnimator;

    [SerializeField]
    AnimationClip _etoileAnim;

    [SerializeField]
    GameObject _questionObject;

    [SerializeField]
    GameObject _resultObject;

    [SerializeField]
    Image _rightAnswerImage;

    [SerializeField]
    TextMeshProUGUI _rightAnswerText;

    [SerializeField]
    TextMeshProUGUI _scoreGainText;

    [SerializeField]
    SpritesOverTime _timerAnim;

    [SerializeField]
    RawImage _videoAnimImage;

    [SerializeField]
    VideoPlayer _videoAnimPlayer;

    [SerializeField]
    GameObject _answerResultObject;

    [SerializeField]
    Image _letterResultImage;

    [SerializeField]
    TextMeshProUGUI _answerResultText;

    Quiz_Question _currentQuestion;

    bool ATeamScore { get; set; }

    int WinningTeam { get; set; }
    int WinningScore { get; set; }

    const string AnimTaskListName = "QuizAnimTaskList";

    float _timer = 0;

    public Category debugCategory;

    #endregion

    #region ScorePage

    public GameObject ScorePage => _scorePage;
    [SerializeField, Header("ScorePage")]
    GameObject _scorePage;

    public TextMeshProUGUI ScoreText => _scoreText;
    [SerializeField]
    TextMeshProUGUI _scoreText;

    public TextMeshProUGUI ScoreTeam => _scoreTeam;
    [SerializeField]
    TextMeshProUGUI _scoreTeam;

    #endregion



    public override async void Awake()
    {
        base.Awake();
        HomePage.SetActive(true);
        GamePage.SetActive(false);
        ScorePage.SetActive(false);

        GameManager.Instance.CurrentGame = GameQuiz;

        for (int i = 0; i < Teams.Length; i++)
        {
            Teams[i].TeamAnswersHolder.SetActive(false);
            Teams[i].TeamScoreHolder.SetActive(false);
            Teams[i].TeamName.text = Teams[i].Name;
            Teams[i].TeamScore.text = Teams[i].Score.ToString() + " pts";
            Teams[i].SetColor();
        }

        SetCategoryButton();

        await GameManager.Instance.AddressablesManager.LoadScreen(Read());

        GameManager.Instance.TasksManager.CreateTaskList(AnimTaskListName);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            SetSelectedQuestion(debugCategory);

        if(Input.GetKeyDown(KeyCode.N))
            SetQuestion(selectedQuestion[selectedQuestionID]);

        if (Input.GetKeyDown(KeyCode.P))
            PlayQuestion();
    }

    public async void PlayQuestion()
    {
        ATeamScore = false;
        if (selectedQuestionID < 0)
        {
            PlayScore();
            return;
        }

        _resultObject.SetActive(false);
        _questionObject.SetActive(true);
        float reflectionTimer = SetQuestion(selectedQuestion[selectedQuestionID]);
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        UnityMainThreadDispatcher.Instance().Enqueue(async () => await _timerAnim.Anim(reflectionTimer - 1f, tokenSource.Token));
        await Task.Delay(Mathf.RoundToInt((reflectionTimer - 1f) * 1000));
        FadeAnimator.SetTrigger("FadeIn");
        await Task.Delay(50);
        await Task.Delay(Mathf.RoundToInt(FadeAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration * 1100));

        SetTeamsButton();

        await Task.Delay(50);
        UnityMainThreadDispatcher.Instance().Enqueue(async () => await _timerAnim.Anim(_currentQuestion.answersTime, tokenSource.Token));

        _timer = Time.time;

        Task[] waitTimerOrRightAnswer = new Task[2];

        waitTimerOrRightAnswer[0] = Task.Run(async () =>
        {
            await Task.Delay(Mathf.RoundToInt(_currentQuestion.answersTime * 1000));
            tokenSource.Cancel();
        });

        waitTimerOrRightAnswer[1] = Task.Run(async () => 
        {
            while (!ATeamScore)
                await Task.Delay(10);
            tokenSource.Cancel();
        });

        await Task.WhenAny(waitTimerOrRightAnswer);
        tokenSource.Dispose();

        await Task.Run(async () =>
        {
            while(!GameManager.Instance.TasksManager.AllTasksFinish(AnimTaskListName))
                await Task.Delay(100);
        });

        SetTeamsScoreHolder(WinningTeam);
        HideTeamsButton();
        ATeamScore = false;

        await Task.Run(async () =>
        {
            while (!GameManager.Instance.TasksManager.AllTasksFinish())
                await Task.Delay(10);
        });

        FadeAnimator.SetTrigger("FadeOut");
        await Task.Delay(50);
        await Task.Delay(Mathf.RoundToInt(FadeAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration * 1100));
        PlayQuestion();
    }

    public async void PlayScore()
    {
        GamePage.SetActive(false);
        ScorePage.SetActive(true);
        if (Teams[0].Score == Teams[1].Score)
        {
            ScoreTeam.text = "Egalité !";
            ScoreText.text = Teams[0].Score.ToString() + " pts";
            ScoreTeam.faceColor = Color.red;
            ScoreText.faceColor = Color.red;
        }
        else
        {
            int id = (Teams[0].Score > Teams[1].Score) ? 0 : 1;
            ScoreTeam.text = Teams[id].Name.ToString() + " Gagne !";
            ScoreText.text = Teams[id].Score.ToString() + " pts";
            ScoreTeam.faceColor = Teams[id].Color;
            ScoreText.faceColor = Teams[id].Color;
        }
    }

    public float SetQuestion(Quiz_Question question)
    {
        if (selectedQuestionID < 0)
            return selectedQuestionID;

        _currentQuestion = question;
        QuestionText.text = question.sentence;
        QuestionImage.sprite = question.image;
        for (int i = 0; i < question.answers.Length; i++)
        {
            AnswersTexts[i].text = question.answers[i];
        }
        for (int i = 0; i < Teams.Length; i++)
        {
            Teams[i].TeamAnswersHolder.SetActive(false);
            Teams[i].TeamScoreHolder.SetActive(false);
        }

        selectedQuestionID = NextQuestionID(selectedQuestionID);
        return question.reflectionTime;
    }

    int NextQuestionID(int id)
    {
        int newID = id + 1;

        if (newID >= selectedQuestion.Count)
            newID = -1;

        return newID;
    }


    public void SetSelectedQuestion(Category category)
    {
        selectedQuestion = GameQuiz.questions.FindAll(x => x.category == category);
        selectedQuestion.Shuffle();
        selectedQuestionID = 0;
    }

    public void SetSelectedQuestion(string categoryText)
    {
        Category cat = CategoryFromString(categoryText);
        selectedQuestion = GameQuiz.questions.FindAll(x => x.category == cat);
        selectedQuestion.Shuffle();
        selectedQuestionID = 0;
        PlayQuestion();
    }

    public void SetCategoryButton()
    {
        for (int i = 0; i < CategoryButtons.Length; i++)
        {
            CategoryButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = CategoryFromInt(i).ToString();
        }
    }

    #region Read Question

    private bool _ready = false;
    private string _fileLocation
#if UNITY_EDITOR
        = "C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Quizz";
#else
         = "C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Quizz";
#endif 


    public bool IsReady() => _ready;
    //public Question GetRandomQuestion()
    //{
    //    Question res;
    //    if (_currentQuestions.Count == 0)
    //    {

    //        _currentQuestions = new List<Question>(_questions);
    //    }
    //    int index = Random.Range(0, _currentQuestions.Count);
    //    Question question = _currentQuestions[index];
    //    _currentQuestions.RemoveAt(index);
    //    return question;
    //}
    //public Question GetRandomQuestionDifferent(Question current)
    //{

    //    return GetRandomQuestion();
    //}
    private async Task Read()
    {
        if (IsReady())
            return;


        Debug.Log("Read !");
        _ready = false;
        GameQuiz.questions.Clear();
        string appPath = Application.dataPath;
        string newPath = Path.GetFullPath(Path.Combine(appPath, @"..\..\..\..\"));
        newPath = Path.GetFullPath(Path.Combine(newPath, _fileLocation));

        string csv = File.ReadAllText(_fileLocation + "\\Questions.csv", Encoding.GetEncoding("ISO-8859-1"));
        csv = csv.Replace("\u0092", "'");
        int collum = csv.Split(new string[] { "\n" }, StringSplitOptions.None).Length;
        int line = csv.Split(new string[] { "," }, StringSplitOptions.None).Length;
        int lineLength = (line + collum) / collum;
        string[] data = csv.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = (data.Length / lineLength) - 1;




        for (int i = 1; i <= tableSize; i++)
        {
            Quiz_Question currentQuestion = new Quiz_Question();

            currentQuestion.answers = new string[4];
            currentQuestion.sentence = data[lineLength * i];
            for (int j = 0; j < 4; j++)
            {
                currentQuestion.answers[j] = data[lineLength * i + 2 + j];
            }
            data[lineLength * i + 1] = data[lineLength * i + 1].Replace("\"", String.Empty);
            currentQuestion.correctAnswer = ValueFromString(data[lineLength * i + 6]);

            Task<Sprite> sprite = ToolBox.CreateSpriteFromPath(Path.Combine(newPath, data[lineLength * i + 1]));
            await sprite;
            currentQuestion.image = sprite.Result;

            currentQuestion.category = CategoryFromString(data[lineLength * i + 7]);
            Int32.TryParse(data[lineLength * i + 8], out int reflectionTimer);
            currentQuestion.reflectionTime = reflectionTimer;
            Int32.TryParse(data[lineLength * i + 9], out int answersTimer);
            currentQuestion.answersTime = answersTimer;
            GameQuiz.questions.Add(currentQuestion);
        }
        _ready = true;
        //_currentQuestions = new List<Question>(_questions);
        Debug.Log("Finish Reading");
    }

    private int ValueFromString(string text)
    {
        text = text.ToLower();
        text = text.Trim();
        if (text[0] == 'a')
            return 0;
        if (text[0] == 'b')
            return 1;
        if (text[0] == 'c')
            return 2;
        if (text[0] == 'd')
            return 3;
        Debug.LogError("Answer not found " + text[0] + " " + (text[0] == 'a'));
        return 8000;
    }

    private string StringFromValue(int value)
    {
        switch (value)
        {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            default:
                return string.Empty;
        }
    }

    Category CategoryFromString(string text)
    {
        text = text.Replace("\r", String.Empty);
        text = text.Replace("\n", String.Empty);
        switch (text)
        {
            case "GeneralCulture":
                return Category.GeneralCulture;

            case "Sport":
                return Category.Sport;

            case "People":
                return Category.People;

            default:
                Debug.Log(text);
                return Category.Sport;
        }
    }

    Category CategoryFromInt(int id)
    {
        switch (id)
        {
            case 0:
                return Category.GeneralCulture;

            case 1:
                return Category.Sport;

            case 2:
                return Category.People;

            default:
                Debug.LogError(id);
                return Category.Sport;
        }
    }

    #endregion

    public async void AddScoreToTeam(int id)
    {
        if (ATeamScore) return;
        
        int score = 0;
        int teamID = 0;
        if (id > 3)
        {
            id -= 4;
            teamID = 1;
        }

        if (id == _currentQuestion.correctAnswer)
        {
            HideOtherTeamsButton(Teams[teamID].TeamAnswers[id]);
            ATeamScore = true;
            WinningTeam = teamID;
            score = GetScore();
        }

        await GameManager.Instance.TasksManager.AddTaskToList(AnimTaskListName, DestroyTeamButton(id, teamID));

        if (id == _currentQuestion.correctAnswer)
        {
            await GameManager.Instance.TasksManager.AddTaskToList(PlayVideoAnim(teamID));
            await GameManager.Instance.TasksManager.AddTaskToList(Teams[teamID].ScoreAnim(score, 1.5f));
            await GameManager.Instance.TasksManager.AddTaskToList(Task.Delay(1000));
        }
    }

    async Task PlayVideoAnim(int teamID)
    {
        VideoClip[] videoClips = GameQuiz.GetVideoClipOfTeam(teamID);
        if(videoClips == null)
        {
            Debug.LogError("VideoClip[] is null");
            return;
        }

        _videoAnimPlayer.clip = videoClips[Random.Range(0, videoClips.Length)];
        _answerResultText.text = _currentQuestion.answers[_currentQuestion.correctAnswer];
        _letterResultImage.sprite = GameQuiz.GetAnswerLetterSprite(_currentQuestion.correctAnswer);
        _videoAnimPlayer.gameObject.SetActive(true);
        _answerResultObject.SetActive(true);
        await Task.Delay(50);
        _videoAnimImage.gameObject.SetActive(true);
        await Task.Delay(Mathf.RoundToInt((float)_videoAnimPlayer.clip.length * 1000));
        _resultObject.SetActive(true);
        _videoAnimPlayer.gameObject.SetActive(false);
        _videoAnimImage.gameObject.SetActive(false);
        _answerResultObject.SetActive(false);
    }

    int GetScore()
    {
        int score = 1000;

        float time = 1 - ((Time.time - _timer) / _currentQuestion.answersTime);
        score = Mathf.RoundToInt(score * time);

        WinningScore = score < 100 ? 100 : score;
        return WinningScore;
    }

    public void SetTeamsButton()
    {
        foreach (var t in Teams)
        {
            t.TeamAnswersHolder.SetActive(true);
            for (int i = 0; i < t.TeamAnswers.Length; i++)
            {
                t.TeamAnswers[i].SetActive(true);
            }
        }
    }

    public void SetTeamsScoreHolder(int teamID)
    {
        foreach (var t in Teams)
        {
            t.TeamScoreHolder.SetActive(true);
        }

        _rightAnswerImage.sprite = GameQuiz.GetAnswerLetterSprite(_currentQuestion.correctAnswer);
        _rightAnswerText.text = _currentQuestion.answers[_currentQuestion.correctAnswer];
        QuizTeam team = Teams[teamID];
        _scoreGainText.text = team.Name + " +" + WinningScore.ToString() + "pts";
        _scoreGainText.faceColor = team.Color;
    }

    public void HideTeamsButton()
    {
        foreach (var t in Teams)
        {
            t.TeamAnswersHolder.SetActive(false);
            for (int i = 0; i < t.TeamAnswers.Length; i++)
            {
                t.TeamAnswers[i].SetActive(false);
            }
        }
    }

    public void HideTeamsScoreHolder()
    {
        foreach (var t in Teams)
        {
            t.TeamScoreHolder.SetActive(false);
        }
    }

    public void HideOtherTeamsButton(GameObject button)
    {
        foreach (var t in Teams)
        {
            if(!t.TeamAnswers.ToList().Exists(x => x == button))
            {
                t.TeamAnswersHolder.SetActive(false);
                t.TeamScoreHolder.SetActive(false);
            }

            for (int i = 0; i < t.TeamAnswers.Length; i++)
            {
                if (t.TeamAnswers[i] != button)
                    t.TeamAnswers[i].SetActive(false);
            }
        }
    }

    public async Task DestroyTeamButton(int id, int teamID)
    {
        Teams[teamID].TeamAnswers[id].GetComponent<Animator>().SetTrigger("IsDestroy");
        await Task.Delay(Mathf.RoundToInt(_etoileAnim.length * 1000) + 200);
    }


    [System.Serializable]
    public class QuizTeam
    {
        public string Name;
        public int Score;
        public Color Color;

        public GameObject TeamAnswersHolder;
        public GameObject[] TeamAnswers;
        public GameObject[] TeamColorObjects;

        [Space(5)]
        public GameObject TeamScoreHolder;
        public TextMeshProUGUI TeamName;
        public TextMeshProUGUI TeamScore;

        public async Task ScoreAnim(int score, float duration)
        {
            duration /= score;
            for (int i = 0; i < score; i++)
            {
                TeamScore.text = (Score + i).ToString() + " pts";
                await Task.Delay((int)(duration * 1000));
            }
            this.Score += score;
            TeamScore.text = Score.ToString() + " pts";
        }

        public void SetColor()
        {
            foreach (var o in TeamColorObjects)
                SetColor(o, Color);
        }

        public void SetColor(GameObject go, Color color)
        {

            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
            if (text != null)
                text.faceColor = color;

            Image image = go.GetComponent<Image>();
            if (image != null)
                image.color = color;

            SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.color = color;
        }
    }
}


