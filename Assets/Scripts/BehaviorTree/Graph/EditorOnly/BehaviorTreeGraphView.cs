#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using MonstersDomain.BehaviorTree;
using UnityEditor;

namespace MonstersDomain
{
    public class BehaviorTreeGraphView : BaseGraphView
    {
        public BehaviorTreeGraphView(EditorWindow window) : base(window)
        {
        }

        protected override bool canDeleteSelection
        {
            // Rootを消せないように
            get { return !selection.Any(e => e is RootView); }
        }

        public override IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries())
            {
                // Rootを追加できないように
                if (nodeMenuItem.type == typeof(Root)) continue;
                yield return nodeMenuItem;
            }
        }
    }
}
#endif