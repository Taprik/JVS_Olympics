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
            base.Play();
            SetHomePage(false);
            SetMenuPage(false);
            SetScorePage(false);
            SetGamePage(true);
        }

        public override void OnNameReceive(string name)
        {

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
    }
}