using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tir
{
    public class Tir_MenuManager : MonoBehaviour
    {
        public static Tir_MenuManager Instance => _instance;
        private static Tir_MenuManager _instance;

        public void Awake()
        {
            if (Instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);
        }

        [SerializeField]
        TMP_Dropdown _dropDownTimer;

        [SerializeField]
        TextMeshProUGUI _timeBetweenPhaseText;
        [SerializeField]
        Slider _timeBetweenPahseSlider;

        [SerializeField]
        TMP_Dropdown _dropDownDifficulty;

        [SerializeField]
        TMP_InputField _playerOneInput;
        [SerializeField]
        TMP_InputField _playerTwoInput;

        [SerializeField]
        Toggle _stickerToggle;

        public void Start()
        {
            if (PlayerPrefs.HasKey(Tir_SceneObject.TimerKey))
                _dropDownTimer.value = (PlayerPrefs.GetInt(Tir_SceneObject.TimerKey) - 60) / 30;

            if (PlayerPrefs.HasKey(Tir_SceneObject.DifficultyKey))
                _dropDownDifficulty.value = PlayerPrefs.GetInt(Tir_SceneObject.DifficultyKey);

            if (PlayerPrefs.HasKey(Tir_SceneObject.TimeBetweenPhaseKey))
                _timeBetweenPahseSlider.value = PlayerPrefs.GetFloat(Tir_SceneObject.TimeBetweenPhaseKey);

            _timeBetweenPhaseText.text = (Mathf.RoundToInt(_timeBetweenPahseSlider.value * 1000f) / 100f).ToString();

            if (PlayerPrefs.HasKey(Tir_SceneObject.StickerKey))
                _stickerToggle.isOn = PlayerPrefs.GetInt(Tir_SceneObject.StickerKey) == 1;
            else 
                _stickerToggle.isOn = false;
        }

        public void SetTimer()
        {
            //Debug.Log(_dropDownTimer.value);
            PlayerPrefs.SetInt(Tir_SceneObject.TimerKey, (_dropDownTimer.value * 30) + 60);
        }

        public void ResetScoreBoard()
        {
            //ScoreBoardManager.CreateScoreBoard(GameScoreBoard.TirScoreBoard);
        }

        public void SaveTimeBetweenPhase()
        {
            PlayerPrefs.SetFloat(Tir_SceneObject.TimeBetweenPhaseKey, Mathf.RoundToInt(_timeBetweenPahseSlider.value * 100f) / 100f);
            _timeBetweenPhaseText.text = (Mathf.RoundToInt(_timeBetweenPahseSlider.value * 1000f) / 100f).ToString();
        }

        public void SaveDifficulty()
        {
            PlayerPrefs.SetInt(Tir_SceneObject.DifficultyKey, _dropDownDifficulty.value);
        }

        public void SaveName(int id)
        {
            if (id != 1 && id != 2) return;
            PlayerPrefs.SetString(Tir_SceneObject.PlayerNameKey + id.ToString(), id == 1 ? _playerOneInput.text : id == 2 ? _playerTwoInput.text : string.Empty);
        }

        public void SaveSticker()
        {
            PlayerPrefs.SetInt(Tir_SceneObject.StickerKey, _stickerToggle.isOn ? 1 : 0);
        }
    }
}