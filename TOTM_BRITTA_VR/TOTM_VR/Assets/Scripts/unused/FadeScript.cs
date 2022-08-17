using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    // public RawImage img;
    public float fadeinDelay = 2.0f;
    public float fadeoutTime = 5.0f;
    public float fadeinTime = 5.0f;
    private Texture2D tex;
    private bool bFadeout = false;
    private bool bIsFading = false;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
    	rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (!bFadeout)
            {
            	Debug.Log("Fadeout");
                TriggerFadeout();
                bFadeout = true;
            }
            else
            {
            	Debug.Log("Fadein");
                TriggerFadein();
                bFadeout = false;
            }
        }
    }

    public void TriggerFadeout() {
        Debug.Log("Fadeout");
        if (bFadeout || bIsFading) {
            Debug.Log("Already faded out");
            return;
        }
        StartCoroutine("Fadeout");
        bFadeout = true;
    }

    public void TriggerFadein() {
        Debug.Log("Fadein");
        if (!bFadeout || bIsFading) {
            Debug.Log("Already faded in");
            return;
        }
        StartCoroutine("Fadein");
        bFadeout = false;
    }

   IEnumerator Fadeout()
	{
        bIsFading = true;
        float start_t = Time.time;
        float end_t = start_t + fadeoutTime;
    	while (Time.time < end_t)
    	{
            float mult = Mathf.Clamp01(1.0f - (Time.time - start_t) / fadeoutTime);
    		rend.material.SetFloat("_Mult", (float)mult);
        	yield return null;
    	}
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
            rend.material.SetFloat("_Mult", (float)mult);
            yield return null;
        }
        bIsFading = false;
    }
}

