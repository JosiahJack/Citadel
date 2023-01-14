using UnityEngine;

public class LogicTimer : MonoBehaviour {
	public float timeInterval = 0.35f; //save
	public float randomMin = 5f; //save
	public float randomMax = 10f; //save
	public bool useRandomTimes = false; //save
	public bool active = true; //save
	[HideInInspector] public float intervalFinished; //save
	public string target; //save
	public string argvalue; //save

	void Start() {
		intervalFinished = PauseScript.a.relativeTime + (useRandomTimes ? Random.Range(randomMin,randomMax) : timeInterval);
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive() && active) {
			if (intervalFinished < PauseScript.a.relativeTime) {
				if (useRandomTimes) {
					intervalFinished = PauseScript.a.relativeTime
									   + Random.Range(randomMin,randomMax);
				} else {
					intervalFinished = PauseScript.a.relativeTime
									   + timeInterval;
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
			Debug.Log("BUG: no TargetIO.cs found on an object with a "
					  + "ButtonSwitch.cs script!  Trying to call UseTargets "
					  + "without parameters!");
		}
		Const.a.UseTargets(ud,target);
	}

	public static string Save(GameObject go) {
		LogicTimer lt = go.GetComponent<LogicTimer>();
		if (lt == null) {
			Debug.Log("LogicTimer missing on savetype of LTimer!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("ffffbbss");
		}

		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(lt.intervalFinished);
		line += Utils.splitChar + Utils.FloatToString(lt.timeInterval);
		line += Utils.splitChar + Utils.FloatToString(lt.randomMin);
		line += Utils.splitChar + Utils.FloatToString(lt.randomMax);
		line += Utils.splitChar + Utils.BoolToString(lt.useRandomTimes);
		line += Utils.splitChar + Utils.BoolToString(lt.active);
		line += Utils.splitChar + lt.target;
		line += Utils.splitChar + lt.argvalue;
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicTimer lt = go.GetComponent<LogicTimer>();
		if (lt == null) {
			Debug.Log("LogicTimer.Load failure, lt == null");
			return index + 8;
		}

		if (index < 0) {
			Debug.Log("LogicTimer.Load failure, index < 0");
			return index + 8;
		}

		if (entries == null) {
			Debug.Log("LogicTimer.Load failure, entries == null");
			return index + 8;
		}

		lt.intervalFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		lt.timeInterval = Utils.GetFloatFromString(entries[index]); index++;
		lt.randomMin = Utils.GetFloatFromString(entries[index]); index++;
		lt.randomMax = Utils.GetFloatFromString(entries[index]); index++;
		lt.useRandomTimes = Utils.GetBoolFromString(entries[index]); index++;
		lt.active = Utils.GetBoolFromString(entries[index]); index++;
		lt.target = entries[index]; index++;
		lt.argvalue = entries[index]; index++;
		return index;
	}
}