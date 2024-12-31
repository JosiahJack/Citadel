using System.Collections;
using UnityEngine;

namespace CSM.Samples
{
    public class AsyncTeleportation : MonoBehaviour
    {
        [SerializeField] Transform p1;
        [SerializeField] Transform p2;
        [SerializeField][Range(1,9999)] int framesToWait = 100;
        [SerializeField] bool isAsync = true;

        // Start is called before the first frame update
        void Start()
        {
            if (this.p1 == null || this.p2 == null) {
                return;
            }

            this.StartCoroutine(routine : this.Teleport());
        }

        public async void AsyncTeleport(Vector3 pos)
        {
            this.transform.position = this.p2.position;
            //await Task.Run(()=>this.transform.position = pos);
        }

        public IEnumerator Teleport()
        {
            while (this.enabled)
            {
                var i = 0;
                while (i++ < this.framesToWait) {
                    yield return new WaitForEndOfFrame();
                }

                if (this.isAsync) {
                    this.AsyncTeleport(pos : this.p1.position);
                } else {
                    this.transform.position = this.p1.position;
                }

                i = 0;
                while (i++ < this.framesToWait) {
                    yield return new WaitForEndOfFrame();
                }

                if (this.isAsync) {
                    this.AsyncTeleport(pos : this.p2.position);
                } else {
                    this.transform.position = this.p2.position;
                }
            }
        }
    }
}
