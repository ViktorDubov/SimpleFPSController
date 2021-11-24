using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace Scripts.Core.Inputs
{
    public class GeneralInput : MonoBehaviour
    {
        [SerializeField] private KeyboardInput _keyboardInput;
        [SerializeField] private MouseInput _mouseInput;

        public IObservable<Vector2> MoveObservable { get { return _keyboardInput.MoveObservable; } }
        public IObservable<Vector2> RotationObservable { get { return _mouseInput.RotationObservable; } }
        public IObservable<bool> LockCursorObservable { get { return _keyboardInput.LockCursorObservable; } }

        private static GeneralInput _instance;
        public static GeneralInput Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = FindObjectOfType<GeneralInput>();
                    if (_instance==null)
                    {
                        throw new ArgumentNullException("there is no active GO with GeneralInput");
                    }
                }
                return _instance;
            }
        }
    }
}
