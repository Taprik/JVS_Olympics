using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Game_Quiz", menuName = "Game/Quiz/Game_Quiz")]
public class GameQuiz : GameSO
{
    public VideoClip[] VideoAnimOrange;
    public VideoClip[] VideoAnimBlue;
    public VideoClip[] VideoAnimRed;
    public VideoClip[] GetVideoClipOfTeam(int teamID) => teamID == 0 ? VideoAnimBlue : teamID == 1 ? VideoAnimOrange : null;

    public Sprite[] AnswerLetterSprites;
    public Sprite GetAnswerLetterSprite(int id) => AnswerLetterSprites.Length >= id + 1 ? AnswerLetterSprites[id] : null;

    public List<Quiz_Question> Questions;

    public List<TMP_FontAsset> TeamsFonts;
    public TMP_FontAsset GetFontAsset(TeamFontColor color) => TeamsFonts[(int)color];

    public enum TeamFontColor
    {
        None = 3,
        Orange = 1,
        Blue = 0,
        Red = 2
    }
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
