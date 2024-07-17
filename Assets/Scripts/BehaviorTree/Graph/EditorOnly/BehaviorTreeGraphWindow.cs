#if UNITY_EDITOR
using System.IO;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace MonstersDomain
{
    public class BehaviorTreeGraphWindow : BaseGraphWindow
    {
        //ToolbarView _toolbarView;
        protected override void OnDestroy()
        {
            graphView?.Dispose();
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            Assert.IsNotNull(graph);

            // ウィンドウのタイトルを適当に設定
            var fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(graph));
            titleContent = new GUIContent(ObjectNames.NicifyVariableName(fileName));
            // グラフを編集するためのビューであるGraphViewを設定
            if (graphView == null) graphView = new BehaviorTreeGraphView(this);
            // _toolbarView = new ToolbarView(graphView);
            // rootView.Add(_toolbarView);
            rootView.Add(graphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            view.OpenPinned<ExposedParameterView>();
        }
    }
}
#endif