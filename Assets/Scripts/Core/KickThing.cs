using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

using Scripts.Core.Things;
using Scripts.Core.UI;


namespace Scripts.Core
{
    public class KickThing : MonoBehaviour
    {
        [SerializeField] private GameObject _kickedGO;
        [SerializeField] private float _force = 5f;

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
            if (other.TryGetComponent<KickIt>(out KickIt kickIt) && !_isStartTake)
            {
                _isStartTake = true;
                _cts = new CancellationTokenSource();
                StartKickThing(_cts.Token).Forget();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<KickIt>(out KickIt kickIt))
            {
                Promt.PrintMessage(PromptType.None);
                _cts.Dispose();
                _isStartTake = false;
            }
        }

        private async UniTask StartKickThing(CancellationToken token)
        {
            Camera camera = GetComponentInChildren<Camera>();
            while (!token.IsCancellationRequested)
            {
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 1.5f))
                {
                    if (hit.transform.TryGetComponent<KickIt>(out KickIt kickIt))
                    {
                        Promt.PrintMessage(PromptType.KickIt);
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            _kickedGO = kickIt.gameObject;
                            _kickedGO.GetComponent<Rigidbody>().AddForce(_force * Vector3.Cross(camera.transform.forward, camera.transform.right) , ForceMode.Impulse);
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
