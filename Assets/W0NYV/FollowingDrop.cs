using UnityEngine;
using UniRx;

namespace Puzzules
{
    public class FollowingDrop : MonoBehaviour
    {

        void Start()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => FollowMouse())
                .AddTo(this);
            
        }

        private void FollowMouse() {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        }

    }
}

