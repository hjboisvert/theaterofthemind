using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFloatPixels : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //OVRManager manager = (OVRManager)FindObjectOfType(typeof(OVRManager));
        //OVRManager.EyeTextureFormat fmt = OVRPlugin.EyeTextureFormat.R11G11B10_FP;
        OVRManager.eyeTextureFormat = (OVRManager.EyeTextureFormat)OVRManager.EyeTextureFormat.R11G11B10_FP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
