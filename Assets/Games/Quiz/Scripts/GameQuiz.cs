using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Tool;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Quiz
{
    [CreateAssetMenu(fileName = "Game_Quiz", menuName = "Game/Quiz/Game_Quiz")]
    public class GameQuiz : GameSO
    {
        [Header("Video")]
        [SerializeField] VideoClip[] VideoAnimOrange;
        [SerializeField] VideoClip[] VideoAnimBlue;
        public VideoClip[] VideoAnimRed;
        public VideoClip[] GetVideoClipOfTeam(int teamID) => teamID == 0 ? VideoAnimBlue : teamID == 1 ? VideoAnimOrange : null;

        [Header("Sprite")]
        [SerializeField] Sprite[] AnswerLetterSprites;
        public Sprite GetAnswerLetterSprite(int id) => AnswerLetterSprites.Length >= id + 1 ? AnswerLetterSprites[id] : null;

        [SerializeField]
        Sprite _defaultImage;
        public Sprite GetDefaultImage() => _defaultImage;

        [Header("Question")]
        public List<Quiz_Question> Questions;

        [Header("Font")]
        [SerializeField] List<TMP_FontAsset> TeamsFonts;
        public TMP_FontAsset GetFontAsset(TeamFontColor color) => TeamsFonts[(int)color];

        public enum TeamFontColor
        {
            None = 3,
            Orange = 1,
            Blue = 0,
            Red = 2
        }

        [Header("Sound")]
        [SerializeField] AudioClip[] RightAnswerSound;
        public AudioClip GetRandomRightAnswerSound() => RightAnswerSound[Random.Range(0, RightAnswerSound.Length)];

        [SerializeField] AudioClip[] WrongAnswerSound;
        public AudioClip GetRandomWrongAnswerSound() => WrongAnswerSound[Random.Range(0, WrongAnswerSound.Length)];

        public AudioClip StartSound;
        public AudioClip GainPtsSound;
        public AudioClip EndTimerSound;

        public async override Task GameInit()
        {
            _ready = false;
            await Read();
        }

        private bool _ready = false;
        private const string _fileLocation
#if UNITY_EDITOR
        = "Documents\\Capteur\\Personnalisation\\Quiz";
#else
        = "Personnalisation\\Quiz";
#endif

        public bool IsReady() => _ready;

        private async Task Read()
        {
            if (IsReady())
                return;

            Debug.Log("Read ! ");

            try
            {
                _ready = false;
                Questions.Clear();
                string appPath = Application.dataPath;
                string newPath = Path.GetFullPath(Path.Combine(appPath, @"..\..\..\..\"));
                newPath = Path.GetFullPath(Path.Combine(newPath, _fileLocation));

                string csv = File.ReadAllText(newPath + "\\Questions.csv"/*, Encoding.GetEncoding("ISO-8859-1")*/);
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

                    if (File.Exists(Path.Combine(newPath, data[lineLength * i + 1])))
                    {
                        Sprite sprite = ToolBox.CreateSpriteFromPath(Path.Combine(newPath, data[lineLength * i + 1]));
                        currentQuestion.image = sprite == null ? GetDefaultImage() : sprite;
                    }
                    else
                    {
                        currentQuestion.image = GetDefaultImage();
                    }


                    currentQuestion.category = CategoryFromString(data[lineLength * i + 7]);
                    Questions.Add(currentQuestion);
                }
                _ready = true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            
            Debug.Log("Finish Reading");
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

        int ValueFromString(string text)
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

    }

    [System.Serializable]
    public class Quiz_Question
    {
        public string sentence;
        public Sprite image;
        public Category category;

        public string[] answers;
        public int correctAnswer;
    }

    public enum Category
    {
        GeneralCulture,
        Sport,
        People,
    }
}