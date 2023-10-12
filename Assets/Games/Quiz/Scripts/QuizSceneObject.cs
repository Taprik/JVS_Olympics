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

    public QuizTeam[] Teams => _teams;
    [SerializeField]
    QuizTeam[] _teams;

    public Animator FadeAnimator => _fadeAnimator;
    [SerializeField]
    Animator _fadeAnimator;

    Quiz_Question _currentQuestion;

    public bool ATeamScore { get; set; }


    public Category debugCategory;

    #endregion

    #region ScorePage

    public GameObject ScorePage => _scorePage;
    [SerializeField, Header("ScorePage")]
    GameObject _scorePage;


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
        }

        SetCategoryButton();

        await GameManager.Instance.AddressablesManager.LoadScreen(Read());
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
            //call End Selected Question 
            return;
        }

        float reflectionTimer = SetQuestion(selectedQuestion[selectedQuestionID]);

        await Task.Delay(Mathf.RoundToInt((reflectionTimer - 1f) * 1000));
        FadeAnimator.SetTrigger("FadeIn");
        await Task.Delay(Mathf.RoundToInt(1000));


        SetTeamsButton();

        await Task.Delay(50);

        Task[] waitTimerOrRightAnswer = new Task[2];

        waitTimerOrRightAnswer[0] = Task.Run(async () =>
        {
            await Task.Delay(Mathf.RoundToInt(_currentQuestion.answersTime * 1000));
        });

        waitTimerOrRightAnswer[1] = Task.Run(async () => 
        {
            while (!ATeamScore)
                await Task.Delay(50);
        });

        await Task.WhenAny(waitTimerOrRightAnswer);
        ATeamScore = false;

        HideTeamsButton();
        FadeAnimator.SetTrigger("FadeOut");

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
            AnswersObject[i].GetComponentInChildren<TextMeshProUGUI>().text = question.answers[i];
        }
        for (int i = 0; i < Teams.Length; i++)
        {
            Teams[i].TeamAnswersHolder.SetActive(false);
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
        selectedQuestionID = 0;
    }

    public void SetSelectedQuestion(string categoryText)
    {
        Category cat = CategoryFromString(categoryText);
        selectedQuestion = GameQuiz.questions.FindAll(x => x.category == cat);
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
        = "C:\\Users\\psuchet\\Documents\\JVS_Olympics\\Personnalisation\\Quizz";
#else
         = "C:\\Users\\psuchet\\Documents\\JVS_Olympics\\Personnalisation\\Quizz";
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
        //Debug.Log(csv.Contains("�"));
        csv = csv.Replace("\u0092", "'");
        int collum = csv.Split(new string[] { "\n" }, StringSplitOptions.None).Length;
        int line = csv.Split(new string[] { "," }, StringSplitOptions.None).Length;
        int lineLength = (line + collum) / collum;
        Debug.Log(lineLength);
        string[] data = csv.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        //for(int i =0; i < data.Length;i++)
        //{
        //    data[i] = data[i].Replace("'", "&rsquo;");
        //}
        //
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
                Debug.Log(id);
                return Category.Sport;
        }
    }

    #endregion

    public void AddScoreToTeam(int id)
    {
        int teamID = 0;
        if (id > 3)
        {
            id -= 4;
            teamID = 1;
        }

        if (id != _currentQuestion.correctAnswer)
            return;

        ATeamScore = true;
        Teams[teamID].TeamScore += GetScore();
    }

    int GetScore()
    {
        int score = 1000;

        if(score < 100) score = 100;

        return score;
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

    public void DestroyTeamButton(int id)
    {
        if (id > 3)
        {
            id -= 4;
            Teams[1].TeamAnswers[id].GetComponent<Animator>().SetTrigger("IsDestroy");
            return;
        }

        Teams[0].TeamAnswers[id].GetComponent<Animator>().SetTrigger("IsDestroy");

    }
}

[System.Serializable]
public struct QuizTeam
{
    public string Name;
    public int TeamScore;

    public GameObject TeamAnswersHolder;
    public GameObject[] TeamAnswers;

}
