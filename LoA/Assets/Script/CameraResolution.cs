using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;

        float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16); //  = 가로 / 세로
        float scaleWidth = 1.0f / scaleHeight;

        if (scaleHeight < 1) // 위아래가 남는 경우
        {
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else // 좌우가 남는 경우
        {
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        camera.rect = rect;
    }
}
