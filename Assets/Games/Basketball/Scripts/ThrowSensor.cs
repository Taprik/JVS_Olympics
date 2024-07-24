using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basket
{
    public class ThrowSensor : MonoBehaviour, IReceivePoint
    {
        //[SerializeField] float _throwForce;
        [SerializeField] Camera _cam;
        [SerializeField] CinemachineVirtualCamera _virtualCam;
        [SerializeField] GameObject _ballPref;
        [SerializeField] Transform _aimPoint;
        [SerializeField] BoxCollider _spawnZone;

        private bool _canShot;
        public bool CanShot 
        {
            get
            {
                return _canShot && !Basket_GameManager.i.IsGameOver && !Basket_GameManager.i.Teams[Team ? 1 : 0].Next;
            } 
            
            set
            {
                _canShot = value && !Basket_GameManager.i.IsGameOver && !Basket_GameManager.i.Teams[Team ? 1 : 0].Next;
            } 
        }

        public bool Team;

        public void ReceivePoint(float xPoint, float yPoint)
        {
            if (!CanShot) return;

            //xPoint *= Screen.width;
            //yPoint *= Screen.height;
            Vector3 hit = new Vector3(xPoint, yPoint, 2f);
            //Debug.Log(hit);

            if (((_cam.rect.x < 0 && xPoint < Screen.width / 2) || (_cam.rect.x > 0 && xPoint > Screen.width / 2)) && CanShot)
            {
                CanShot = false;
                StartCoroutine(Delay());

                var pointer = _cam.WorldToScreenPoint(_virtualCam.LookAt.position);
                
                var x = (hit.x - pointer.x) / 480f;
                var y = (hit.y - pointer.y) / 560f;

                //Debug.Log($"{x};{y}");

                var spawnPos = _spawnZone.transform.position + 
                (
                    Quaternion.Euler(0, _spawnZone.transform.eulerAngles.y - 180f, 0) * 
                    new Vector3
                        (
                            x * Basket_GameManager.i.Teams[Team ? 1 : 0].Multiplicator * 1.5f,
                            0,
                            y * Basket_GameManager.i.Teams[Team ? 1 : 0].Multiplicator * 1.5f
                        )
                );

                //Debug.Log($"{pointer} | Pos : {spawnPos} | Hit : {hit}");

                var ball = Instantiate(_ballPref, spawnPos, Quaternion.identity);
                
                ///Old Script (� garder au cas o� on reprend l'ancien system)
                //var rb = ball.GetComponent<Rigidbody>();
                //var dir = new Vector3((pos - ball.transform.position).x, 0f, (pos - ball.transform.position).z) + (3 * Vector3.up);
                //var force = (Mathf.Clamp01(pos.y - 1.75f) * 117.65f) + 390f;
                //rb.AddForce(dir.normalized * force);


                ball.GetComponent<BallSensor>().sensor = this;
                StartCoroutine(Basket_GameManager.i.LookAtBall(_virtualCam, ball.transform, Team));
                Basket_GameManager.i.Teams[Team ? 1 : 0].AllBalls.Add(ball);

                var colliders = new List<CapsuleCollider>();
                foreach (var b in Basket_GameManager.i.Teams[Team ? 1 : 0].AllBalls)
                    if(b != null) colliders.Add(b.GetComponent<CapsuleCollider>());

                Basket_GameManager.i.Teams[Team ? 1 : 0].NetCloth.capsuleColliders = colliders.ToArray();
            }

        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.2f);
            CanShot = true;
        }
    }
}