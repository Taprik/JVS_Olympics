using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Basket
{
    public class BasketSceneObject : GameSceneObject
    {
        [SerializeField] GameBasket GameSO;

        public override void Start()
        {
            base.Start();
            GameManager.CurrentGame = GameSO;
        }

        public async override Task InitScene()
        {
            await base.InitScene();
            
        }

        public override void Play()
        {
            base.Play();
        }

        public override void PlayScore()
        {
            base.PlayScore();
        }

        public override void OnNameReceive(string name)
        {
            Basket_ScoreBoardManager.i.OnReceiveName(name);
        }

        public override void PageDown()
        {

        }

        public override void PageUp()
        {

        }

        public async override Task Replay()
        {

        }

        public override void OpenMenu()
        {
            
        }
    }
}