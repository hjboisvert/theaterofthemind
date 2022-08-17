using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomImage : MonoBehaviour
{
    public float distance = 50f;
    public float moveSpeed = 0.001f;
    public float fadeoutTime = 1.0f;
    // public float delayTime = 1.0f;
    public float resetDelay = 30.0f;
    // public Texture2D waitingRoomImage;

    Transform cameraTransform;

    private HeadsetStatusScript headsetStatus;

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
        cameraTransform = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        renderer = gameObject.GetComponentInParent<SpriteRenderer>();
        renderer.color = new Color(0f, 0f, 0f, 0f);

        // orientationQueue = gameObject.GetComponentInParent<OrientationQueue>();
        // overlay = gameObject.GetComponentInParent<OVROverlay>();
        // waitingRoomImage = Resources.Load<Texture2D>("Images/WaitingRoom");
        //RawImage img = gameObject.GetComponent<RawImage>();
        //img.texture = waitingRoomImage;
        /*
        overlay.textures[0] = waitingRoomImage;
        overlay.textures[1] = waitingRoomImage;
        overlay.colorScale.x = 0.0f;
        overlay.colorScale.y = 0.0f;
        overlay.colorScale.z = 0.0f;
        overlay.colorScale.w = 0.0f;
        */
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
            /*
            overlay.colorScale.x = c;
            overlay.colorScale.y = c;
            overlay.colorScale.z = c;
            overlay.colorScale.w = c;
            */
            // rend.material.SetFloat("_Mult", (float)mult);
            renderer.color = new Color(c, c, c, c);
            yield return null;
        }
        /*
        overlay.colorScale.x = 0.0f;
        overlay.colorScale.y = 0.0f;
        overlay.colorScale.z = 0.0f;
        overlay.colorScale.w = 0.0f;
        */
        renderer.color = new Color(0f, 0f, 0f, 0f);
        bIsFading = false;
        // transform.parent.gameObject.SetActive(false);
    }

    IEnumerator Fadein()
    {
        bIsFading = true;

        yield return new WaitForSeconds(resetDelay);
        Debug.Log("after waitforseconds");
        // transform.parent.gameObject.SetActive(true);
        renderer.color = new Color(1f, 1f, 1f, 1f);
        /*
        overlay.colorScale.x = 1.0f;
        overlay.colorScale.y = 1.0f;
        overlay.colorScale.z = 1.0f;
        overlay.colorScale.w = 1.0f;
        */

        bIsFading = false;

        // yield return null;
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
        // transform.parent.gameObject.SetActive(true);
        /*
        overlay.colorScale.x = 1.0f;
        overlay.colorScale.y = 1.0f;
        overlay.colorScale.z = 1.0f;
        overlay.colorScale.w = 1.0f;
        */
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
        /*
        if (Vector3.Distance(transform.position, targetPos) > 1)
        {
            Vector3 lerpPos = Vector3.Lerp(overlay.transform.position, targetPos, moveSpeed);
            transform.position = lerpPos;
        }
        */
    }
}
