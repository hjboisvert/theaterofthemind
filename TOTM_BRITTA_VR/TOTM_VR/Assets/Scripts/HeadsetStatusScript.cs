using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadsetStatusScript: MonoBehaviour
{
    private OVRManager vrManager;
    private XRDisplaySubsystem headsetDisplay;
    private OVRCameraRig cameraRig;

    public bool bHeadsetAwake = false;
    private float elapsedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        vrManager = (OVRManager)FindObjectOfType(typeof(OVRManager));

        OVRManager.SetSpaceWarp(false);

        cameraRig = (OVRCameraRig)FindObjectOfType(typeof(OVRCameraRig));
        
        DisableVR();

        // device discovery
        string match = "oculus";

        List<XRDisplaySubsystemDescriptor> displays = new List<XRDisplaySubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(displays);
        Debug.Log("Idle script number of display providers found: " + displays.Count);

        foreach (var d in displays)
        {
            //Debug.Log("Display id: " + d.id);

            if (d.id.Contains(match))
            {
                Debug.Log("Idle script creating display: " + d.id);
                headsetDisplay = d.Create();

                if (headsetDisplay != null)
                {
                    Debug.Log("Headset Status Script: starting listeners " + d.id);
                    //headsetDisplay.Start();
                    //OVRManager.HMDMounted += OnHeadsetMounted;
                    OVRManager.VrFocusAcquired += OnFocusAcquired;
                    //OVRManager.HMDMounted.AddListener(StartHeadsetDisplay);
                    //OVRManager.HMDUnmounted += OnHeadsetUnmounted;
                    OVRManager.VrFocusLost += OnFocusLost;
                    //OVRManager.HMDUnmounted.AddListener(StopHeadsetDisplay);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time;

    }

    private void EnableVR()
    {
        Debug.Log("HeadsetIdleScript: Enable VR");
        cameraRig.leftEyeCamera.enabled = true;
        cameraRig.rightEyeCamera.enabled = true;
        XRSettings.enabled = true;
        //cameraRig.centerEyeCamera.SetActive(true);
    }

    
    private void DisableVR()
    {
        Debug.Log("HeadsetIdleScript: Disable VR");
        cameraRig.leftEyeCamera.enabled = false;
        cameraRig.rightEyeCamera.enabled = false;
        XRSettings.enabled = false;
        //cameraRig.centerEyeCamera.SetActive(false);
    }

    private void OnFocusAcquired()
    {
        Debug.Log("HeadsetIdleScript: Focus acquired");
        Debug.Log("Elapsed Time: " + elapsedTime);
        if (elapsedTime >= 600) // 600 sec = 10 min
        {
            Debug.Log("QUIT APPLICATION");
            Application.Quit();
        }
        
        else
        {
            EnableVR();
            bHeadsetAwake = true;
        }
    }

    private void OnFocusLost()
    {
        Debug.Log("HeadsetIdleScript: Focus lost");
        DisableVR();
        bHeadsetAwake = false;
    }

}
