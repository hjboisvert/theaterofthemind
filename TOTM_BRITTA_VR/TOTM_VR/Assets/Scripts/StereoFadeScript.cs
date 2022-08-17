using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoFadeScript: MonoBehaviour
{
    public float fadeinDelay = 2.0f;
    public float fadeoutTime = 5.0f;
    public float fadeinTime = 5.0f;
    private bool bFadeout = false;
    private bool bIsFading = false;
    private Renderer rendLeft;
    private Renderer rendRight;

    public bool GetIsFading()
    {
        return bIsFading;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject sLeft = GameObject.Find("S-Left");
        GameObject sRight = GameObject.Find("S-Right");
        rendLeft = sLeft.GetComponent<Renderer>();
        rendRight = sRight.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerFadeout()
    {
        Debug.Log("Fadeout");
        if (bFadeout || bIsFading)
        {
            Debug.Log("Already faded out");
            return;
        }
        StartCoroutine("Fadeout");
        bFadeout = true;
    }

    public void TriggerFadein()
    {
        Debug.Log("Fadein");
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
        rendLeft.material.SetFloat("_Mult", 0.0f);
        rendRight.material.SetFloat("_Mult", 0.0f);
        bIsFading = false;
        bFadeout = true;
    }

    IEnumerator Fadeout()
    {
        bIsFading = true;
        float start_t = Time.time;
        float end_t = start_t + fadeoutTime;
        while (Time.time < end_t)
        {
            float mult = Mathf.Clamp01(1.0f - (Time.time - start_t) / fadeoutTime);
            rendLeft.material.SetFloat("_Mult", (float)mult);
            rendRight.material.SetFloat("_Mult", (float)mult);
            yield return null;
        }
        // make sure it gets to zero
        rendLeft.material.SetFloat("_Mult", 0.0f);
        rendRight.material.SetFloat("_Mult", 0.0f);
        bIsFading = false;
    }

    IEnumerator Fadein()
    {
        bIsFading = true;
        float start_t = Time.time + fadeinDelay;
        float end_t = start_t + fadeinTime;
        yield return new WaitForSeconds(fadeinDelay);
        while (Time.time < end_t)
        {
            float mult = Mathf.Clamp01((Time.time - start_t) / fadeinTime);
            rendLeft.material.SetFloat("_Mult", (float)mult);
            rendRight.material.SetFloat("_Mult", (float)mult);
            yield return null;
        }
        bIsFading = false;
    }
}
