using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GstCustomTexture))]
public class CustomPipelinePlayer : MonoBehaviour {

	GstCustomTexture m_Texture;

	public Texture2D BlittedImage;

	public Material TargetMaterial;

	public string pipeline = "";

	public Rect BlitRect=new Rect(0,0,1,1);
	GstImageInfo _img;

	public long position;
	public long duration;
	bool _newFrame=false;
	// Use this for initialization
	void Start () {
		m_Texture = gameObject.GetComponent<GstCustomTexture>();
		m_Texture.Initialize ();
		m_Texture.SetPipeline (pipeline+" ! video/x-raw,format=I420 ! videoconvert ! appsink name=videoSink");
		m_Texture.Player.CreateStream ();
		m_Texture.Player.Play ();

		m_Texture.OnFrameBlitted += OnFrameBlitted;
		_img = new GstImageInfo ();
		_img.Create (3840, 2160, GstImageInfo.EPixelFormat.EPixel_R8G8B8);

		BlittedImage = new Texture2D (7680, 2160);
		BlittedImage.filterMode = FilterMode.Bilinear;
		BlittedImage.anisoLevel=0;
		BlittedImage.wrapMode=TextureWrapMode.Clamp;

		if(TargetMaterial!=null)
			TargetMaterial.mainTexture=BlittedImage;
	}
	void OnFrameBlitted(GstBaseTexture src,int index)
	{
		//m_Texture.Player.CopyFrame (_img);
		float w=m_Texture.Player.FrameSizeImage.x;
		float h=m_Texture.Player.FrameSizeImage.y;
        //m_Texture.Player.CopyFrameCropped(_img,(int)(BlitRect.x*w),(int)(BlitRect.y*h),(int)(BlitRect.width*w),(int)(BlitRect.height*h));
        m_Texture.Player.CopyFrameCropped(_img, 0, 0, 7680, 2160);
        _newFrame = true;
//		Debug.Log(String.Format("Frame Copied {0}x{1}",_img.Width,_img.Height));
	}
	public void OnGUI()
	{
	}
	void OnDestroy()
	{
		if(_img!=null)
			_img.Destory ();
//		Debug.Log ("Destorying Image");
	}
	
	// Update is called once per frame
	void Update () {
        if (_newFrame)
        {
            _img.BlitToTexture(BlittedImage);
            _newFrame = false;
        }
        /*
		position=m_Texture.Player.GetPosition ()/1000;
		duration=m_Texture.Player.GetDuration ()/1000;

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			var p = (position - 5000) ;
			if (p < 0)
				p = 0;
			m_Texture.Player.Seek (p * 1000);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			var p = (position + 5000);
			if (p >=duration)
				p = duration;
			m_Texture.Player.Seek (p * 1000);
		}
		if (Input.GetKeyDown (KeyCode.S))
			m_Texture.Stop ();

		if (Input.GetKeyDown (KeyCode.P))
			m_Texture.Play ();
            */
    }
}
