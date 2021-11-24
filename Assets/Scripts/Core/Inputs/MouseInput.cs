using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace Scripts.Core.Inputs
{
    public class MouseInput : MonoBehaviour
    {
        private IObservable<Vector2> _rotationObservable;
        public IObservable<Vector2> RotationObservable
        {
            get
            {
                if (_rotationObservable == null)
                {
                    _rotationObservable = GetImpulseObservable();
                }
                return _rotationObservable;
            }
        }
        private IObservable<Vector2> GetImpulseObservable()
        {
            return this.FixedUpdateAsObservable()
                .Select
                (
                _ =>
                {
                    float x = Input.GetAxis("Mouse Y");
                    float z = Input.GetAxis("Mouse X");
                    return new Vector2(x, z).normalized;
                }
                );
        }
    }
}
