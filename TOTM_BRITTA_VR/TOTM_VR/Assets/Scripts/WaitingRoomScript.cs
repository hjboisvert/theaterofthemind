using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomScript : MonoBehaviour
{
    //private WaitingRoomSprite waitingRoomSprite;
    private WaitingRoomImage waitingRoomImage;
    private StereoFadeScript stereoFader;
    private HeadsetStatusScript headsetStatus;
    private bool bInWaitingRoom = false;
    private bool bTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        // waitingRoomImage = GameObject.Find("WaitingRoomPoster").GetComponent<WaitingRoomImage>();
        //waitingRoomSprite = GameObject.Find("WaitingRoomSprite");
        waitingRoomImage = GameObject.Find("WaitingRoomSprite").GetComponentInChildren<WaitingRoomImage>();
        //stereoFader = GameObject.Find("StereoFader").GetComponent<StereoFadeScript>();
        stereoFader = GameObject.Find("StereoFader").GetComponent<StereoFadeScript>();

        headsetStatus = GameObject.Find("OVRCameraRig").GetComponent<HeadsetStatusScript>();
    }

    public void TransitionToWaiting()
    {
        
        if (!headsetStatus.bHeadsetAwake)
        {
            //return;
        }

        if (!bInWaitingRoom && !stereoFader.GetIsFading() && !waitingRoomImage.GetIsFading())
        {
            //waitingRoomSprite.setActive(true);
            waitingRoomImage.TriggerFadein();
            stereoFader.TriggerFadeout();
            bInWaitingRoom = true;
        }

    }

    public void TransitionToBritta()
    {
        
        if (!headsetStatus.bHeadsetAwake)
        {
            //return;
        }

        if (bInWaitingRoom && !stereoFader.GetIsFading() && !waitingRoomImage.GetIsFading())
        {
            waitingRoomImage.TriggerFadeout();
            stereoFader.TriggerFadein();
            bInWaitingRoom = false;
        }

    }

    public void Reset()
    {
        waitingRoomImage.Reset();
        stereoFader.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (!bInWaitingRoom)
            {
                Debug.Log("ToWaiting");
                TransitionToWaiting();
            }
            else
            {
                Debug.Log("ToBritta");
                TransitionToBritta();
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            // for debug: reset fades without delay or tweens
            Reset();
        }

    }
}