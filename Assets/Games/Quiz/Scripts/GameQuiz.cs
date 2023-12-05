using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

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
}

[System.Serializable]
public class Quiz_Question
{
    public string sentence;
    public Sprite image;
    public Category category;
    public float reflectionTime;
    public float answersTime;

    public string[] answers;
    public int correctAnswer;

}

public enum Category
{
    GeneralCulture,
    Sport,
    People,
}
