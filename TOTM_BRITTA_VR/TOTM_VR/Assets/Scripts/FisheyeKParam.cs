using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisheyeKParam : MonoBehaviour
{
    private float m_camHeight = 0.0f;
    public float camHeight = 128.0f;

    private float m_k_angle = 0.0f;
    public float k_angle = 100.0f;

    private float m_maskWidth = 0.0f;
    public float maskWidth = 0.59f;

    private float m_maskHeight = 0.0f;
    public float maskHeight = 0.7f;

    private float m_maskTransition = 0.0f;
    public float maskTransition = 0.2f;

    public delegate void OnVariableChangeDelegate(float newVal);
    public event OnVariableChangeDelegate OnCamHeightChange;
    public event OnVariableChangeDelegate OnKAngleChange;
    public event OnVariableChangeDelegate OnMaskWidthChange;
    public event OnVariableChangeDelegate OnMaskHeightChange;
    public event OnVariableChangeDelegate OnMaskTransitionChange;


    private GameObject sLeft;
    private GameObject sRight;

    // Start is called before the first frame update
    void Start()
    {
        sLeft = GameObject.Find("S-Left");
        sRight = GameObject.Find("S-Right");
        OnCamHeightChange += CamHeightChangeHandler;
        OnKAngleChange += KAngleChangeHandler;
        OnMaskWidthChange += MaskWidthChangeHandler;
        OnMaskHeightChange += MaskHeightChangeHandler;
        OnMaskTransitionChange += MaskTransitionChangeHandler;
        camHeight = 128.0f;
        k_angle = 100.0f;
        maskWidth = 0.59f;
        maskHeight = 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camHeight != camHeight && OnCamHeightChange != null)
        {
            m_camHeight = camHeight;
            OnCamHeightChange(m_camHeight);
        }
        if (m_k_angle != k_angle && OnKAngleChange != null)
        {
            m_k_angle = k_angle;
            OnKAngleChange(m_k_angle);
        }
        if (m_maskWidth != maskWidth && OnMaskWidthChange != null)
        {
            m_maskWidth = maskWidth;
            OnMaskWidthChange(m_maskWidth);
        }
        if (m_maskHeight != maskHeight && OnMaskHeightChange != null)
        {
            m_maskHeight = maskHeight;
            OnMaskHeightChange(m_maskHeight);
        }
        if (m_maskTransition != maskTransition && OnMaskTransitionChange != null)
        {
            m_maskTransition = maskTransition;
            OnMaskTransitionChange(m_maskTransition);
        }
    }

    private void CamHeightChangeHandler(float newVal)
    {
        Debug.Log("camHeight change");
        //Shader.SetGlobalFloat("_K_ANGLE", this.m_k_angle);
        sLeft.transform.position = new Vector3(0, this.camHeight, 0);
        sRight.transform.position = new Vector3(0, this.camHeight, 0);
    }

    private void KAngleChangeHandler(float newVal)
    {
        Debug.Log("k_angle change");
        Shader.SetGlobalFloat("_K_ANGLE", this.m_k_angle);
    }

    private void MaskWidthChangeHandler(float newVal)
    {
        Debug.Log("maskWidth change");
        Shader.SetGlobalFloat("_MaskWidth", this.m_maskWidth);
    }

    private void MaskHeightChangeHandler(float newVal)
    {
        Debug.Log("maskHeight change");
        Shader.SetGlobalFloat("_MaskHeight", this.m_maskHeight);
    }

    private void MaskTransitionChangeHandler(float newVal)
    {
        Debug.Log("maskTransition change");
        Shader.SetGlobalFloat("_MaskTransition", this.m_maskHeight);
    }
}
