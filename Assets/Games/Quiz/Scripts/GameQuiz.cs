using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Quiz", menuName = "Game/Quiz/Game_Quiz")]
public class GameQuiz : GameSO
{
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
