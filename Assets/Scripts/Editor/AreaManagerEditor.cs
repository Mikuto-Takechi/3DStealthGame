using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MonstersDomain
{
    [CustomEditor(typeof(AreaManager))]
    public class AreaManagerEditor : Editor
    {
        BoxBoundsHandle _boxHandle;
        Bounds _cacheBounds;
        AreaManager _target;

        void OnEnable()
        {
            _target = target as AreaManager;
            _boxHandle = new BoxBoundsHandle();
        }

        void OnSceneGUI()
        {
            if (_target.AreaDataList is not { Count: > 0 }) return;
            serializedObject.Update();
            for (var i = 0; i < _target.AreaDataList.Count; i++)
            {
                var floorData = _target.AreaDataList[i];
                _boxHandle.center = floorData.FloorBounds.center;
                _boxHandle.size = floorData.FloorBounds.size;
                EditorGUI.BeginChangeCheck();
                _boxHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    _cacheBounds.center = _boxHandle.center;
                    _cacheBounds.size = _boxHandle.size;
                    floorData.FloorBounds = _cacheBounds;
                    EditorUtility.SetDirty(_target);
                }

                EditorGUI.BeginChangeCheck();
                var pos = _target.AreaDataList[i].WarpPoint;
                Handles.Label(pos, new GUIContent($"Warp Point: {i}"));
                var warpPoint = Handles.FreeMoveHandle(pos, 0.5f, Vector3.one, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    _target.AreaDataList[i].WarpPoint = warpPoint;
                    EditorUtility.SetDirty(_target);
                }

                DrawPatrolAnchors(floorData);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawPatrolAnchors(AreaData areaData)
        {
            if (areaData.PatrolAnchors.Count <= 0) return;

            for (var i = 0; i < areaData.PatrolAnchors.Count; i++)
            {
                var pos = areaData.PatrolAnchors[i];
                var size = HandleUtility.GetHandleSize(pos);
                // ハンドルを表示する
                EditorGUI.BeginChangeCheck();
                pos = Handles.PositionHandle(pos, Quaternion.identity);
                var labelPos = pos;
                labelPos.y -= size * 0.25f;
                Handles.Label(labelPos, new GUIContent(i.ToString()));
                if (i > 0)
                {
                    Handles.DrawLine(areaData.PatrolAnchors[i - 1], pos, Handles.lineThickness);
                    Handles.ConeHandleCap(0, pos, Quaternion.LookRotation(pos - areaData.PatrolAnchors[i - 1]),
                        size * 0.25f, EventType.Repaint);
                }

                // アンカーの位置が変更されたら反映する
                if (EditorGUI.EndChangeCheck())
                {
                    areaData.PatrolAnchors[i] = pos;
                    EditorUtility.SetDirty(_target);
                }
            }
        }
    }
}