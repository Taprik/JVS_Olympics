using System.Collections;
using System.Collections.Generic;
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

    public List<Quiz_Question> questions;
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
