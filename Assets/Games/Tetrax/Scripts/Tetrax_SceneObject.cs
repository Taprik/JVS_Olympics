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
            Tetrax_GameManager.Instance.OnNameReceive(name);
        }

        public override void PageDown()
        {
            throw new System.NotImplementedException();
        }

        public override void PageUp()
        {
            throw new System.NotImplementedException();
        }

        public override Task Replay()
        {
            throw new System.NotImplementedException();
        }
    }
}