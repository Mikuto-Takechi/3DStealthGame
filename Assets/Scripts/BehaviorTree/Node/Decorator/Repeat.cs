using GraphProcessor;
using UnityEngine;

namespace MonstersDomain.BehaviorTree
{
    /// <summary>
    ///     子ノードを指定した回数繰り返す。<br />
    ///     子ノードが常にSuccessなら繰り返した後にSuccessを返す。<br />
    ///     子ノードがFailureを返した場合はループを中断し、Failureを返す。<br />
    ///     子ノードがRunningならこのノードもRunningを返し、カウントアップせずに子ノードの処理を待ちます。
    /// </summary>
    [NodeMenuItem("BehaviorTree/Decorator/Repeat")]
    public class Repeat : Decorator
    {
        [SerializeField] int _limit = 1;
        int _count;

        public override BTState Tick()
        {
            if (_limit > 0 && _count < _limit)
                switch (Child.Tick())
                {
                    case BTState.Running:
                        return BTState.Running;
                    case BTState.Failure:
                        _count = 0;
                        return BTState.Failure;
                    default:
                        _count++;
                        if (_count == _limit)
                        {
                            _count = 0;
                            return BTState.Success;
                        }

                        return BTState.Running;
                }

            _count = 0;
            return BTState.Failure;
        }
    }
}