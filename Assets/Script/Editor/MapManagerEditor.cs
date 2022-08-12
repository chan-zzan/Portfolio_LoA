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
                
        EditorGUILayout.BeginHorizontal(); // 인스펙터에 가로로 생성

        if (GUILayout.Button("타일 1개 생성"))
        {
            // 타일 생성
            _map.CreateTile(1);
        }

        if (GUILayout.Button("타일 2개 생성"))
        {
            // 타일 생성
            _map.CreateTile(2);
        }

        if (GUILayout.Button("타일 3개 생성"))
        {
            // 타일 생성
            _map.CreateTile(3);
        }
                
        EditorGUILayout.EndHorizontal(); // 인스펙터에 가로로 생성 끝

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("타일 변경"))
        {
            // 타일 변경
            _map.ChangeTile();
        }

        if (GUILayout.Button("타일 회전"))
        {
            // 타일 회전
            _map.TileRotate();
        }

        EditorGUILayout.EndHorizontal();

        // 생성된 타일 이동
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


        if (GUILayout.Button("타일 모두 삭제"))
        {
            // 타일 모두 삭제
            _map.DestroyAllTile();
        }

        if (GUILayout.Button("맵 저장"))
        {
            _map.SaveMap();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
