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
		Const.a.UseTargets(gameObject,ud,target);
	}

	public static string Save(GameObject go) {
		LogicTimer lt = go.GetComponent<LogicTimer>();
		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(lt.intervalFinished,"intervalFinished");
		line += Utils.splitChar + Utils.FloatToString(lt.timeInterval,"timeInterval");
		line += Utils.splitChar + Utils.FloatToString(lt.randomMin,"randomMin");
		line += Utils.splitChar + Utils.FloatToString(lt.randomMax,"randomMax");
		line += Utils.splitChar + Utils.BoolToString(lt.useRandomTimes,"useRandomTimes");
		line += Utils.splitChar + Utils.BoolToString(lt.active,"active");
		line += Utils.splitChar + Utils.SaveString(lt.target,"target");
		line += Utils.splitChar + Utils.SaveString(lt.argvalue,"argvalue");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicTimer lt = go.GetComponent<LogicTimer>();
		lt.intervalFinished = Utils.LoadRelativeTimeDifferential(entries[index],"intervalFinished"); index++;
		lt.timeInterval = Utils.GetFloatFromString(entries[index],"timeInterval"); index++;
		lt.randomMin = Utils.GetFloatFromString(entries[index],"randomMin"); index++;
		lt.randomMax = Utils.GetFloatFromString(entries[index],"randomMax"); index++;
		lt.useRandomTimes = Utils.GetBoolFromString(entries[index],"useRandomTimes"); index++;
		lt.active = Utils.GetBoolFromString(entries[index],"active"); index++;
		lt.target = Utils.LoadString(entries[index],"target"); index++;
		lt.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
		return index;
	}
}
