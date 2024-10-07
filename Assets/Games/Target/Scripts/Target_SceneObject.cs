using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Target
{
    public class Target_SceneObject : GameSceneObject
    {
        public override void OnNameReceive(string name)
        {
            Target_GameManager.Instance.OnReceiveName(name);
        }

        public override void PageDown()
        {
            Target_GameManager.Instance.ScoreBaord.PageDown();
        }

        public override void PageUp()
        {
            Target_GameManager.Instance.ScoreBaord.PageUp();
        }

        public async override Task Replay()
        {

        }
    }
}