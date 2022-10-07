using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomImage : MonoBehaviour
{
    public float distance = 50f;
    public float moveSpeed = 0.001f;
    public float fadeoutTime = 1.0f;
    public float resetDelay = 10.0f;

    Transform cameraTransform;

    private HeadsetStatusScript headsetStatus;
    private NetworkStereoPlayer stereoPlayer;

    private bool bFadeout = true;
    private bool bIsFading = false;

    private SpriteRenderer renderer;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3f;

    public bool GetIsFading()
    {
        return bIsFading;
    }

    // Start is called before the first frame update
    void Start()
    {
        headsetStatus = GameObject.Find("OVRCameraRig").GetComponent<HeadsetStatusScript>();
        stereoPlayer = GameObject.Find("NetworkStereo").GetComponent<NetworkStereoPlayer>();
        cameraTransform = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        renderer = gameObject.GetComponentInParent<SpriteRenderer>();
        renderer.color = new Color(0f, 0f, 0f, 0f);
    }

    public void TriggerFadeout()
    {
        Debug.Log("Dan Dare Fadeout");
        if (bFadeout || bIsFading)
        {
            Debug.Log("Already faded out");
            return;
        }
        StartCoroutine("Fadeout");
        bFadeout = true;
    }

    IEnumerator Fadeout()
    {
        bIsFading = true;
        float start_t = Time.time;
        float end_t = start_t + fadeoutTime;
        while (Time.time < end_t)
        {
            float c = Mathf.Clamp01(1.0f - (Time.time - start_t) / fadeoutTime);

            renderer.color = new Color(c, c, c, c);
            yield return null;
        }

        renderer.color = new Color(0f, 0f, 0f, 0f);
        bIsFading = false;
    }

    IEnumerator Fadein()
    {
        bIsFading = true;

        yield return new WaitForSeconds(resetDelay);
        Debug.Log("after waitforseconds");
        if (Time.time > 600) // Only if we've been running longer than 10 mins
        {
            Debug.Log("WaitingRoomImage stereoPlayer Stop");
            stereoPlayer.Stop();
        }

        renderer.color = new Color(1f, 1f, 1f, 1f);

        bIsFading = false;
    }

    public void TriggerFadein()
    {
        Debug.Log("Dan Dare Fadein");
        if (!bFadeout || bIsFading)
        {
            Debug.Log("Already faded in");
            return;
        }
        StartCoroutine("Fadein");
        bFadeout = false;
    }

    public void Reset()
    {
        StopCoroutine("Fadein");
        StopCoroutine("Fadeout");
        bIsFading = false;
        bFadeout = false;
        renderer.color = new Color(1f, 1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!headsetStatus.bHeadsetAwake)
        {
            return;
        }

        Vector3 targetPos = cameraTransform.forward * distance;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        transform.LookAt(cameraTransform.forward);
    }
}
