using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchTransitionBlock : MonoBehaviour
{
    // Start is called before the first frame update
    public void LaunchTransition(string sceneName) 
    {
        Transitioner.Instance.TransitionToScene(sceneName);
    }  

}
