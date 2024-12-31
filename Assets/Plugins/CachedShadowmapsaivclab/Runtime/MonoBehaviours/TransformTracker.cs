//#define DELAYED_RESET
//#define TRACK_TRANSFORM_SELF
//#define TICK_ON_UPDATE
//#define TICK_ON_FIXEDUPDATE
//#define TICK_ON_LATEUPDATE
//#define ITERATOR_BASED
//#define DEBUG_TRANSFORM_TRACKER

#define NO_HISTORY_BUFFER
#define AUTOREMOVE_DUPLICATES

#if DELAYED_RESET
using System.Collections;
#endif

using UnityEngine;

namespace CSM.Runtime.MonoBehaviours
{
    /// <inheritdoc />
    ///  <summary>
    /// Tracks the transform of the gameObject the monoBehaviour is attached to, it keeps a history buffer of whether transforms has moved for several ticks back.
    /// Ticks being based on frames or physics updates
    ///  </summary>
    [RequireComponent(requiredComponent : typeof(Transform))]
    [DisallowMultipleComponent]
    public class TransformTracker : MonoBehaviour
    {
#if !NO_HISTORY_BUFFER
    CircularBuffer<bool> _move_history = null;
    #if !ITERATOR_BASED || !LINQ_BASED
      bool[] move_array = {};
    #endif

    static int _move_history_length = 8;
#else
bool _last_has_transform_changed = true;
#endif

#if UNITY_EDITOR
void OnValidate()
        {
            var a = this.GetComponents<TransformTracker>();
            if (a.Length > 1)
            {
                #if CACHED_SHADOW_MAP_DEBUG
                Debug.LogWarning($"{this.gameObject} has multiple transform trackers", this);
#endif
#if AUTOREMOVE_DUPLICATES
                if(!Application.isPlaying)
                {
                    for (var i = 1; i < a.Length; i++)
                    {
                        var destroy_this = a[i];
                        UnityEditor.EditorApplication.delayCall += delegate { DestroyImmediate(obj : destroy_this); };
                    }
                }
#endif
            }
        }
#endif

#if UNITY_EDITOR && DEBUG_TRANSFORM_TRACKER
    [SerializeField] private bool disable_tracking = false;
#endif

#if CACHED_SHADOW_MAP_DEBUG
        protected internal bool Debugging
        {
            get { return this.debugging; }
            set { this.debugging = value; }
        }

        [SerializeField] protected string debugTag = $"CSM-TransformTracker";
        [SerializeField] bool debugging = false;
#endif

#if TRACK_TRANSFORM_SELF
    Vector3 _old_position = Vector3.zero;
    Quaternion _old_quaternion = Quaternion.identity;
    Vector3 _old_scale = Vector3.one;
#endif


        ///  <summary>
        /// This property getter has a side effect of reset transform.hasChanged, USE WITH CAUTION!
        ///  </summary>
        bool HasTransformChanged
        {
            get
            {
#if TRACK_TRANSFORM_SELF
        var a = false;

        if (this._old_position != this.transform.position) {
          a = true;
        } else if (this._old_quaternion != this.transform.rotation) {
          a = true;
        } else if (this._old_scale != transform.lossyScale) {
          a = true;
        }

        var transform1 = this.transform;
        this._old_position = transform1.position;
        this._old_quaternion = transform1.rotation;
        this._old_scale = transform1.lossyScale;

        return a;
#else
                var a = this.transform.hasChanged;
                this.CachedTransform = this.transform;

#if !DELAYED_RESET
                this.transform.hasChanged = false;
                // WARNING DANGEROUS SIDE EFFECT, be careful with when this property is accessed
#endif

                return a;
#endif
            }
        }

