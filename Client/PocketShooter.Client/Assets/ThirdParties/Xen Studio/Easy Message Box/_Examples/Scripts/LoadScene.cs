using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XenStudio.UI
{
    public class LoadScene : MonoBehaviour 
    {
        public void Load(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }
    }
}