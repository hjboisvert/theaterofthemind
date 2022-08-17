using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Recenter"))
        {
            Recenter();
        }
    }

    public void Recenter()
    {
        Debug.Log("RecenterScript recentering");
        OVRManager.display.RecenterPose();
    }
}
