using System;

namespace TankShooter.Common
{
    public static class SubscribeExtensions
    {
        public static void SubscribeToDispose(this IDisposable disposable, IDisposeNotify disposeNotify)
        {
            if (disposable == null)
                throw new Exception("Can't disposable be null action");

            if (disposeNotify == null)
                throw new Exception("Can't disposeNotify be null action");

            disposeNotify.onDispose += disposable.Dispose;
        }
    }
}