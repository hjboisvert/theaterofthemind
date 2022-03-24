using System.Collections;
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
		/*
		networkUtils = GetComponent<NetworkUtils>();
		broadcastIP = networkUtils.GetBroadcastIP();
		pipelineString = "rtpbin buffer-mode=0 name=rtpbin tsdemux name=demux udpsrc port=" + Port.ToString() +
			" ! application/x-rtp, media=video, clock-rate=90000, encoding-name=MP2T, payload=96 ! rtpbin.recv_rtp_sink_0 rtpbin. ! " +
			"rtpmp2tdepay ! video/mpegts ! tsparse  ! demux. demux. ! video/x-h265,alignment=au,framerate=30/1,width=4096,height=2048 ! " +
			"h265parse ! queue ! d3d11h265dec ! queue ! d3d11convert ! video/x-raw(memory:D3D11Memory), format=I420, framerate=30/1 ! " +
			"appsink name=videoSink sync=true ts-offset=-2000000000 " +
			"demux. ! queue ! aacparse ! avdec_aac ! audioconvert ! autoaudiosink ts-offset=-2000000000 " +
			//"rtpbin.send_rtcp_src_0 ! udpsink host=" + broadcastIP + " port=5005 sync=false async=false  udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 ";
			"rtpbin.send_rtcp_src_0 ! udpsink host=192.168.2.255 port=5005 sync=false async=false  udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 ";
		*/
		lastFrameCount = 0;
		badStatusCount = 0;
		base.Start();
	}


	protected override string _GetPipeline()
	{
		string APP_SINK_NAME = "videoSink";
		/*
		string pipeline = " -vvvv tsdemux name=demux udpsrc port=" + Port.ToString() +
			// VIDEO
			" ! application/x-rtp, media=video, clock-rate=90000, encoding-name=MP2T, payload=96 ! rtpjitterbuffer latency=1500 ! " +
			"rtpmp2tdepay ! video/mpegts ! tsparse  ! tsdemux ! video/x-h265,alignment=au,framerate=30/1,width=4096,height=2048 ! h265parse ! queue ! d3d11h265dec " +
			"! queue   ! d3d11convert ! video/x-raw(memory:D3D11Memory), format=I420, framerate=30/1 ! queue ! " +
			"appsink name=videoSink sync=true drop=true max-lateness=2000000000 ts-offset=-2000000000 " +
			// AUDIO
			"udpsrc port=7000 ! application/x-rtp, media=audio, clock-rate=48000, encoding-name=OPUS, payload=96 ! rtpjitterbuffer ! " +
			"rtpopusdepay ! opusdec ! audioconvert ! audioresample ! autoaudiosink sync=true drop=true max-lateness=2000000000 ts-offset=-2000000000";*/

		/*
		string pipeline = "rtpbin name=rtpbin udpsrc port=" + Port.ToString() +
			// VIDEO
			" ! application/x-rtp, media=video, clock-rate=90000, encoding-name=MP2T, payload=96, a-framerate=(string)30 ! queue ! rtpbin.recv_rtp_sink_0 rtpbin. ! " +
			"rtpmp2tdepay ! video/mpegts ! tsparse  ! tsdemux ! video/x-h265, alignment=au,framerate=30/1 ! decodebin ! " +
			"queue ! videoconvert ! video/x-raw, format=I420, framerate=30/1 ! " +
			//"video/x-raw(memory:D3D11Memory),format=I420,framerate=30/1 ! queue ! " + "appsink name=videosink drop=true " +
			"queue ! " + "appsink name=" + APP_SINK_NAME + " " +

			"udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 " + // VIDEO RTCP
			//"";
			// AUDIO
			"udpsrc port=7000 ! application/x-rtp, media=audio, clock-rate=48000, encoding-name=OPUS, payload=96 ! " +
			"rtpbin.recv_rtp_sink_1    rtpbin. ! queue ! rtpopusdepay ! opusdec ! audioconvert ! audioresample ! autoaudiosink " +
			"udpsrc port=7001 ! rtpbin.recv_rtcp_sink_1"; // AUDIO RTCP
		*/

		string pipeline = " rtpbin name=rtpbin latency=1000 udpsrc port=" + Port.ToString() +
		// VIDEO
		" ! application/x-rtp, media=video, clock-rate=90000, encoding-name=H265, payload=96, a-framerate=(string)30 ! queue2 max-size-bytes=0 ! rtpbin.recv_rtp_sink_0 rtpbin. ! " +
		"rtph265depay ! video/x-h265, alignment=au,framerate=30/1,  width=4096, height=2048  ! h265parse ! queue max-size-bytes=0 ! nvh265dec ! " +
		"queue max-size-bytes=0 ! videoconvert ! video/x-raw,framerate=30/1,width=4096,height=2048,format=I420 ! " +
		//"video/x-raw(memory:D3D11Memory),format=I420,framerate=30/1 ! queue ! " + "appsink name=videosink drop=true " +
		" queue max-size-bytes=0 ! " + " appsink name=" + APP_SINK_NAME + " " +

		"udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 " + // VIDEO RTCP
														//"";
		// AUDIO
		"udpsrc port=7000 ! application/x-rtp, media=audio, clock-rate=48000, encoding-name=OPUS, payload=96 ! queue2 ! " +
		"rtpbin.recv_rtp_sink_1    rtpbin. ! rtpopusdepay ! opusdec ! audioconvert ! audioresample ! autoaudiosink " +
		"udpsrc port=7001 ! rtpbin.recv_rtcp_sink_1"; // AUDIO RTCP

		/*
		string pipeline = "rtpbin name=rtpbin " +
			"udpsrc caps=\"application/x-rtp, media=(string)video,clock-rate=(int)90000, encoding-name=(string)H264, payload=(int)96, a-framerate=(string)30\" port=5000 " +
			"! rtpbin.recv_rtp_sink_0 " +
			"rtpbin. ! rtph264depay ! h264parse ! queue ! nvh264dec ! queue ! videoconvert ! video/x-raw, format=I420, framerate=30/1 ! appsink name=videoSink " +
			"udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 " +
			"rtpbin.send_rtcp_src_0 ! udpsink port=5005 sync=false async=false ";*/

		/*
		string pipeline = "-vvvvv rtpbin ntp-sync=true ntp-time-source=3 buffer-mode=4 latency=1000 name=rtpbin tsdemux name=demux " +
			// VIDEO
			"udpsrc port=" + Port.ToString() + " ! application/x-rtp, media=video, clock-rate=90000, encoding-name=MP2T, payload=96 ! " +
			"rtpbin.recv_rtp_sink_0 rtpbin. ! rtpmp2tdepay ! video/mpegts ! tsparse  ! demux. demux. ! video/x-h265,alignment=au,framerate=30/1,width=4096,height=2048 ! " +
			"h265parse ! queue ! d3d11h265dec ! queue ! d3d11convert ! video/x-raw(memory:D3D11Memory), format=I420, framerate=30/1 ! " +
			"appsink name=videoSink " +
			"demux. ! audio/x-opus,channels=2 ! queue ! opusparse ! opusdec max-errors=-1 tolerance=9223372036854775807 ! audioconvert ! autoaudiosink drop=true sync=false " +
			//"rtpbin.send_rtcp_src_0 ! udpsink host=" + broadcastIP + " port=5005 sync=false async=false  udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 ";
			"rtpbin.send_rtcp_src_0 ! udpsink host=192.168.2.255 port=5005 sync=false async=false  udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0  " + "";
			*/
		// AUDIO
		/*
		"udpsrc port=7000 ! application/x-rtp, media=audio, encoding-name=OPUS, clock-rate=(int)48000, payload=127 ! " + "rtpbin.recv_rtp_sink_1  rtpbin. ! " +
		"rtpopusdepay ! queue ! decodebin ! audioconvert ! queue ! autoaudiosink sync=false drop=true" + //"";
		"rtpbin.send_rtcp_src_1 ! udpsink host=192.168.2.255 port=7005 sync=false async=false  udpsrc port=7001 ! rtpbin.recv_rtcp_sink_1";
		*/


		//string pipeline = "udpsrc port=" + Port.ToString() + " ! application/x-rtp ! rtpjitterbuffer ! queue !  rtph265depay ! h265parse ! queue ! d3d11h265dec " +
		//	"! queue! d3d11convert ! video/x-raw(memory:D3D11Memory), format=I420 ! queue max-size-bytes=0 max-size-time=0 max-size-buffers=0 min-threshold-time=500000000  ! appsink name=videoSink sync=false";
		//if (Audio)
		//{
		//pipeline += " udpsrc port=7000 ! application/x-rtp,media=audio,clock-rate=48000,encoding-name=OPUS,payload=96,encoding-params=2  ! rtpbin.recv_rtp_sink_1 rtpbin. ! rtpopusdepay ! opusdec ! audioconvert ! audioresample ! autoaudiosink";
		//pipeline += "demux. ! queue ! aacparse ! avdec_aac ! audioconvert ! autoaudiosink ts-offset=-1000000000 ";
		//" autoaudiosrc ! audioconvert ! audioresample ! opusenc complexity=5 bitrate-type=vbr frame-size=5 ! rtpopuspay ! udpsink host=172.28.18.87 port=5052 sync=false"
		//}

		//pipeline += "rtpbin.send_rtcp_src_0 ! udpsink host=192.168.2.255 port=5005 sync=false async=false  udpsrc port=5001 ! rtpbin.recv_rtcp_sink_0 ";
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
			if (badStatusCount == 3)
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
