using UnityEngine;

namespace Common
{
    /// <summary>
    /// расширение для удобства, чтобы можно было искать компоненты и сразу получить bool
    /// </summary>
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(this Component parentComponent, out T component, bool includeInactive = false)
            where T : Component
        {
            return TryGetComponentInChildren(parentComponent.gameObject, out component, includeInactive);
        }

        public static bool TryGetComponentInChildren<T>(this GameObject parentGameObject, out T component, bool includeInactive = false)
            where T : Component
        {
            return (component = parentGameObject?.GetComponentInChildren<T>(includeInactive)) ?? false;
        }
    }
}