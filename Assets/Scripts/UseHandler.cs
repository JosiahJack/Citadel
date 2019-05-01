using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHandler : MonoBehaviour {
	// All booleans default to true, default then is to always use any and all available scripts on this gameObject
	private ButtonSwitch bs;
	public bool useButtonSwitch = true;

	private ChargeStation cs;
	public bool useChargeStation = true;

	private Door dr;
	public bool useDoor = true;

	private HealingBed hb;
	public bool useHealingBed = true;

	private KeypadElevator ke;
	public bool useKeypadElevator = true;

	private KeypadKeycode kk;
	public bool useKeypadKeycode = true;

	private PaperLog pl;
	public bool usePaperLog = true;

	private PuzzleGridPuzzle pgp;
	public bool usePuzzleGridPuzzle = true;

	private UseableObjectUse uou;
	public bool useUseableObjectUse = true;

	private UseableAttachment ua;
	public bool useUseableAttachment = true;

	// Check all active and existing scripts on this gameObject that can be used and use them
	// Called by MouseLookScript.cs from a Use() raycast
	public void Use (UseData ud) {
		// ButtonSwitch - used by static switches and button panels
		bs = GetComponent<ButtonSwitch>();
		if (useButtonSwitch && bs != null) bs.Use(ud);

		cs = GetComponent<ChargeStation>();
		if (useChargeStation && cs != null) cs.Use(ud);

		dr = GetComponent<Door>();
		if (useDoor && dr != null) dr.Use(ud);

		hb = GetComponent<HealingBed>();
		if (useHealingBed && hb != null) hb.Use(ud);

		ke = GetComponent<KeypadElevator>();
		if (useKeypadElevator && ke != null) ke.Use(ud);

		kk = GetComponent<KeypadKeycode>();
		if (useKeypadKeycode && kk != null) kk.Use(ud);

		pl = GetComponent<PaperLog>();
		if (usePaperLog && pl != null) pl.Use(ud);

		pgp = GetComponent<PuzzleGridPuzzle>();
		if (usePuzzleGridPuzzle && pgp != null) pgp.Use(ud);

		uou = GetComponent<UseableObjectUse>();
		if (useUseableObjectUse && uou != null) uou.Use(ud);

		ua = GetComponent<UseableAttachment>();
		if (useUseableAttachment && ua != null) ua.Use(ud);
	}
}
