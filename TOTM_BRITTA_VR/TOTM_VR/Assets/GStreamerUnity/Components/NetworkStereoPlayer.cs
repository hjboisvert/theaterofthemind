﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GstCustomTexture))]

public class NetworkStereoPlayer : BaseVideoPlayer
{

	public int Port = 5000;
	public string broadcastIP = "";
	public bool Audio = true;
	private int lastFrameCount;
	private int badStatusCount;

	//private NetworkUtils networkUtils;
	//private string pipelineString;


	protected override void Start()
	{
		lastFrameCount = 0;
		badStatusCount = 0;
		base.Start();
	}


	protected override string _GetPipeline()
	{
		string APP_SINK_NAME = "videoSink";

		string pipeline = " rtpbin name=rtpbin latency=2000 udpsrc port=" + Port.ToString() +
		// VIDEO
		" ! application/x-rtp, media=video, clock-rate=90000, encoding-name=H265, payload=96, a-framerate=(string)30, width=3200, " +
        "height=1600 ! queue2 max-size-bytes=0 max-size-buffers=0 max-size-time=2000000000 use-buffering=true ! " +
        "rtpbin.recv_rtp_sink_0 rtpbin. ! rtph265depay ! video/x-h265, alignment=au,framerate=30/1,  width=3200, height=1600  ! " +
        "h265parse ! queue max-size-bytes=0 max-size-buffers=0 ! nvh265dec ! " +
		"videoscale ! videoconvert !  video/x-raw,framerate=30/1,width=4096,height=2048,format=I420 ! " +
		" queue max-size-bytes=0 max-size-buffers=0 ! " + " appsink name=" + APP_SINK_NAME + " " +

		"udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 " + // VIDEO RTCP

        // AUDIO
		"udpsrc port=7000 ! application/x-rtp, media=audio, clock-rate=48000, encoding-name=OPUS, payload=96 ! "+
        "queue2 max-size-buffers=0 max-size-time=2000000000 use-buffering=true ! " +
		"rtpbin.recv_rtp_sink_1    rtpbin. ! rtpopusdepay ! opusdec ! audioconvert ! audioresample ! autoaudiosink " +
		"udpsrc port=7001 ! rtpbin.recv_rtcp_sink_1"; // AUDIO RTCP

		return pipeline;
	}

	public int GetHeartBeatStatus()
	{
		int bStatus = 0;
		if (base.frameCount > lastFrameCount)
		{
			bStatus = 1;
		}
		else
		{
			badStatusCount += 1;
			if (badStatusCount == 6)
			{
				badStatusCount = 0;
				RestartPipeline();
			}
		}
		lastFrameCount = base.frameCount;

		return bStatus;
	}

	public void RestartPipeline()
	{
		Debug.Log("Restarting pipeline... ");
		base.m_Texture.SetPipeline(_GetPipeline());
		base.m_Texture.Player.CreateStream();
		base.m_Texture.Play();
	}

	/*
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
	*/

}
