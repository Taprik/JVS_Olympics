using OSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Tir
{
    public class Tir_SceneObject : GameSceneObject
    {
        public const string WinnerScoreKey = "Tir_WinnerScore";
        public const string TimerKey = "Tir_Timer";
        public const string TimeBetweenPhaseKey = "Tir_TimeBetweenPhase";
        public const string DifficultyKey = "Tir_Difficulty";
        public const string PlayerNameKey = "Tir_PlayerName";
        public const string StickerKey = "Tir_Sticker";

        public override async Task Replay()
        {

        }

        public override void OnNameReceive(string name)
        {
            Tir_ScoreSceneManager.Instance.OnReceiveName(name);
        }

        public override void PageUp()
        {

        }

        public override void PageDown()
        {

        }

    }
}