        /// <summary>
        /// Getter returns whether the transform has changed within the length of the history buffer,
        /// Setter adds to the history buffer and calls events when certain criteria on resting and moving logic is met
        /// </summary>
        public bool Moved
        {
            get
            {
#if UNITY_EDITOR && DEBUG_TRANSFORM_TRACKER
        if (disable_tracking)
        {
          return false;
        }
#endif

#if !NO_HISTORY_BUFFER
        if (this._move_history != null) {
          #if ITERATOR_BASED
            return BooleanIteratorLogic.Any(this._move_history.GetEnumerator());
          #elif LINQ_BASED
            return this._move_history.Any(b => b);
          #else
          move_array = this._move_history.ToArray();
            return ArrayUtilities.Contains(move_array, true);
          #endif
        }
        else
        {
          _move_history = new CircularBuffer<bool>(_move_history_length);
        }

        return true;
#else
                return this._last_has_transform_changed;
#endif
            }
            private set
            {
#if UNITY_EDITOR && DEBUG_TRANSFORM_TRACKER
        if (disable_tracking)
        {
          this.OnRestingEvent?.Invoke();
          return;
        }
#endif

#if !NO_HISTORY_BUFFER
        #if !ITERATOR_BASED || !LINQ_BASED
          move_array = this._move_history.ToArray();
        #endif

        if (value) {
          this.OnMovingEvent?.Invoke();

          #if ITERATOR_BASED
            if (BooleanIteratorLogic.AllFalse(this._move_history.GetEnumerator())){
          #elif LINQ_BASED
                      if (this._move_history.All(b => !b)) {
          #else
          if (!ArrayUtilities.Contains(move_array, true)) {
            #endif
            this.OnNowMovingEvent?.Invoke();
          }

          #if ITERATOR_BASED
        }else if (BooleanIteratorLogic.Any(this._move_history.GetEnumerator())){
            if (BooleanIteratorLogic.OnlyFirst(this._move_history.GetEnumerator())){
                      this.OnNowRestingEvent?.Invoke();
          #elif LINQ_BASED
        } else if (this._move_history.Any(b => b)) {
          if (this._move_history.First() && this._move_history.Skip(1).All(b => !b)) {
                      this.OnNowRestingEvent?.Invoke();
          #else
        } else if (ArrayUtilities.Contains(move_array, true)) {
          if (move_array[0]) {
            if (move_array.Skip(1).All(b => !b)) {
              this.OnNowRestingEvent?.Invoke();
            }
            #endif
          }
        } else {
          this.OnRestingEvent?.Invoke();
        }

        #if CACHED_SHADOW_MAP_DEBUG
        if (this.Debugging) {
          Debug.Log($"{this.debugTag}:{string.Join(",", this._move_history.ToArray().Select(e => e.ToString()))}");
        }
        #endif

        this._move_history.Prepend(value);
#else
                var prev = this._last_has_transform_changed;

                this._last_has_transform_changed = value;
                if (value)
                {
                    if (!prev)
                    {
                        this.OnNowMovingEvent?.Invoke();
                    }

                    this.OnMovingEvent?.Invoke();
                }
                else
                {
                    if (prev)
                    {
                        this.OnNowRestingEvent?.Invoke();
                    }

                    this.OnRestingEvent?.Invoke();
                }
#endif
            }
        }

        /// <summary>
        ///
        /// </summary>
        public delegate void TransformStateDelegate();

        /// <summary>
        ///
        /// </summary>
        public delegate void TickDelegate();

        /// <summary>
        ///
        /// </summary>
        public delegate void EnabledStateDelegate();

        /// <summary>
        ///
        /// </summary>
        public event TransformStateDelegate OnNowMovingEvent;

        /// <summary>
        ///
        /// </summary>
        public event TransformStateDelegate OnMovingEvent;

        /// <summary>
        ///
        /// </summary>
        public event TransformStateDelegate OnNowRestingEvent;

        /// <summary>
        ///
        /// </summary>
        public event TransformStateDelegate OnRestingEvent;

        /// <summary>
        ///
        /// </summary>
        public event EnabledStateDelegate OnChangeEnabled;

        /// <summary>
        ///
        /// </summary>
        public event TickDelegate OnTickEvent;

        /// <summary>
        ///
        /// </summary>
        public bool ChangeEnableState
        {
            get { return this._did_change_enabled_state; }
            private set
            {
                if (this._last_enabled_state != value)
                {
                    this.OnChangeEnabled?.Invoke();
                    this._did_change_enabled_state = true;
                }
                else
                {
                    this._did_change_enabled_state = false;
                }

                this._last_enabled_state = value;
            }
        }

        bool _did_change_enabled_state = false;
        bool _last_enabled_state = false;


        public Transform CachedTransform
        {
            get => this.cachedTransform;
            private set
            {
                this.cachedTransform = value;
                this.CachedRotation = value.rotation;
                this.CachedPosition = value.position;
                this.CachedForward = value.forward;
            }
        }

        [field: SerializeField]
        public Quaternion CachedRotation { get; private set; }

        [field: SerializeField]
        public Vector3 CachedPosition { get; private set; }
        [field: SerializeField]
        public Vector3 CachedForward { get; private set; }

        [SerializeField] Transform cachedTransform;

        void OnDisable()
        {
            this.ChangeEnableState = false;
#if CACHED_SHADOW_MAP_DEBUG
            if (this.Debugging)
            {
                Debug.Log($"CSM: {this} OnDisable");
            }
#endif
            this.Teardown();
#if DELAYED_RESET && !TRACK_TRANSFORM_SELF
        StopCoroutine(this.WaitForEndOfFrameCoroutine());
#endif
#if !NO_HISTORY_BUFFER
      this._move_history = null;
#endif
        }

        void OnDestroy()
        {
            this.Teardown();
        }

        ~TransformTracker()
        {
            this.Teardown();
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void Teardown()
        {
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void Setup()
        {

        }

        void OnEnable()
        {
#if !NO_HISTORY_BUFFER
      this._move_history = new CircularBuffer<bool>(_move_history_length);
#endif
#if CACHED_SHADOW_MAP_DEBUG
            if (this.Debugging)
            {
                Debug.Log($"CSM: {this} OnEnable");
            }
#endif

            this.CachedTransform = this.transform;
            this.Setup();
            this.ChangeEnableState = true;


#if DELAYED_RESET && !TRACK_TRANSFORM_SELF
        StartCoroutine(this.WaitForEndOfFrameCoroutine());
#endif
        }


        /// <summary>
        ///
        /// </summary>
        protected void Tick()
        {
            this.Moved = this.HasTransformChanged;
            this.ChangeEnableState = true;
            this.OnTickEvent?.Invoke();
        }

#if TICK_ON_FIXEDUPDATE
    void FixedUpdate() {
        this.Tick();
    }
#endif
#if TICK_ON_UPDATE
    void Update() {
        this.Tick();
    }
#endif
#if TICK_ON_LATEUPDATE
      void LateUpdate() { this.Tick(); }
#endif

#if DELAYED_RESET && !TRACK_TRANSFORM_SELF
    IEnumerator WaitForEndOfFrameCoroutine() {
      while (true) {
        yield return new WaitForEndOfFrame();
        this.transform.hasChanged = false;
      }
    }
#endif
    }
}