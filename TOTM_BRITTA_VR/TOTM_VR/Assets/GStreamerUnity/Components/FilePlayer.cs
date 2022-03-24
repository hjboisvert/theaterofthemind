using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GstCustomTexture))]
public class FilePlayer : BaseVideoPlayer
{
	public string filePath = "totm.mp4";

	protected override string _GetPipeline()
	{
		filePath = (Application.dataPath + "/" + filePath).Replace('\\', '/');
		string pipeline = "filesrc location=\"" + filePath + "\" ! decodebin ! videoconvert ! video/x-raw, format=I420, framerate=30/1 ! appsink name=videoSink ";

		return pipeline;
	}

}