using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

using Scripts.Core.Things;
using Scripts.Core.UI;

namespace Scripts.Core
{
    public class DrugThing : MonoBehaviour
    {
        [SerializeField] private GameObject _drugedGO;

        private CancellationTokenSource _cts;
        private bool _isStartDrug=false;

        public void OnTriggerEnter(Collider other)
        {
            StartDrug(other);
        }
        public void OnTriggerStay(Collider other)
        {
            StartDrug(other);
        }
        private void StartDrug(Collider other)
        {
            if (other.TryGetComponent<DrugIt>(out DrugIt drugIt) && !_isStartDrug)
            {
                _isStartDrug = true;
                _cts = new CancellationTokenSource();
                StartDrugThing(_cts.Token).Forget();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<DrugIt>(out DrugIt drugIt))
            {
                Promt.PrintMessage(PromptType.None);
                _cts.Dispose();
                _isStartDrug = false;
            }
        }

        private async UniTask StartDrugThing(CancellationToken token)
        {
            Camera camera = GetComponentInChildren<Camera>();
            while (!token.IsCancellationRequested)
            {
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 5f))
                {
                    if (hit.transform.TryGetComponent<DrugIt>(out DrugIt drugIt))
                    {
                        Promt.PrintMessage(PromptType.DrugIt);
                        if (Input.GetKeyDown(KeyCode.Q))
                        {
                            _drugedGO = drugIt.gameObject;
                            Destroy(_drugedGO.GetComponent<Rigidbody>());
                            _drugedGO.transform.SetParent(camera.transform);
                            _drugedGO.transform.position = camera.transform.position + 2f * camera.transform.forward;
                            _drugedGO.transform.rotation = Quaternion.Euler(Vector3.zero);
                            _cts.Cancel();
                            await PutDrugThing();
                        }
                    }
                    else
                    {
                        Promt.PrintMessage(PromptType.None);
                    }
                }
                await UniTask.Yield();
            }
        }
        private async UniTask PutDrugThing()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3), ignoreTimeScale: false);
            Promt.PrintMessage(PromptType.DropIt);
            while (!cts.Token.IsCancellationRequested)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _drugedGO.transform.SetParent(null);
                    _drugedGO.AddComponent<Rigidbody>();
                    Promt.PrintMessage(PromptType.None);
                    await UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: false);

                    cts.Cancel();
                }
                await UniTask.Yield();
            }
            _isStartDrug = false;
        }

    }

}
