using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRRecenter : MonoBehaviour
{
    private OVRCameraRig cam;
    private Vector3 initialAngles;
    // Start is called before the first frame update
    void Start()
    {
        initialAngles = transform.eulerAngles;
        cam = (OVRCameraRig)FindObjectOfType(typeof(OVRCameraRig));
    }

    // Called from here on key press or from OscScript
    public void Recenter() {
        Debug.Log("Recentering");
        //cam.rightEyeAnchor = Quaternion.Euler(0, 0, 0);
        //cam.leftEyeAnchor = Quaternion.Euler(0, 0, 0);
        Transform camTransform = cam.centerEyeAnchor.transform;
        Vector3 currentAngles = transform.eulerAngles;
        Vector3 camAngles = camTransform.eulerAngles;
        transform.eulerAngles = initialAngles + camAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Recenter"))
        {
            Recenter();
        }
        
    }
}
