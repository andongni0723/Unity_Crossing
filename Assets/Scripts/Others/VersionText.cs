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
        versionText.text = "v" + Application.version;
    }
}
