using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// менеджер, из которого враги определенного типа могут получать следующую точку, куда им нужно пойти
    /// таких менеджеров может быть много и для каждого типа врага их может быть несколько
    /// </summary>
    public class EnemyPathManager : MonoBehaviour
    {
        [SerializeField] private float defaultMinDistance = 10f;
        [SerializeField] private Transform[] pointsTransforms;
        [SerializeField] private Transform defaultPointTransform; //точка по-умолчанию, куда пойдут враги, если вдруг точка не найдена

        private Vector3 defaultPoint;
        private Vector3[] points;
        
        //этот список нам нужен для того, чтобы один раз сюда положить точки, а потом
        //без полной реаллокации очищать, наполнять теми, кто подходит по радиусу и выбирать рандомную
        private List<Vector3> potentialPoints;

        private void Start()
        {
            defaultPoint = defaultPointTransform.position;
            points = pointsTransforms.Select(pt => pt.position).ToArray();
            potentialPoints = new List<Vector3>(points.Length);
        }
        
        public Vector3 GetNextPoint(Vector3 currentPosition, float? requiredMinDistance = null)
        {
            potentialPoints.Clear();

            var minDistance = requiredMinDistance.GetValueOrDefault(defaultMinDistance);
            var sqrDistance = minDistance * minDistance; 
            for (int i = 0; i < points.Length; ++i)
            {
                var point = points[i];
                var pointToCurrentPositionVec = currentPosition - point;
                if (pointToCurrentPositionVec.sqrMagnitude >= sqrDistance)
                {
                    potentialPoints.Add(point);
                }
            }

            if (potentialPoints.Count > 0)
            {
                return potentialPoints[Random.Range(0, potentialPoints.Count)];
            }
            
            return defaultPoint;
        }
    }
}