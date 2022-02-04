using System;
using UnityEngine;

namespace TankShooter.Common
{
    public interface IDisposeNotify
    {
        event Action onDispose;
    }
    
    public class NotifiableMonoBehaviour : MonoBehaviour, IDisposeNotify, IDisposable
    {
        private bool isDestroyed;
        private bool isDisposed;

        public event Action onDispose;

        private void Awake()
        {
            SafeAwake();
        }

        private void OnDestroy()
        {
            DisposeSelf();
        }

        protected virtual void SafeAwake()
        {
        }

        protected virtual void OnDispose()
        {
        }

        public void Dispose()
        {
            if (isDestroyed != true)
            {
                Destroy(gameObject);
            }
        }

        private void DisposeSelf()
        {
            if (isDestroyed || isDisposed)
                return;

            isDestroyed = true;
            isDisposed = true;

            onDispose?.Invoke();
            onDispose = null;

            OnDispose();
        }
    }
}