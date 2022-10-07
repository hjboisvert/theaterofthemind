using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadsetStatusScript: MonoBehaviour
{
    private OVRManager vrManager;
    private XRDisplaySubsystem headsetDisplay;

    public bool bHeadsetAwake = false;
    private float elapsedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        vrManager = (OVRManager)FindObjectOfType(typeof(OVRManager));

        OVRManager.SetSpaceWarp(false);

        /* Possible events to listen for: HMDMounted, HMDUnmounted, VrFocusAcquired, VrFocusLost */
        OVRManager.VrFocusAcquired += OnFocusAcquired;
        OVRManager.VrFocusLost += OnFocusLost;

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time;

    }

    private IEnumerator QuitApp()
    {
        Debug.Log("DELAY BEFORE QUIT");
        yield return new WaitForSeconds(2);
        Debug.Log("QUIT APPLICATION");
        Application.Quit();
    }

    private void OnFocusAcquired()
    {
        Debug.Log("HeadsetStatusScript: Focus acquired");
        Debug.Log("Elapsed Time: " + elapsedTime);
        if (elapsedTime >= 600) // 600 sec = 10 min
        {
            // QuitApp();
            Debug.Log("QUIT APPLICATION");
            Application.Quit();
        }
        
        else
        {
            Debug.Log("HeadsetStatusScript: bHeadsetAwake = true");
            bHeadsetAwake = true;
        }
    }

    private void OnFocusLost()
    {
        Debug.Log("HeadsetStatusScript: Focus lost");
        bHeadsetAwake = false;
    }

}
