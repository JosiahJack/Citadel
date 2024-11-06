using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandler : MonoBehaviour {
	// Check all active and existing scripts on this gameObject that can be used and use them
	// Called by MouseLookScript.cs from a Use() raycast
	public void Use (UseData ud) {
		// ButtonSwitch - used by static switches and button panels
		ButtonSwitch bs = GetComponent<ButtonSwitch>();
		if (bs != null) bs.Use(ud);

		ChargeStation cs = GetComponent<ChargeStation>();
		if (cs != null) cs.Use(ud);

		Door dr = GetComponent<Door>();
		if (dr != null) dr.Use(ud);

		HealingBed hb = GetComponent<HealingBed>();
		if (hb != null) hb.Use(ud);

		KeypadElevator ke = GetComponent<KeypadElevator>();
		if (ke != null) ke.Use(ud);

		KeypadKeycode kk = GetComponent<KeypadKeycode>();
		if (kk != null) kk.Use(ud);

		PaperLog pl = GetComponent<PaperLog>();
		if (pl != null) pl.Use(ud);

		PuzzleGridPuzzle pgp = GetComponent<PuzzleGridPuzzle>();
		if (pgp != null) pgp.Use(ud);

		PuzzleWirePuzzle pwp = GetComponent<PuzzleWirePuzzle>();
		if (pwp != null) pwp.Use(ud);

		UseableObjectUse uou = GetComponent<UseableObjectUse>();
		if (uou != null) uou.Use(ud);

		UseableAttachment ua = GetComponent<UseableAttachment>();
		if (ua != null) ua.Use(ud);

		CyberAccess ca = GetComponent<CyberAccess>();
		if (ca != null) ca.Use(ud);

		InteractablePanel ip = GetComponent<InteractablePanel>();
		if (ip != null) ip.Use(ud);
	}
}
