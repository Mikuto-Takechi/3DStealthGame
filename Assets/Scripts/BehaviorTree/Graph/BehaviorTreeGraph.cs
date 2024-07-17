using System;
using System.Linq;
using GraphProcessor;
using MonstersDomain.BehaviorTree;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace MonstersDomain
{
    [Serializable, CreateAssetMenu(menuName = "Behavior Tree")]
    public class BehaviorTreeGraph : BaseGraph
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            // Rootが無かったらつくる
            if (!nodes.Any(x => x is Root)) AddNode(BaseNode.CreateFromType<Root>(Vector2.zero));
        }

        [ContextMenu("Process")]
        void Process()
        {
            var processor = new BehaviorTreeProcessor(this);
            processor.Run();
        }
#if UNITY_EDITOR
        // ダブルクリックでウィンドウが開かれるように
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as BehaviorTreeGraph;

            if (asset == null) return false;

            var window = EditorWindow.GetWindow<BehaviorTreeGraphWindow>();
            window.InitializeGraph(asset);
            return true;
        }
#endif
    }
}