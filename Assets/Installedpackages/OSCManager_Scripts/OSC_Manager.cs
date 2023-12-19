using UnityEngine;
using OscSimpl;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace OSC
{
    public class OSC_Manager : MonoBehaviour
    {
        //receiver
        public OscIn _oscIn;
        //sender
        public OscOut _oscOut;

        [SerializeField] private int portIn = 7005;
        [SerializeField] private int portOut = 8000;

        //pour le projet
        const string remoteAccueilTous = "/remote/AccueilTous";     //argument 1
        const string remoteQuit = "/remote/Quit";                 //argument 1
                                                                  //pour les jeux
        const string lesImpacts = "/point";                     //list de float
        const string remoteStart = "/remote/Start";         //argument nom du jeu
        const string remoteAccueil = "/remote/Accueil";     //argument nom du jeu
        const string remoteNameGamer = "/remote/nameGamer";                 //argument nom du joueur
        const string remoteCalibrage = "/remote/Calibrage";                 //argument nom du joueur
        const string remoteVelo = "/remote/Velo";                           //argument intervalle pédale et angle du guidon*

        //pour communiquer avec l'appli Interface
        const string nomJoueur = "/remote/Name";             //argument 1
        //Envoyer lorsqu'une GameScene est lancée
        const string accueilAppli = "/remote/Accueil";      //argument 1
        const string enCours = "/remote/Encours";           //argument nom du jeu
        const string remoteQuitAll = "/remote/Quit";         //argument 1
        const string photoMonstresDemo = "/remote/PhotoMonstresDemo";   //argument 1
        const string photoBlockQuestion = "/remote/PhotoblockQuestion"; //argument 1
        const string hide = "/remote/Hide";                             //arguement 1
        const string show = "/remote/Show";                             //argument 1
        const string ready = "/remote/Ready";                            //argument 1
        private const int _nbreOfCharacter = 25;
        OscMessage _message;

        [HideInInspector] public string playerOneName, playerTwoName;
        public delegate void OnNameEnter(string test);
        public OnNameEnter nameDelegate;

        //Mandatory for Monstres game
        public List<Sprite> playersSprites;

        //Mandatory for Photoblock game
        public string chosenPicture;


        public bool inOpened;
        public bool outOpened;

        [SerializeField] GameObject ImpactPref;
        [SerializeField] RectTransform Canvas;

        void Start()
        {
            // Ensure that we have a OscIn component and start receiving on port 7000.
            if (!_oscIn) _oscIn = gameObject.AddComponent<OscIn>();
            inOpened = _oscIn.Open(portIn);      //TODO : faire que ce soit une variable

            // Ensure that we have a OscOut component.
            if (!_oscOut) _oscOut = gameObject.AddComponent<OscOut>();

            // Prepare for sending messages locally on this device on port 7000.
            outOpened = _oscOut.Open(portOut);     //TODO : faire que ce soit une variable
        }

        void OnEnable()
        {
            //mapping des messages
            _oscIn.MapInt(remoteAccueilTous, onOSCAccueilTous);
            _oscIn.MapInt(remoteQuit, onOSCQuit);
            //_oscIn.MapInt(remoteCalibrage, onOSCCalibrage);

            _oscIn.Map(remoteVelo, onOSCVelo);
            _oscIn.Map(lesImpacts, onOSCPoint);
            _oscIn.Map(remoteStart, onOSCStart);
            _oscIn.Map(remoteAccueil, onOSCAccueil);
            _oscIn.Map(remoteNameGamer, onOSCNameGamer);
        }

        void OnDisable()
        {
            // If you want to stop receiving messages you have to "unmap".
            _oscIn.UnmapInt(onOSCAccueilTous);
            _oscIn.UnmapInt(onOSCQuit);
            //_oscIn.UnmapInt(onOSCCalibrage);

            _oscIn.Unmap(onOSCVelo);
            _oscIn.Unmap(onOSCPoint);
            _oscIn.Unmap(onOSCStart);
            _oscIn.Unmap(onOSCAccueil);
            _oscIn.Unmap(onOSCNameGamer);
        }

        private void onOSCVelo(OscMessage message)
        {
            int intervale;
            message.TryGet(0, out intervale);
            int angle;
            message.TryGet(1, out angle);

            OscPool.Recycle(message);
        }

        //RECEPTION
        public void onOSCAccueilTous(int value)
        {
            _message = new OscMessage(remoteAccueilTous);
            _message.Set(0, 1);
            _oscOut.Send(_message);
            GameManager.Instance.GameSceneManager.LoadScene(SceneName.MainScene);
        }

        public void onOSCQuit(int value)
        {
            Application.Quit();
        }

        //public void onOSCCalibrage(int value)
        //{
        //    SceneManager.LoadScene("Calibrage");
        //}

        public void onOSCPoint(OscMessage message)
        {
            float impactX;
            message.TryGet(0, out impactX);
            impactX *= Screen.width;
            float impactY;
            message.TryGet(1, out impactY);
            impactY *= Screen.height;

            var receiveParents = FindObjectsOfType<MonoBehaviour>().OfType<IReceivePoint>();
            foreach (IReceivePoint rp in receiveParents)
            {
                rp.ReceivePoint(impactX, impactY);
            }

            Destroy(Instantiate(ImpactPref, new Vector3(impactX, impactY, 0), Quaternion.identity, Canvas), 1f);

            // Always recycle incoming messages when used.
            OscPool.Recycle(message);
        }

        public void onOSCStart(OscMessage message)
        {
            //ouvrir la scene game du jeu
            string nomJeu = "";
            message.TryGet(0, ref nomJeu);

            if(GameManager.Instance.CurrentGameSceneObject != null)
            {
                GameManager.Instance.CurrentGameSceneObject.Play();
            }

            Debug.Log(nomJeu);

            // Always recycle incoming messages when used.
            OscPool.Recycle(message);
        }

        public void onOSCAccueil(OscMessage message)
        {
            //ouvrir la scene accueil du jeu (sauf pour Monstres & Photoblock)
            string nomJeu = "";
            message.TryGet(0, ref nomJeu);

            //SceneManager.LoadScene("Accueil_" + nomJeu);
            int sceneID = 0;

            switch (nomJeu)
            {
                case "Blocks":
                    sceneID = 1;
                    break;
                case "Quiz":
                    sceneID = 2;
                    break;
                default:
                    break;
            }

            GameManager.Instance.GameSceneManager.LoadScene((SceneName)sceneID);

            // Always recycle incoming messages when used.
            OscPool.Recycle(message);
        }

        public void onOSCNameGamer(OscMessage message)
        {
            //on renseigne le nom du joueur pour le tableau des scores
            string nomGamer = "";

            message.TryGet(0, ref nomGamer);
            SendGamerName(nomGamer);

            if(GameManager.Instance.CurrentGameSceneObject != null)
                GameManager.Instance.CurrentGameSceneObject.OnNameReceive(nomGamer);

            // Always recycle incoming messages when used.
            OscPool.Recycle(message);
        }
       
        private void SendGamerName(string nomGamer)
        {
            nomGamer = Regex.Replace(nomGamer, @"\s+", " ");
            nomGamer = nomGamer.Substring(0, Mathf.Min(nomGamer.Length, _nbreOfCharacter));
        }

        //ENVOI
        public void messageOutQuit()
        {
            _message = new OscMessage(remoteQuitAll);
            _message.Set(0, 1);
            _oscOut.Send(_message);
            Application.Quit();
        }

        public void GameEnCours()
        {
            _message = new OscMessage(enCours);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void NeedName()
        {
            _message = new OscMessage(nomJoueur);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void PhotoMonstresDemo()
        {
            _message = new OscMessage(photoMonstresDemo);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void PhotoBlockQuestion()
        {
            _message = new OscMessage(photoBlockQuestion);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void onOSCAccueilAppli()
        {
            _message = new OscMessage(accueilAppli);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void Hide()
        {
            _message = new OscMessage(hide);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void Show()
        {
            _message = new OscMessage(show);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }

        public void Ready()
        {
            _message = new OscMessage(ready);
            _message.Set(0, 1);
            _oscOut.Send(_message);
        }
    }
}
