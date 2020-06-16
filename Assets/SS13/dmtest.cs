using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SS13;
using UnityEngine;

public class dmtest : MonoBehaviour
{
    private string DmFolder = Application.streamingAssetsPath;

    void Start()
    {
        var code = new ByondCode(DmFolder);
    }
    
    void Update()
    {
        
    }
}
