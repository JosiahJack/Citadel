using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour {
    public enum UpdateMode { BucketA, BucketB, Always }
    public static UpdateManager Instance { get; private set; }
    private readonly HashSet<IBatchUpdate> _slicedUpdateBehavioursBucketA = new HashSet<IBatchUpdate>();
    private readonly HashSet<IBatchUpdate> _slicedUpdateBehavioursBucketB = new HashSet<IBatchUpdate>();
    private bool _isCurrentBucketA;
	private int i;

    public void RegisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour, UpdateMode updateMode) {
        if (updateMode == UpdateMode.Always) {
            _slicedUpdateBehavioursBucketA.Add(slicedUpdateBehaviour);
            _slicedUpdateBehavioursBucketB.Add(slicedUpdateBehaviour);
        } else {
            var targetUpdateFunctions = updateMode == UpdateMode.BucketA ? _slicedUpdateBehavioursBucketA : _slicedUpdateBehavioursBucketB;
            targetUpdateFunctions.Add(slicedUpdateBehaviour);
        }
    }
    
    public void DeregisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour) {
        _slicedUpdateBehavioursBucketA.Remove(slicedUpdateBehaviour);
        _slicedUpdateBehavioursBucketB.Remove(slicedUpdateBehaviour);
    }
    
    void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Update() {
        var targetUpdateFunctions = _isCurrentBucketA ? _slicedUpdateBehavioursBucketA : _slicedUpdateBehavioursBucketB;
        foreach (var slicedUpdateBehaviour in targetUpdateFunctions) {
            slicedUpdateBehaviour.BatchUpdate();
        }
        _isCurrentBucketA = !_isCurrentBucketA;
    }
}

public interface IBatchUpdate {
    void BatchUpdate();
}

// Buckets:
// Always:
	// AIAnimationController
// A:
	// Door
	// Force Bridge
	// HealthManager
	// ColorCurvesManager
// B:
	// ButtonSwitch
	// LightAnimation
	// TickIndicatorAnimation
	// SecurityCameraRotate
	// SearchableItem


// Usage:

	//void Start() {
	//	UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.BucketA); // every other frame
	//  OR
	//	UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.BucketB); // every 2nd other frame (opposite of the above)
	//  OR
	//	UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.Always); // avoids overhead of a separate normal Update() but still every frame
    //}

    //private void OnDestroy() { UpdateManager.Instance.DeregisterSlicedUpdate(this);}