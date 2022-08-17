using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeHeadset : MonoBehaviour
{
    private OVRManager manager;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting WakeHeadset script...");
        Debug.Log("CPU Level: " + OVRManager.cpuLevel);
        manager = (OVRManager)FindObjectOfType(typeof(OVRManager));

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!manager.isUserPresent)
        {
            // Debug.Log("NOT PRESENT");
        }
        */
        /*
        if (OVRManager.cpuLevel < 2)
        {
            Debug.Log("SETTING CPU LEVEL");
            OVRManager.cpuLevel = 2;
        }
        */
    }
}
