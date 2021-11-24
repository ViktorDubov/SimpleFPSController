using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace Scripts.Core.Inputs
{
    public class KeyboardInput : MonoBehaviour
    {
        private IObservable<Vector2> _moveObservable;
        public IObservable<Vector2> MoveObservable
        {
            get
            {
                if (_moveObservable == null)
                {
                    _moveObservable = GetMoveObservable();
                }
                return _moveObservable;
            }
        }
        private IObservable<bool> _lockCursorObservable;
        public IObservable<bool> LockCursorObservable
        {
            get
            {
                if (_lockCursorObservable == null)
                {
                    _lockCursorObservable = GetLockCursorObservableObservable();
                }
                return _lockCursorObservable;
            }
        }

        private IObservable<Vector2> GetMoveObservable()
        {
            return this.FixedUpdateAsObservable()
                .Select
                (
                _ =>
                {
                    float x = Input.GetAxis("Horizontal");
                    float z = Input.GetAxis("Vertical");
                    return new Vector2(x, z).normalized;
                }
                );
        }
        private IObservable<bool> GetLockCursorObservableObservable()
        {
            return Observable.EveryUpdate()
                .Select
                (
                _ =>
                {
                    bool isLockCursor = true;
                    if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        isLockCursor = false;
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        isLockCursor = true;
                    }
                    return isLockCursor;
                }
                );
        }
    }
}
