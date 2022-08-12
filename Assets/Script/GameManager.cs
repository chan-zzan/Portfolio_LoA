using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    #region �̱���
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GameManager";
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    bool pause = false; // �Ͻ�����

    private void Update()
    {
        if (pause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    //�ε�ȭ�� ���� �ε��(�ε���)
    public void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    //�ε�ȭ�� ���� �ε��(��Ʈ��)
    public void LoadScene(string Scene)
    {
        SceneManager.LoadSceneAsync(Scene);
    }

    public void PauseScene()
    {
        if(!pause)
        {
            pause = true;
        }
    }

    public void RePlay()
    {
        if (pause)
        {
            pause = false;
        }
    }
}
