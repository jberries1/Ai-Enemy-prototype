using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridData))]
public class CustomGridInspector : Editor
{
    private bool _isEditing;
    private bool _settingNotWalkable;
    private GridData _target;
    public override void OnInspectorGUI()
    {
        _target = target as GridData;
        base.OnInspectorGUI();
        var edit = GUILayout.Button("Edit");
        if (edit)
        {
            _isEditing = !_isEditing;
            _target.edit = _isEditing;
        }
        _settingNotWalkable = GUILayout.Toggle(_settingNotWalkable, "Walkable?");
    }

    private void OnSceneGUI()
    {
        if (_isEditing)
        {
            var mousePos = Event.current.mousePosition;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                var ray = HandleUtility.GUIPointToWorldRay(mousePos);
                var plane = new Plane(Vector3.up, 0);
                plane.Raycast(ray, out float enter);
                var hit = ray.GetPoint(enter);
                hit = new Vector3(hit.x, hit.z, 0);
                OnClick(hit);
            }
            else if (Event.current.type == EventType.Used && Event.current.button == 0)
            {
                var ray = HandleUtility.GUIPointToWorldRay(mousePos);
                var plane = new Plane(Vector3.up, 0);
                plane.Raycast(ray, out float enter);
                var hit = ray.GetPoint(enter);
                hit = new Vector3(hit.x, hit.z, 0);
                OnClick(hit);
            }
        }
    }

    private void OnClick(Vector2 mousePos)
    {
        var node = _target.GetNode(mousePos);
        if(node != null)
        {
            node.isWalkable = _settingNotWalkable;
            if (!_settingNotWalkable)
                _target.AddNotWalkable(new Vector2Int(node.x, node.y));
            else
                _target.RemoveNotWalkable(new Vector2Int(node.x, node.y));
            return;
        }
    }
}
