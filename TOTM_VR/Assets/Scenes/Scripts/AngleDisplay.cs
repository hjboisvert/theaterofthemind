using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleDisplay : MonoBehaviour
{
	public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	float angle = 90;
    	float yangle = transform.eulerAngles.y;
    	txt.text = yangle.ToString();
    }
}
