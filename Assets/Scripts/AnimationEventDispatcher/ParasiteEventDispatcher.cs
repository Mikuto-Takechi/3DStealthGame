using System;
using UniRx;
using UnityEngine;

public class ParasiteEventDispatcher : MonoBehaviour
{
    public readonly Subject<Unit> EventFootSteps = new();
    void FootSteps() => EventFootSteps.OnNext(Unit.Default);
}
