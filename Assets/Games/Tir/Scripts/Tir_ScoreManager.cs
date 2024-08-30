using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Tir
{
    public class Tir_ScoreManager : MonoBehaviour
    {
        public TirTeam[] Teams => _teams;
        [SerializeField]
        TirTeam[] _teams;

        [SerializeField]
        int _multiplicator;

        [SerializeField]
        float _timeBeforePopUpGetDestroy;

        [SerializeField]
        TextMeshProUGUI _congratulationText;

        private void Start()
        {
            Tir_GameManager.Instance.OnGameStart += InitTeam;
            Tir_GameManager.Instance.OnGameEnd += SaveWinnerScore;
            Tir_GameManager.Instance.OnGameEnd += ShowCongratulation;
        }

        public void AddScore(TirTeam team, int score, RectTransform rect)
        {
            team.UpdateNameAndScore();
            int pts = (10 - score) * _multiplicator;
            team.Score += pts;
            team.PopUpAnimator.gameObject.GetComponent<TextMeshProUGUI>().text = "+" + pts.ToString();

            StartCoroutine(PopUp(team));

            //TextMeshProUGUI text = Instantiate(_popUpPrefab, rect.transform.position, Quaternion.identity, _popUpParent).GetComponent<TextMeshProUGUI>();
            ////Debug.Log(GetPositionPopUp(rect, text.gameObject.GetComponent<RectTransform>(), team));
            //text.gameObject.transform.position = GetPositionPopUp(rect, text.gameObject.GetComponent<RectTransform>(), team);
            ////Debug.Log(text.gameObject.transform.position);
            //text.text = "+" + pts.ToString();
            //Destroy(text.gameObject, _timeBeforePopUpGetDestroy);
        }

        private IEnumerator PopUp(TirTeam team)
        {
            team.EndAnimation = false;
            team.PopUpAnimator.gameObject.SetActive(true);

            team.PopUpAnimator.SetTrigger("Trigger");
            yield return new WaitForSeconds(_timeBeforePopUpGetDestroy);
            team.PopUpAnimator.SetTrigger("Trigger");

            yield return new WaitUntil(() => team.EndAnimation);
            team.UpdateNameAndScore();
        }

        public void EndAnimation(int id)
        {
            TirTeam team = Teams[id];
            team.EndAnimation = true;
        }

        private Vector3 GetPositionPopUp(RectTransform rectTarget, RectTransform rectPopUp, TirTeam team)
        {
            Vector3 dir = team.Id == 0 ? Vector3.left : Vector3.right;
            Vector3 startingPos = rectTarget.transform.position;
            int id = rectTarget.gameObject.GetComponent<TargetBehaviour>().CurrentID;
            Vector3 posiblePos = startingPos + new Vector3(dir.x * ((rectTarget.rect.size.x / 2 / 10 * (id + 1)) + (rectPopUp.rect.size.x / 2)), 0);

            if (posiblePos.x + (rectPopUp.rect.size.x / 2) < Screen.width && posiblePos.x - (rectPopUp.rect.size.x / 2) > 0)
                return posiblePos;
            else
                return startingPos + new Vector3(-1 * dir.x * ((rectTarget.rect.size.x / 2 / 10 * (id + 1)) + (rectPopUp.rect.size.x / 2)), 0);
        }

        private void InitTeam()
        {
            foreach (var team in Teams)
            {
                if (PlayerPrefs.HasKey(Tir_SceneObject.PlayerNameKey + (team.Id + 1).ToString()))
                    team.Name = PlayerPrefs.GetString(Tir_SceneObject.PlayerNameKey + (team.Id + 1).ToString());
                team.Score = 0;
                team.UpdateNameAndScore();
                team.LastSpawn = DateTime.MinValue;
                foreach (var target in team.SpawnTargets)
                {
                    Destroy(target);
                }
                team.SpawnTargets = new List<TargetBehaviour>();
                team.PopUpAnimator.gameObject.SetActive(false);
                team.EndAnimation = true;
            }
            _congratulationText.transform.parent.gameObject.SetActive(false);
        }

        private void SaveWinnerScore()
        {
            Tir_GameManager.Instance.OnGameEnd -= SaveWinnerScore;
            TirTeam team = Teams.OrderBy(x => x.Score).Last();
            PlayerPrefs.SetFloat(Tir_SceneObject.WinnerScoreKey, team.Score);
        }

        private void ShowCongratulation()
        {
            Tir_GameManager.Instance.OnGameEnd -= ShowCongratulation;
            string text = "";
            
            if (Teams[0].Score != Teams[1].Score)
                text = "Bravo " + Teams.OrderBy(x => x.Score).Last().Name + " ! \nVous avez gagné !!!";
            else
                text = "Egalité ! \nAucun Gagnant";
            
            _congratulationText.transform.parent.gameObject.SetActive(true);
            _congratulationText.text = text;
        }
    }

    [System.Serializable]
    public class TirTeam
    {
        public string Name;
        public byte Id;
        public byte SpriteId;
        public RectTransform TargetHolder;
        public Animator PopUpAnimator;
        [HideInInspector] public bool EndAnimation;
        public int Score;
        public DateTime LastSpawn;
        public TextMeshProUGUI NameAndScore;
        [HideInInspector] public List<TargetBehaviour> SpawnTargets;

        public void UpdateNameAndScore()
        {
            NameAndScore.text = $"<sprite={SpriteId}> {Name}    <b><size=150%>{Score} pts</size></b>";
        }
    }
}