using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomonitorGraphs : MonoBehaviour {
	public PlayerMovement pm;
	public float beatThresh = 0.1f;
	public float beatVariation = 0.05f;
	public float freq = 35f;
	public float flip = -1f;
	public float fatigueFactor = 0f;
	public float beatFinished = 0f;
	public float heartRateOffset = -0.5f;

    void Awake() {
		beatFinished = PauseScript.a.relativeTime + 1.2f;
    }

	void OnEnable() {
		// fatigueFactor = (((pm.fatigue / 100f) * 150f) + 20f) / 60f; // Apply percent fatigued to 200bpm max heart rate with baseline 50bpm.
		// beatFinished = PauseScript.a.relativeTime + (1f/fatigueFactor); // Adjust into seconds and add to game timer.
		if (BiomonitorGraphSystem.a != null) BiomonitorGraphSystem.a.ClearGraphs();
	}

    // void Update() {
		// fatigueFactor = (((pm.fatigue / 100f) * 150f) + baseBPM) / 60f; // Apply percent fatigued to 200bpm max heart rate with baseline 50bpm.
		// if (beatFinished < PauseScript.a.relativeTime) {
			// beatFinished = PauseScript.a.relativeTime + (1f/fatigueFactor); // Adjust into seconds and add to game timer.
		// }

        // BiomonitorGraphSystem.a.Graph("EnergyValue",((pe.drainJPM/449f)*2f)-1f); // Take percentage of max JPM drain per second (449) and apply it to a scale of ±1.0

		// Chi wave is different when on genius patch
		// if (pp.geniusFinishedTime > PauseScript.a.relativeTime)
			// BiomonitorGraphSystem.a.Graph("ChiValue", Mathf.Sin(PauseScript.a.relativeTime * 3f) + UnityEngine.Random.Range(-0.3f,0.3f));
		// else
			// BiomonitorGraphSystem.a.Graph("ChiValue", Mathf.Sin(PauseScript.a.relativeTime * 1f));

		// beatShift = (beatFinished - PauseScript.a.relativeTime)/(1f/fatigueFactor);
		// if (beatShift > 0.8f) {
			// ecgValue = Mathf.Sin(beatShift * freq);
		// } else ecgValue = 0;
		// if (ecgValue > beatThresh || ecgValue < (beatThresh * -1f)) ecgValue += UnityEngine.Random.Range((beatVariation * -1f),beatVariation); // Inject variation when beating
        // BiomonitorGraphSystem.a.Graph("HeartbeatValue", ecgValue);
    // }
}

// Old method

	// public float a = 1.56f; // Centering up/down
	// public float b = 0f; // R wave peak amplitude
	// public float c = 63f; // Needed for consistent waveshape

		// Heartbeat Electrocardiograph Wave (ECG / EKG)
		// a = 1.56f;
		// fatigueFactor = Mathf.Floor(((pm.fatigue/100f)*5f) + 1); // for range percentage
		// if (fatigueFactor < 1f) fatigueFactor = 1f;
		// if (fatigueFactor >= 2f) a = 0.79f;
		// if (fatigueFactor >= 3f) a = 1.18f;
		// if (fatigueFactor >= 4f) a = 0.32f;
		// if (fatigueFactor >= 6f) a = 0.26f; // Adjusting the centering value to account for the skewness that increasing the frequency causes with its alignment to the sine waves.
		// ecgValue = (Mathf.Pow(Mathf.Sin(PauseScript.a.relativeTime * freq * fatigueFactor),c) * Mathf.Sin((PauseScript.a.relativeTime + a) * freq * fatigueFactor)*b);