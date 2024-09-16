using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Tetrax
{
    public class Tetrax_SceneObject : GameSceneObject
    {


        public override void OnNameReceive(string name)
        {
            Tetrax_ScoreManager.Instance.OnReceiveName(name);
        }

        public override void PageDown()
        {
            Tetrax_ScoreManager.Instance.PageDown();
        }

        public override void PageUp()
        {
            Tetrax_ScoreManager.Instance.PageUp();
        }

        public override Task Replay()
        {
            throw new System.NotImplementedException();
        }
    }
}