using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    // public RawImage img;
    public float fadeoutTime = 5.0f;
    public float fadeinTime = 5.0f;
    private Texture2D tex;
    private bool bFadeout = false;

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
                StartCoroutine("Fadeout");
                bFadeout = true;
            }
            else
            {
            	Debug.Log("Fadein");
                StartCoroutine("Fadein");
                bFadeout = false;
            }
        }
    }

    public void TriggerFadeout() {
        Debug.Log("Fadeout");
        StartCoroutine("Fadeout");
        bFadeout = true;
    }

    public void TriggerFadein() {
        Debug.Log("Fadein");
        StartCoroutine("Fadein");
        bFadeout = false;
    }

   IEnumerator Fadeout() 
	{
        float start_t = Time.time;
        float end_t = start_t + fadeoutTime;
    	while (Time.time < end_t) 
    	{
            double mult = 1.0 - (Time.time - start_t) / fadeoutTime;
    		rend.material.SetFloat("_Mult", (float)mult);
        	yield return null;
    	}
	}

	IEnumerator Fadein() 
	{
        float start_t = Time.time;
        float end_t = start_t + fadeinTime;
        while (Time.time < end_t)
        {
            double mult = (Time.time - start_t) / fadeinTime;
            rend.material.SetFloat("_Mult", (float)mult);
            yield return null;
        }
    }
}

