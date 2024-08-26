using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Basket
{
    public class BasketSceneObject : GameSceneObject
    {
        [SerializeField] GameBasket GameSO;

        [Header("Home")]
        [SerializeField] GameObject _homePage;
        public void SetHomePage(bool isActive) => _homePage.SetActive(isActive);

        [Header("Game")]
        [SerializeField] GameObject _gamePage;
        public void SetGamePage(bool isActive) => _gamePage.SetActive(isActive);

        [Header("Menu")]
        [SerializeField] GameObject _menuPage;
        public void SetMenuPage(bool isActive) => _menuPage.SetActive(isActive);

        [Header("Score")]
        [SerializeField] GameObject _scorePage;
        public void SetScorePage(bool isActive) => _scorePage.SetActive(isActive);

        public override void Start()
        {
            base.Start();
            GameManager.CurrentGame = GameSO;
            SetHomePage(true);
            SetGamePage(false);
            SetMenuPage(false);
            SetScorePage(false);
        }

        public async override Task InitScene()
        {
            await base.InitScene();
            
        }

        public override void Play()
        {
            SetHomePage(false);
            SetGamePage(true);
            SetMenuPage(false);
            SetScorePage(false);
            base.Play();
        }

        public void PlayScore()
        {
            SetHomePage(false);
            SetGamePage(false);
            SetMenuPage(false);
            SetScorePage(true);
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
            SetHomePage(false);
            SetGamePage(false);
            SetMenuPage(true);
            SetScorePage(false);
        }
    }
}