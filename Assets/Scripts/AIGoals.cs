using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType {None,AttackAnyPlayer,AttackPlayer1,AttackPlayer2,AttackPlayer3,AttackPlayer4,Interact,Wander,Guard,Patrol};

public class AIGoals : MonoBehaviour {
	private int tempInt = 0;
	public static AIGoals a;

	void Awake() {
		a = this;
	}

	public void AddInGoal(GoalType[] gList, GoalType goalToAdd) {
		for (tempInt = 0;tempInt<gList.Length;tempInt++) {
			if (gList[tempInt] == GoalType.None) {
				gList[tempInt] = goalToAdd;
				return;
			}
		}
		// Debug.Log("WARNING: Attempting to add goal to full list!");
	}

	public bool CheckIfInGoals(GoalType[] gList, GoalType goal) {
		for (tempInt=0;tempInt<gList.Length;tempInt++) {
			if (gList[tempInt] == goal) return true;
		}
		return false;
	}
}
