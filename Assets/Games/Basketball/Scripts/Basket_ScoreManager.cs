using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basket
{
    public class Basket_ScoreManager : MonoBehaviour
    {
        public void AddScore(bool IsP1)
        {
            BasketTeam team =Basket_GameManager.i.Teams[IsP1 ? 1 : 0];
            team.Score++;
            team.ScoreDisplay.DisplayScore(team.Score);
            team.ScoreText.text = $"{team.Score.ToString("00")}pts";
            team.Next = true;
        }
    }
}