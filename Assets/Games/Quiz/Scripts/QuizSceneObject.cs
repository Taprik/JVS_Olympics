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

    List<Quiz_Question> selectedQuestion;

    public GameObject AnswersHolder => _answersHolder;
    [SerializeField]
    GameObject _answersHolder;

    List<GameObject> instantiateAnswers;

    public GameObject[] AnswersObject => _answersObject;
    [SerializeField]
    GameObject[] _answersObject;

    #endregion

    #region ScorePage

    public GameObject ScorePage => _scorePage;
    [SerializeField, Header("ScorePage")]
    GameObject _scorePage;


    #endregion



    public override void Awake()
    {
        base.Awake();

    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            SetSelectedQuestion(Category.GeneralCulture);
        }

        if(Input.GetKeyUp(KeyCode.N))
        {
            SetQuestion(selectedQuestion[0]);
        }
    }


    public void SetQuestion(Quiz_Question question)
    {
        QuestionText.text = question.sentence;
        QuestionImage.sprite = question.image;
        instantiateAnswers = new();
        for (int i = 0; i < question.answers.Length; i++)
        {
            instantiateAnswers.Add(Instantiate(AnswersObject[i], AnswersHolder.transform));
            instantiateAnswers[i].GetComponentInChildren<TextMeshProUGUI>().text = question.answers[i];
        }
    }

    public async void SetSelectedQuestion(Category category)
    {
        await Read();
        selectedQuestion = GameQuiz.questions.FindAll(x => x.category == category);
    }

    #region Read Question

    private bool _ready = false;
    private string _fileLocation
#if UNITY_EDITOR
        = "C:\\Users\\psuchet\\Documents\\JVS_Olympics\\Personnalisation\\Quizz";
#else
         = "Personnalisation\\Quizz\\";
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
        Debug.Log("Start Reading");
        GameQuiz.questions.Clear();
        string appPath = Application.dataPath;

        string newPath = Path.GetFullPath(Path.Combine(appPath, @"..\..\..\..\"));
        newPath = Path.GetFullPath(Path.Combine(newPath, _fileLocation));

        string csv = File.ReadAllText(_fileLocation + "\\Questions.csv", Encoding.GetEncoding("ISO-8859-1"));
        //Debug.Log(csv.Contains("’"));
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
            Debug.Log((sprite.Result == null) + " | " + Path.Combine(newPath, data[lineLength * i + 1]));

            currentQuestion.category = CategoryFromString(data[lineLength * i + 7]);
            GameQuiz.questions.Add(currentQuestion);
            QuestionImage.sprite = currentQuestion.image;
        }
        _ready = true;
        //_currentQuestions = new List<Question>(_questions);
        Debug.Log("Finish Reading");
    }

    private int ValueFromString(string text)
    {
        int res = 0;
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
        switch (text)
        {
            case "GeneralCulture":
                return Category.GeneralCulture;

            case "Sport":
                return Category.Sport;

            case "People":
                return Category.People;

            default:
                return Category.GeneralCulture;
        }
    }

    #endregion

}
