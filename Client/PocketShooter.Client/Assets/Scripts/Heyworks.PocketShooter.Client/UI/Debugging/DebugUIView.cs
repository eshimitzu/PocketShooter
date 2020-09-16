using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIView : MonoBehaviour
{
    [SerializeField]
    private Text debugInfo;

    public void Setup(string debugInfo)
    {
        this.debugInfo.text = debugInfo;
    }
}
