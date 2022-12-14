using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MapManager))]
[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    MapManager _map;
    SerializedProperty _tileSource;
    SerializedProperty _tileList;
    SerializedProperty _curTileList;
    SerializedProperty _tileParent;
    SerializedProperty _curTileIndex;
    SerializedProperty _tilePos;
    SerializedProperty _convertedTilePos;

    private void OnEnable()
    {
        _map = target as MapManager;
        _tileSource = serializedObject.FindProperty("curTileSource");
        _tileList = serializedObject.FindProperty("TileList");
        _curTileList = serializedObject.FindProperty("curTileList");
        _tileParent = serializedObject.FindProperty("TileParent");
        _curTileIndex = serializedObject.FindProperty("curTileIndex");
        _tilePos = serializedObject.FindProperty("TilePos");
        _convertedTilePos = serializedObject.FindProperty("convertedTilePos");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_tileSource);
        EditorGUILayout.PropertyField(_tileList);
        EditorGUILayout.PropertyField(_curTileList);
        EditorGUILayout.PropertyField(_tilePos);
        EditorGUILayout.PropertyField(_convertedTilePos);


        if (serializedObject.ApplyModifiedProperties())
        {
            _map.SetTilePos(_map.TilePos);
        }

        EditorGUILayout.PropertyField(_tileParent);
        EditorGUILayout.PropertyField(_curTileIndex);
                
        EditorGUILayout.BeginHorizontal(); // 牢胶棋磐俊 啊肺肺 积己

        if (GUILayout.Button("鸥老 1俺 积己"))
        {
            // 鸥老 积己
            _map.CreateTile(1);
        }

        if (GUILayout.Button("鸥老 2俺 积己"))
        {
            // 鸥老 积己
            _map.CreateTile(2);
        }

        if (GUILayout.Button("鸥老 3俺 积己"))
        {
            // 鸥老 积己
            _map.CreateTile(3);
        }
                
        EditorGUILayout.EndHorizontal(); // 牢胶棋磐俊 啊肺肺 积己 场

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("鸥老 函版"))
        {
            // 鸥老 函版
            _map.ChangeTile();
        }

        if (GUILayout.Button("鸥老 雀傈"))
        {
            // 鸥老 雀傈
            _map.TileRotate();
        }

        EditorGUILayout.EndHorizontal();

        // 积己等 鸥老 捞悼
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Up"))
        {
            _map.TileMoveUp();
        }

        if (GUILayout.Button("Down"))
        {
            _map.TileMoveDown();
        }

        if (GUILayout.Button("Left"))
        {
            _map.TileMoveLeft();
        }

        if (GUILayout.Button("Right"))
        {
            _map.TileMoveRight();
        }

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("鸥老 葛滴 昏力"))
        {
            // 鸥老 葛滴 昏力
            _map.DestroyAllTile();
        }

        if (GUILayout.Button("甘 历厘"))
        {
            _map.SaveMap();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
