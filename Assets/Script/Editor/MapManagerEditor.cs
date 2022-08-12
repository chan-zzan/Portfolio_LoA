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
                
        EditorGUILayout.BeginHorizontal(); // �ν����Ϳ� ���η� ����

        if (GUILayout.Button("Ÿ�� 1�� ����"))
        {
            // Ÿ�� ����
            _map.CreateTile(1);
        }

        if (GUILayout.Button("Ÿ�� 2�� ����"))
        {
            // Ÿ�� ����
            _map.CreateTile(2);
        }

        if (GUILayout.Button("Ÿ�� 3�� ����"))
        {
            // Ÿ�� ����
            _map.CreateTile(3);
        }
                
        EditorGUILayout.EndHorizontal(); // �ν����Ϳ� ���η� ���� ��

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Ÿ�� ����"))
        {
            // Ÿ�� ����
            _map.ChangeTile();
        }

        if (GUILayout.Button("Ÿ�� ȸ��"))
        {
            // Ÿ�� ȸ��
            _map.TileRotate();
        }

        EditorGUILayout.EndHorizontal();

        // ������ Ÿ�� �̵�
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


        if (GUILayout.Button("Ÿ�� ��� ����"))
        {
            // Ÿ�� ��� ����
            _map.DestroyAllTile();
        }

        if (GUILayout.Button("�� ����"))
        {
            _map.SaveMap();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
