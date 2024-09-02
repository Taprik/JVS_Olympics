using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Tetrax
{
    public class Animate_accueil : MonoBehaviour
    {
        public GameObject carreRouge;
        public GameObject carreBleu;
        public GameObject startIndication;
        public TextMeshProUGUI playerNameText;
        public GameObject playerNameIndication;
        public float timeNameIndication = 5f;

        private Vector3 ThisSize;

        private float timeStep;
        private float timeLastChange;
        private float timeStart;
        private float currentTimeNameIndication;

        void Start()
        {
            timeStep = Random.Range(0.0f, 0.5f);
            timeLastChange = timeStart = Time.time;

            ThisSize = gameObject.GetComponent<Renderer>().bounds.size;

            if (PlayerPrefs.GetInt("Tetrax_ShowIndication") == -1) // check if we want to show indications (can be set on/off in the menu)
            {
                startIndication.SetActive(false);
            }
            else
            {
                startIndication.SetActive(true);
            }
        }

        void Update()
        {
            if (Time.time - timeLastChange > timeStep)
            {
                timeLastChange = Time.time;
                timeStep += Random.Range(0.0f, 0.5f);
                GameObject newcube;
                Quaternion LRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //on place le fake au coin haut gauche de la ligne
                Vector3 randomPos = new Vector3(Random.Range(-ThisSize.x / 3.0f, ThisSize.x / 3.0f), ThisSize.y / 1.5f, ThisSize.z);
                Vector3 LPos = randomPos;
                //Debug.Log(randomPos.ToString());
                bool decide = (Random.value < 0.5);
                if (decide) newcube = Instantiate(carreRouge, LPos, LRotation);
                else newcube = Instantiate(carreBleu, LPos, LRotation);
            }
            if (Time.time - timeStart > 20.0f) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            if (currentTimeNameIndication > 0)
            {
                currentTimeNameIndication -= Time.deltaTime;
            }
            else if (currentTimeNameIndication < 0)
            {
                currentTimeNameIndication = 0;
                playerNameIndication.SetActive(false);
            }
        }

        void RegisterPlayerOneName(string playerOneName)
        {
            playerNameText.text = "Le nom du joueur 1 est maintenant : " + "<size=100>" + playerOneName + "</size>";
            playerNameIndication.SetActive(true);
            currentTimeNameIndication = timeNameIndication;
            PlayerPrefs.SetString("Player1Name", playerOneName);
        }
    }
}