using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class realtime : MonoBehaviour
{
    private AudioSource source;
    private AudioClip mic;
    private int lastSample = 0;
    private float[] samples = null;
    private List<float> readSamples = null;
    private int channels = 0;
    private int readUpdateId = 0;
    private int previousReadUpdateId = -1;
    public float READ_FLUSH_TIME = 0.5f;
    private float readFlushTimer = 0.0f;
    void Start()
    {
        readSamples = new List<float>();
        source = new GameObject("Mic").AddComponent<AudioSource>();
        mic = Microphone.Start("Microphone (High Definition Audio Device)", true, 100, 44100);
        channels = mic.channels;
    }
    void Update()
    {
        ReadMic();
        PlayMic();
    }
    private void ReadMic()
    {
	    int pos = Microphone.GetPosition(null);
		int diff = pos - lastSample;
		if (diff > 0)
		{
			samples = new float[diff * channels];
            print(samples);
		    mic.GetData(samples, lastSample);
            readSamples.AddRange(samples);
        }
		lastSample = pos;
    }
    private void PlayMic()
    {
        readFlushTimer += Time.deltaTime;

		if (readFlushTimer > READ_FLUSH_TIME)
		{
			if (readUpdateId != previousReadUpdateId && readSamples != null && readSamples.Count > 0)
			{
		    	previousReadUpdateId = readUpdateId;
                source.clip = AudioClip.Create("Real_time", readSamples.Count, channels, 44100, false);
                source.spatialBlend = 0;
                source.clip.SetData(readSamples.ToArray(), 0);
                if (!source.isPlaying){
                    source.Play();
                }

                readSamples.Clear();
                readUpdateId++;
			}

			readFlushTimer = 0.0f;
		}
    }
}
