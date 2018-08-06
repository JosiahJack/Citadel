using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessCardInventory : MonoBehaviour {
	public Door.accessCardType[] accessCardsOwned;
	private int i = 0;
	private int defIndex = 0;

	void Awake() {
		for (i=defIndex;i<accessCardsOwned.Length;i++) {
			accessCardsOwned[i] = Door.accessCardType.None;
		}
	}

	public bool HasAccessCard(Door.accessCardType card) {
		for (i=defIndex;i<accessCardsOwned.Length;i++) {
			if (accessCardsOwned[i] == card) return true;
		}
		return false;
	}

	public bool AddCardToInventory(Door.accessCardType card) {
		for (i=defIndex;i<accessCardsOwned.Length;i++) {
			if (accessCardsOwned[i] == Door.accessCardType.None) {
				accessCardsOwned[i] = card;
				return true;
			}
		}
		return false;
	}
}
