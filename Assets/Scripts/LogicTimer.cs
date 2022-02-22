using UnityEngine;
using System.Collections;

public class LogicTimer : MonoBehaviour {
	public float timeInterval = 0.35f;
	public float randomMin = 5f;
	public float randomMax = 10f;
	public bool useRandomTimes = false;
	public bool active = true;
	[HideInInspector] public float intervalFinished; //save
	public string target;
	public string argvalue;

	void Start() {
		if (useRandomTimes)
			intervalFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(randomMin,randomMax);
		else
			intervalFinished = PauseScript.a.relativeTime + timeInterval;
    }

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive() && active) {
			if (intervalFinished < PauseScript.a.relativeTime) {
				if (useRandomTimes) {
					intervalFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(randomMin,randomMax);
				} else {
					intervalFinished = PauseScript.a.relativeTime + timeInterval;
				}
				UseTargets();
			}
		}
	}

	public void Targetted (UseData ud) {
		active = !active;
	}

	public void UseTargets () {
		UseData ud = new UseData();
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);
	}
}