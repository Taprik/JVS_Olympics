using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class ButtonPartRotation : ButtonParent
    {
        public Action Rotate;

        public override void DoWork()
        {
            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.gameObject.transform.rotation.eulerAngles.z - 90));
            Rotate?.Invoke();
            if (GameManager.Instance.CurrentGameSceneObject != null && GameManager.Instance.CurrentGameSceneObject is BlockSceneObject)
                AudioSource.PlayClipAtPoint((GameManager.Instance.CurrentGameSceneObject as BlockSceneObject).GameBlockSo.AudioClic, Vector3.zero);
            if (this.gameObject.transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                IsActive = false;
        }
    }
}