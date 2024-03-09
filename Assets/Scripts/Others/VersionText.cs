using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    // [Header("Component")]
    private TextMeshProUGUI versionText;
    
    //[Header("Settings")]
    //[Header("Debug")]
    
    private void Awake()
    {
        versionText = GetComponent<TextMeshProUGUI>();
        versionText.text = "";

        if (Application.platform == RuntimePlatform.Android)
            versionText.text += "Android";
        else if (Application.platform == RuntimePlatform.WindowsEditor)
            versionText.text += "Windows";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            versionText.text += "iOS";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            versionText.text += "WebGL";
        else if (Application.platform == RuntimePlatform.OSXEditor)
            versionText.text += "MacOS";
        else if (Application.platform == RuntimePlatform.LinuxPlayer)
            versionText.text += "Linux";
        else
            versionText.text += "Unknown";
        
        versionText.text += " v" + Application.version;
    }
}
