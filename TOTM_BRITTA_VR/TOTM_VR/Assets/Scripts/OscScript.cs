using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OscScript : MonoBehaviour
{
    public int heartbeatInterval = 1; // seconds
    public string broadcasterIpAddress = "192.168.2.255";
    private OscServer server;
    private OscClient client;
    private DateTime lastHeartbeatTime;
    private int oscId;
    private VRRecenter sphereLeft;
    private VRRecenter sphereRight;
    private FadeScript faderLeft;
    private FadeScript faderRight;
    private PostProcess.BlinkEffect blinkLeft;
    private PostProcess.BlinkEffect blinkRight;
    private NetworkStereoPlayer networkPlayer;
    private bool bRecvRecenter = false;
    private bool bRecvFadeout = false;
    private bool bRecvFadein = false;
    private bool bRecvBlinkOpen = false;
    private bool bRecvBlinkClosed = false;

    // Start is called before the first frame update
    void Start()
    {
        string user = Environment.UserName;
        oscId = int.Parse(user.Substring(2));
        Debug.Log("UserName: " + oscId);
    	sphereLeft = GameObject.Find("S-Left").GetComponent<VRRecenter>();
   		sphereRight = GameObject.Find("S-Right").GetComponent<VRRecenter>();

        faderLeft = GameObject.Find("S-Left").GetComponent<FadeScript>();
        faderRight = GameObject.Find("S-Right").GetComponent<FadeScript>();

        blinkLeft = GameObject.Find("LeftEyeAnchor").GetComponent<PostProcess.BlinkEffect>();
        blinkRight = GameObject.Find("RightEyeAnchor").GetComponent<PostProcess.BlinkEffect>();

        networkPlayer = GameObject.Find("NetworkStereo").GetComponent<NetworkStereoPlayer>();

        Debug.Log("Starting OSC Server on port 8000");
    	server = new OscServer(8000);
    	// server.MessageDispatcher.AddCallback("/cmd", CmdMsgCallback);
        server.MessageDispatcher.AddCallback("/cmd", this.CmdMsgCallback);

        Debug.Log("Sending heartbeats to " + broadcasterIpAddress);
        client = new OscClient(broadcasterIpAddress, 9000);

        lastHeartbeatTime = DateTime.Now;
    }

    void CmdMsgCallback(string address, OscDataHandle data) {
    	Debug.Log("Got OSC: " + address);
   		// Debug.Log(string.Format("({0}, {1})", data.GetElementAsFloat(0), data.GetElementAsFloat(1)));
    	string cmd = data.GetElementAsString(0);
        switch (cmd)
        {
            case "recenter":
                bRecvRecenter = true;
                break;
            case "fadeout":
                bRecvFadeout = true;
                break;
            case "fadein":
                bRecvFadein = true;
                break;
            case "blinkclosed":
                bRecvBlinkClosed = true;
                break;
            case "blinkopen":
                bRecvBlinkOpen = true;
                break;
            default:
                Debug.Log("got unknown command: " + cmd);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bRecvRecenter)
        {
            Debug.Log("got RECENTER command");
            sphereLeft.Recenter();
            sphereRight.Recenter();
            bRecvRecenter = false;
        }
        if (bRecvFadeout)
        {
            Debug.Log("got FADEOUT command");
            faderLeft.TriggerFadeout();
            faderRight.TriggerFadeout();
            bRecvFadeout = false;
        }
        if (bRecvFadein)
        {
            Debug.Log("got FADEIN command");
            faderLeft.TriggerFadein();
            faderRight.TriggerFadein();
            bRecvFadein = false;
        }
        if (bRecvBlinkClosed)
        {
            Debug.Log("got BLINK CLOSED command");
            blinkLeft.BlinkClosed();
            blinkRight.BlinkClosed();
            bRecvBlinkClosed = false;
        }
        if (bRecvBlinkOpen)
        {
            Debug.Log("got BLINK OPEN command");
            blinkLeft.BlinkOpen();
            blinkRight.BlinkOpen();
            bRecvBlinkOpen = false;
        }

        TimeSpan interval = DateTime.Now - lastHeartbeatTime;
        if (client == null || interval.Seconds < heartbeatInterval)
        {
            return;
        }
        sendHeartbeat();
        lastHeartbeatTime = DateTime.Now;
    }

    void sendHeartbeat()
    {
        // Debug.Log("Sending heartbeat");
        int status = networkPlayer.GetHeartBeatStatus();
        client.Send("/heartbeat", oscId, status);
    }

    void OnDestroy() {
    	server.Dispose();
        client.Dispose();
    }
}
