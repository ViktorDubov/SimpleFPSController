using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

using Scripts.Core.Things;
using Scripts.Core.UI;

namespace Scripts.Core
{
    public class TakeThing : MonoBehaviour
    {
        [SerializeField] private GameObject _takedGO;

        private CancellationTokenSource _cts;
        private bool _isStartTake = false;

        public void OnTriggerEnter(Collider other)
        {
            StartTake(other);
        }
        public void OnTriggerStay(Collider other)
        {
            StartTake(other);
        }
        private void StartTake(Collider other)
        {
            if (other.TryGetComponent<TakeIt>(out TakeIt takeIt) && !_isStartTake)
            {
                _isStartTake = true;
                _cts = new CancellationTokenSource();
                StartTakeThing(_cts.Token).Forget();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<TakeIt>(out TakeIt takeIt))
            {
                Promt.PrintMessage(PromptType.None);
                _cts.Dispose();
                _isStartTake = false;
            }
        }

        private async UniTask StartTakeThing(CancellationToken token)
        {
            Camera camera = GetComponentInChildren<Camera>();
            while (!token.IsCancellationRequested)
            {
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 1.5f))
                {
                    if (hit.transform.TryGetComponent<TakeIt>(out TakeIt takeIt))
                    {
                        Promt.PrintMessage(PromptType.TakeIt);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            _takedGO = takeIt.gameObject;
                            _takedGO.transform.SetParent(this.transform);
                            _takedGO.transform.position = camera.transform.position - 1f * camera.transform.forward;
                            _takedGO.SetActive(false);
                            _cts.Cancel();
                        }
                    }
                    else
                    {
                        Promt.PrintMessage(PromptType.None);
                    }
                }
                await UniTask.Yield();
            }
            Promt.PrintMessage(PromptType.None);
        }

    }

}
