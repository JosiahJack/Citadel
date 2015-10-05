using UnityEngine;
using System.Collections;

public class DataReaderTextLoader : MonoBehaviour {

	public const string path = "AudioLogs";

	// Use this for initialization
	void Start () 
	{
		Debug.Log ("Script started");
		DataReaderTextContainer drtc = DataReaderTextContainer.Load (path);

		foreach (Log audioLog in drtc.audioLogs)
		{
			print(audioLog.name);
			print (audioLog.LogContent);
			Debug.Log ("Okay...");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
