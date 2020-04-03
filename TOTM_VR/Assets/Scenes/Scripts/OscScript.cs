using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class OscScript : MonoBehaviour
{
	private OscServer server;
	private VRRecenter sphereLeft;
	private VRRecenter sphereRight;
    private bool bRecvRecenter = false;

    // Start is called before the first frame update
    void Start()
    {
    	sphereLeft = GameObject.Find("S-Left").GetComponent<VRRecenter>();
   		sphereRight = GameObject.Find("S-Right").GetComponent<VRRecenter>();

    	Debug.Log("Starting OSC Server on port 9000");
    	server = new OscServer(9000);
    	// server.MessageDispatcher.AddCallback("/cmd", CmdMsgCallback);
        server.MessageDispatcher.AddCallback("/cmd", this.CmdMsgCallback);    
    }

    void CmdMsgCallback(string address, OscDataHandle data) {
    	Debug.Log("Got OSC: " + address);
   		// Debug.Log(string.Format("({0}, {1})", data.GetElementAsFloat(0), data.GetElementAsFloat(1)));
    	string cmd = data.GetElementAsString(0);
    	switch (cmd) {
    		case "recenter":
                bRecvRecenter = true;
    			break;
    		default:
    			Debug.Log("got unknown command: " + cmd);
    			break;
    	}
    }

    // Update is called once per frame
    void Update()
    {
        if (bRecvRecenter) {
            Debug.Log("got RECENTER command");
            sphereLeft.Recenter();
            sphereRight.Recenter();
            bRecvRecenter = false;
        }
        
    }

    void OnDestroy() {
    	server.Dispose();
    }
}
