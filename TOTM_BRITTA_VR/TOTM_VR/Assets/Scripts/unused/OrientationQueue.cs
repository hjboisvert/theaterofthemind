using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationQueue : MonoBehaviour
{
    // GameObject eyeAnchor;
    Transform cameraTransform;

    public int queueLen = 100;
    private Queue<Vector3> queue;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<Vector3>();
        // eyeAnchor = GameObject.Find("OVRCameraRig");
        //cameraTransform = GameObject.Find("OVRCameraRig").GetComponentInChildren<Transform>();
        cameraTransform = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        if (!cameraTransform)
        {
            Debug.Log("NO TRANSFORM");
        } else
        {
            Debug.Log("FOUND TRANSFORM");
        }
    }

    public int GetLen()
    {
        return queue.Count;
    }

    public Vector3 Pop()
    {
        return queue.Dequeue();
    }

    public void Push(Vector3 vec)
    {
        if (queue.Count == queueLen)
        {
            queue.Dequeue();
        }
        queue.Enqueue(vec);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform)
        {
            Push(cameraTransform.eulerAngles);
        }
    }
}
