using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadsetIdleScript : MonoBehaviour
{
    private OVRManager vrManager;
    // Start is called before the first frame update
    void Start()
    {
        vrManager = (OVRManager)FindObjectOfType(typeof(OVRManager));
    }

    // Update is called once per frame
    void Update()
    {
        if (vrManager.isUserPresent && !XRSettings.enabled)
        {
            Debug.Log("HeadsetIdleScript: Enable VR");
            StartCoroutine(EnableVR());
        }
        else if (!vrManager.isUserPresent && XRSettings.enabled)
        {
            Debug.Log("HeadsetIdleScript: Disable VR");
            StartCoroutine(DisableVR());
        }
    }

    IEnumerator EnableVR()
    {
        XRSettings.enabled = true;
        yield return null; // wait one frame
    }

    IEnumerator DisableVR()
    {
        XRSettings.LoadDeviceByName("");
        yield return null; // wait one frame
        XRSettings.enabled = false;
        yield return null; // wait one frame
        XRSettings.LoadDeviceByName("Oculus");
        yield return null; // wait one frame
    }
}
