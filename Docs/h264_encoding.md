# H.264 Encoding



#### Improving Video Quality

***

In short, the limiting factor is that we need very low latency from camera to headset.



The general rule for encoding  higher quality video is to increase the bitrate.  But in this case where we need fast encoding, increasing the bitrate by any amount does not result in significant quality improvement.

In the [x264 library](https://www.videolan.org/developers/x264.html) used by Gstreamer and ffmpeg, there are **speed-presets** to configure the encoding time. 

The biggest quality improvement we've been able to get was by changing the speed-preset from ultrafast --the highest, to veryfast -- two levels slower. This is at the cost of scaling the incoming camera frames to 75%, but the result is much better.



These demos show the difference between speed-preset levels

* [original vs ultrafast](https://imgsli.com/MDc0Mg) 
* [original vs veryfast](https://imgsli.com/MDc0Mw) 



Compared with the quality difference from greatly increasing the bitrate

* [3500 vs 6000](https://imgsli.com/MDc1NQ) 



[More comparison demos in this guide](https://obsproject.com/blog/streaming-with-x264) 



Since encoding speed is the major factor, it is possible we could achieve better quality with [H265](http://x265.org/hevc-h265/), which is claimed to encode twice as efficiently.

According to the [ffmpeg FAQ](https://trac.ffmpeg.org/wiki/Encode/H.264#FAQ), using the GPU would likely not speed up the encoding, at least for x264.



[This page](http://dev.beandog.org/x264_preset_reference.html) has a list of all the specific settings changed by the speed presets 



#### Encoding Parameters

***

Explanations of the more significant individual settings.



* **Bitrate**: Determines how much information you can put into each frame of video. Higher bitrate means higher quality but with diminishing returns. See [Youtube's recomendations](https://support.google.com/youtube/answer/2853702) for live streaming.

* **8×8 Adaptive DCT Transform:** “This will *significantly* improve the visual quality at a minor speed cost. In fact this option is known to give the bests speed/quality trade-off of all options. Requires a “High Profile” capable H.264 decoder. It's highly recommended to keep this option enabled, if possible! “
  * enabled for all speed-presets other than ultrafast

* **CABAC:** This setting enables CABAC entropy encoding. results in higher compression. “CABAC requires additional CPU time for both encoding and decoding! The extra CPU time required for CABAC highly depends on the bitrate. Note that CABAC can easily become the most compute-intensive part of H.264 decoding!” However not recommended to turn off.
  * Enabled for all speed-presets other than ultrafast



More at  [H264 Parameter Guide](https://www.avidemux.org/admWiki/doku.php?id=tutorial:h.264) 

#### Links

[Streaming with x264](https://obsproject.com/blog/streaming-with-x264)

[FFmpeg H264 wiki](https://trac.ffmpeg.org/wiki/Encode/H.264) 

[Youtube live encoder settings](https://support.google.com/youtube/answer/2853702)

[Preset comparison](http://dev.beandog.org/x264_preset_reference.html) 

[x264 fullhelp](http://x264.janhum.alfahosting.org/fullhelp.txt) (verbose help info from command line version)

[x264](https://www.videolan.org/developers/x264.html) 

[x265](http://x265.org/hevc-h265/) 





