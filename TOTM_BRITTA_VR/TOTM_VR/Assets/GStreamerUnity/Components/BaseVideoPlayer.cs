using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public abstract class BaseVideoPlayer : DependencyRoot {

	private static Mutex mutex;
	protected GstCustomTexture m_Texture = null;
	//Material material;
	OffscreenProcessor _Processor;

	public Shader[] PostProcessors;

	OffscreenProcessor[] _postProcessors;

	//public RenderTexture VideoTexture1;
	//public RenderTexture VideoTexture2;
	public int textureIndex = 0; // 0 for left 1 for right
	public int xOffset = 0;
	public int frameWidth = 4096; // TODO: Actual camera frames are 2880x2880. CamLink converts to 3840x2160. This assumes it scales w and h equally
    //public CustomRenderTexture VideoTexture;
    private Texture frame;
	public bool ConvertToRGB=true;
    public int frameCount;
    public float fps = 0;

	private Texture leftBackFrame;
	private Texture leftFrontFrame;

	private Texture rightBackFrame;
	private Texture rightFrontFrame;

	public Material leftMaterial;
	public Material rightMaterial;

	public GstCustomTexture InternalTexture
	{
		get{return m_Texture;}
	}

	public delegate void Delg_OnFrameAvailable(BaseVideoPlayer src,Texture tex);
	public event Delg_OnFrameAvailable OnFrameAvailable;

	protected abstract string _GetPipeline ();

	// Use this for initialization
	protected override void Start () {
		mutex = new Mutex();
		leftFrontFrame = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
		leftFrontFrame.filterMode = FilterMode.Bilinear;
		leftFrontFrame.anisoLevel = 0;
		leftFrontFrame.wrapMode = TextureWrapMode.Clamp;

		leftBackFrame = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
		leftBackFrame.filterMode = FilterMode.Bilinear;
		leftBackFrame.anisoLevel = 0;
		leftBackFrame.wrapMode = TextureWrapMode.Clamp;

		rightFrontFrame = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
		rightFrontFrame.filterMode = FilterMode.Bilinear;
		rightFrontFrame.anisoLevel = 0;
		rightFrontFrame.wrapMode = TextureWrapMode.Clamp;

		rightBackFrame = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
		rightBackFrame.filterMode = FilterMode.Bilinear;
		rightBackFrame.anisoLevel = 0;
		rightBackFrame.wrapMode = TextureWrapMode.Clamp;

		_Processor =new OffscreenProcessor();
		m_Texture = gameObject.GetComponent<GstCustomTexture>();

		//material=gameObject.GetComponent<MeshRenderer> ().material;
		// Check to make sure we have an instance.
		if (m_Texture == null)
		{
			DestroyImmediate(this);
		}

		m_Texture.Initialize ();
		//		pipeline = "filesrc location=\""+VideoPath+"\" ! decodebin ! videoconvert ! video/x-raw,format=I420 ! appsink name=videoSink sync=true";
		//		pipeline = "filesrc location=~/Documents/Projects/BeyondAR/Equirectangular_projection_SW.jpg ! jpegdec ! videoconvert ! imagefreeze ! videoconvert ! imagefreeze ! videoconvert ! video/x-raw,format=I420 ! appsink name=videoSink sync=true";
		//		pipeline = "videotestsrc ! videoconvert ! video/x-raw,width=3280,height=2048,format=I420 ! appsink name=videoSink sync=true";
		Debug.Log("Setting pipeline: " + _GetPipeline());
		m_Texture.SetPipeline (_GetPipeline());
		m_Texture.Player.CreateStream();
		m_Texture.Play();

		m_Texture.OnFrameGrabbed += OnFrameGrabbed;

		_Processor.ShaderName="Image/I420ToRGB";

		if (PostProcessors != null) {
			_postProcessors = new OffscreenProcessor[PostProcessors.Length];
			for (int i = 0; i < PostProcessors.Length; ++i) {
				_postProcessors [i] = new OffscreenProcessor ();
				_postProcessors [i].ProcessingShader = PostProcessors [i];
			}
		}

		Debug.Log ("Starting Base");
		base.Start ();
	}

	bool _newFrame=false;
	void OnFrameGrabbed(GstBaseTexture src,int index)
	{
		_newFrame = true;
	}

	void _processNewFrame()
	{
		_newFrame = false;
		if (m_Texture.PlayerTexture ().Length == 0)
			return;

		Texture tex=m_Texture.PlayerTexture () [0];

		if (ConvertToRGB) {
			if (m_Texture.PlayerTexture () [0].format == TextureFormat.Alpha8)
				frame = _Processor.ProcessTexture(tex);
			else
				frame = tex;
			
		} else {
			frame = tex;
		}

		if (_postProcessors != null) {
			foreach (var p in _postProcessors) {
				//frame = p.ProcessTexture (VideoTexture);
			}
		}
        //List<CustomRenderTextureUpdateZone> zones = new List<CustomRenderTextureUpdateZone>();
        //VideoTexture.GetUpdateZones(zones);
        //Graphics.Blit(frame, (RenderTexture)zones[0]);
        //Graphics.Blit(frame, VideoTexture);
        int srcX = 0;
        int srcY = 0;
        int srcWidth = frameWidth/2;
        int srcHeight = frameWidth/2;
        int dstX = 0;
        int dstY = 0;
		// int offset = 2880 / 2 - srcWidth / 2; //actual camera frame centered in black. Not needed anymore since we're cropping on sender side

		mutex.WaitOne();
		Graphics.CopyTexture(frame, 0, 0, 0, 0, srcWidth, srcHeight, leftBackFrame,
		    0, 0, dstX, dstY);

		Graphics.CopyTexture(frame, 0, 0, srcWidth, 0, srcHeight, srcHeight, rightBackFrame,
			0, 0, dstX, dstY);
		mutex.ReleaseMutex();

		//this doesn't work
		//Graphics.Blit(frame, VideoTexture, new Vector2(1.0f, 1.0f), new Vector2(offset, 0));

		//if (OnFrameAvailable != null)
			//OnFrameAvailable (this, VideoTexture);

	}

	void OnGui()
    {

	}

	// Update is called once per frame
	void Update() {

		//if (m_Texture.Player.IsPlaying && (int)OVRManager.display.displayFrequency != 60)
		//{
		//OVRManager.display.displayFrequency = 60.0f;
		//}

		if (leftFrontFrame != leftBackFrame && rightFrontFrame != rightBackFrame)
		{
			mutex.WaitOne();
			leftFrontFrame = leftBackFrame;
			rightFrontFrame = rightBackFrame;
			leftMaterial.mainTexture = leftFrontFrame;
			rightMaterial.mainTexture = rightFrontFrame;
			mutex.ReleaseMutex();
		}


		if (_newFrame) { 
			//Debug.Log("NEW FRAME");
			frameCount++;
			fps = frameCount / Time.time;
			_processNewFrame();
		}
	}

	/*
	protected override void OnDestroy()
	{
		InternalTexture.Destroy();
		base.OnDestroy();
	}
	*/
}
