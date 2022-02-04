using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TankShooter.Common;
using UnityEngine;

public interface IPhysicsBeforeTickListener
{
    void OnBeforePhysicsTick(float dt);
}

public interface IPhysicsAfterTickListener
{
    void OnAfterPhysicsTick(float dt);
}

/// <summary>
/// механизм централизованного обновления, чтобы мы могли задавать приоритеты для обработки отдельных модулей
/// </summary>
public class BattleTimeMachine : MonoBehaviour
{
    private readonly SortedList<int, List<IPhysicsBeforeTickListener>> physicsBeforeTickListeners = new SortedList<int, List<IPhysicsBeforeTickListener>>();
    private readonly SortedList<int, List<IPhysicsAfterTickListener>> physicsAfterTickListeners = new SortedList<int, List<IPhysicsAfterTickListener>>();

    private static BattleTimeMachine instance;
    private static BattleTimeMachine Instance
    {
        get
        {
            if (instance == null)
            {
                var gameObj = new GameObject(nameof(BattleTimeMachine));
                instance = gameObj.AddComponent<BattleTimeMachine>();
                DontDestroyOnLoad(gameObj);
            }

            return instance;
        }
    }

    private void CallBeforePhysicsTick(SortedList<int, List<IPhysicsBeforeTickListener>> sortedList, float dt)
    {
        var sortedIndexCount = sortedList.Count;
        for (var sortedIndex = 0; sortedIndex < sortedIndexCount; ++sortedIndex)
        {
            var listeners = sortedList[sortedIndex];
            var listenersCount = listeners.Count;
            for (var listenerIndex = 0; listenerIndex < listenersCount; ++listenerIndex)
                listeners[listenerIndex].OnBeforePhysicsTick(dt);
        }
    }

    private void CallAfterPhysicsTick(SortedList<int, List<IPhysicsAfterTickListener>> sortedList, float dt)
    {
        var sortedIndexCount = sortedList.Count;
        for (var sortedIndex = 0; sortedIndex < sortedIndexCount; ++sortedIndex)
        {
            var listeners = sortedList[sortedIndex];
            var listenersCount = listeners.Count;
            for (var listenerIndex = 0; listenerIndex < listenersCount; ++listenerIndex)
                listeners[listenerIndex].OnAfterPhysicsTick(dt);
        }
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        //тут можно сделать механизм стабилизации, если будет нужен, чтобы при низком фпс догонять по кол-ву тиков
        CallBeforePhysicsTick(physicsBeforeTickListeners, dt);
        CallAfterPhysicsTick(physicsAfterTickListeners, dt);
    }

    private static IDisposable Subscribe<T>(SortedList<int, List<T>> sortedListeners, T listener, int order)
    {
        if (listener == null)
            throw new Exception("Can't add null listener to battle time machine");

        if (sortedListeners.TryGetValue(order, out var listeners))
        {
            if (listeners.Contains(listener) != true)
                listeners.Add(listener);
        }
        else
        {
            listeners = new List<T> {listener};
            sortedListeners.Add(order, listeners);
        }

        return new ActionDisposable(() =>
        {
            if (listeners.Remove(listener))
            {
                if (listeners.Count == 0)
                {
                    sortedListeners.Remove(order);
                }
            }
        });
    }
    
    public static IDisposable SubscribePhysicsBeforeTick(IPhysicsBeforeTickListener listener, int order = 0)
        => Subscribe(Instance.physicsBeforeTickListeners, listener, order);
    
    public static IDisposable SubscribePhysicsAfterTick(IPhysicsAfterTickListener listener, int order = 0)
        => Subscribe(Instance.physicsAfterTickListeners, listener, order);
}