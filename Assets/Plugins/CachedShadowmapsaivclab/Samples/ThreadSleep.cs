using System.Threading;
using UnityEngine;

namespace CSM.Samples
{
    public class ThreadSleep : MonoBehaviour{

        enum WhenEnum {
            Update,
            FixedUpdate,
            PreRender
        }

        [SerializeField] int amount = 300;
        [SerializeField] WhenEnum when = WhenEnum.Update;

        void Stutter() {

            Thread.Sleep(millisecondsTimeout : this.amount);
        }

        void Update(){
            if (this.when == WhenEnum.Update) {
                this.Stutter();
            }
        }

        void OnPreRender() {
            if (this.when == WhenEnum.PreRender) {
                this.Stutter();
            }
        }

        void FixedUpdate() {         if (this.when == WhenEnum.FixedUpdate) {
            this.Stutter();
        } }
    }

}