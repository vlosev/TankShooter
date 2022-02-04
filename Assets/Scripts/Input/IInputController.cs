using System;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Battle
{
    public interface IInputController
    {
        /// <summary>
        /// скорость движения танка, которая передается от контроллера
        /// это желаемая скорость, то есть, условно, насколько мы нажали "газ" 
        /// </summary>
        IReadonlyReactiveProperty<float> Acceleration { get; }

        /// <summary>
        /// значение поворота "руля"
        /// </summary>
        IReadonlyReactiveProperty<float> Steering { get; }
        
        /// <summary>
        /// точка, куда должен целиться танк
        /// </summary>
        IReadonlyReactiveProperty<Vector3> TargetPoint { get; }
        
        /// <summary>
        /// дельта изменения зума, например, колесиком или двумя пальцами для тача
        /// </summary>
        IReadonlyReactiveProperty<float> CameraZoomDelta { get; }

        /// <summary>
        /// свойство, которое говорит, что кнопка выстрела зажата, нужно, например, для пулемета
        /// </summary>
        IReadonlyReactiveProperty<bool> Shooting { get; }
        
        /// <summary>
        /// флаг для передвижения камеры, например нажат джойстик или зажата правая кнопка мыши
        /// </summary>
        IReadonlyReactiveProperty<bool> CameraMove { get; }
        
        /// <summary>
        /// сообщает насколько сдвинулась камера
        /// </summary>
        IReadonlyReactiveProperty<Vector2> CameraMoveDelta { get; }

        /// <summary>
        /// событие для выстрела
        /// </summary>
        event Action DoShot;

        /// <summary>
        /// событие для выбора предыдущего оружия
        /// </summary>
        event Action DoSelectPrevWeapon;

        /// <summary>
        /// событие для выбора следующего оружия
        /// </summary>
        event Action DoSelectNextWeapon;

        /// <summary>
        /// событие для выбора оружия по индексу
        /// </summary>
        event Action<int> DoSelectWeapon;
    }
}