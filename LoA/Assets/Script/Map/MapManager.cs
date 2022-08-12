using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject curTileSource;
    public List<GameObject> TileList = null;
    public List<GameObject> curTileList = null;
    public Transform TileParent;
    public int curTileIndex = 0;

    [Tooltip("(0,0) ~ (8,14)")]
    public Vector2 TilePos;
    public Vector3 convertedTilePos;

    public void CreateTile(int n)
    {
        curTileList.Clear();

        for (int i = 0; i < n; i++)
        {
            GameObject obj = Instantiate(curTileSource, TileParent);
            obj.transform.position = new Vector3(convertedTilePos.x + i, obj.transform.position.y, convertedTilePos.z);

            curTileList.Add(obj);
        }           
    }

    public void ChangeTile()
    {
        if (curTileIndex >= TileList.Count)
        {
            curTileIndex = 0;
        }

        // 현재 선택된 타일 변경
        curTileSource = TileList[curTileIndex++];        
    }

    public void DestroyAllTile()
    {
        // 실제 타일 삭제
        var children = TileParent.GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child != TileParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        curTileList.Clear();
    }

    public void SetTilePos(Vector2 pos)
    {
        // (-4, 0, -5) -> 오른쪽 아래 좌표 -> 기준(0,0)
        convertedTilePos = new Vector3(-4 + pos.x, 0, -5 + pos.y);
    }

    public void TileMoveUp()
    {
        foreach(GameObject obj in curTileList)
        {
            obj.transform.position = obj.transform.position + new Vector3(0, 0, 1); 
        }
    }

    public void TileMoveDown()
    {
        foreach (GameObject obj in curTileList)
        {
            obj.transform.position = obj.transform.position + new Vector3(0, 0, -1);
        }
    }

    public void TileMoveLeft()
    {
        foreach (GameObject obj in curTileList)
        {
            obj.transform.position = obj.transform.position + new Vector3(-1, 0, 0);
        }
    }

    public void TileMoveRight()
    {
        foreach (GameObject obj in curTileList)
        {
            obj.transform.position = obj.transform.position + new Vector3(1, 0, 0);
        }
    }

    public void TileRotate()
    {
        if (curTileList.Count == 1)
        {
            // 타일이 1개인 경우 회전 x
            return;
        }

        for (int i = 0; i < curTileList.Count; i++)
        {
            curTileList[i].transform.RotateAround(curTileList[0].transform.position, Vector3.up, 90.0f);
        }
    }

    public void SaveMap()
    {
        GameObject obj = Instantiate(TileParent.gameObject);
        obj.name = "new Map";
    }    
}
