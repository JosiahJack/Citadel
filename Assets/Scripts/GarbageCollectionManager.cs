using System;
using UnityEngine;
using UnityEngine.Scripting;

// This smooths out the frame hiccups from the Garbage Collector's blocking,
// albeit incremental, garbage collections.  Increasing the default from 3 to
// 6.9ms seems to help, though still not perfect it was the most optimal I was
// able to achieve with a few test scenarios.  Might try to re-evaluate this
// later.
public class GarbageCollectionManager : MonoBehaviour {
    //public float maxCleanTimerSeconds = 600f; // 10 minutes
    //public bool relyOnlyOnIncremental = true;
    public ulong intervalNanos = 6900000; // 1ms
    //public float pingRate = 1f; // Seconds between checking system mem.
    //public double purgeThreshold = 144000000; // 144MB purge threshold.
    //private float pingFinished = 0;
    //private float finished = 0;

    void Awake() {
        //if (!relyOnlyOnIncremental) {
        //    GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        //} else {
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
            GarbageCollector.incrementalTimeSliceNanoseconds = intervalNanos;
        //}
        //finished = Time.time;
        //pingFinished = Time.time + pingRate;
    }

    // Seemed smoother to simply increase the interval time to correspond to my frame time at 144fps.
    // Leaving this stuff commented out here in case I decide to revisit this and don't feel like
    // doing a git commit history walk to find this again.
    //void Update() {
    //    if (relyOnlyOnIncremental) return;
    //    if (pingRate > Time.time) return;

    //    pingFinished = Time.time + pingRate; // Prevent checking mem too often.
    //    if (System.GC.GetTotalMemory(true) > purgeThreshold
    //        || finished < Time.time) {
    //        finished = Time.time + maxCleanTimerSeconds;
    //        System.GC.Collect(); // Collect it all!
    //    }
    //}
}
