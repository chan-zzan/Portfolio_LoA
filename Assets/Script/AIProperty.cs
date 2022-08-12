using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIProperty : MonoBehaviour
{
    NavMeshAgent _nav = null;
    protected NavMeshAgent nav
    {
        get
        {
            if (_nav == null)
            {
                _nav = GetComponent<NavMeshAgent>();
            }
            return _nav;
        }
    }

    Animator _anim = null;
    protected Animator myAnim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponent<Animator>();
            }
            return _anim;
        }
    }

    DetectPlayer _detect = null;
    protected DetectPlayer detect
    {
        get
        {
            if (_detect == null)
            {
                _detect = GetComponentInChildren<DetectPlayer>();
            }
            return _detect;
        }
    }

    [SerializeField] MonsterData _data = null;
    protected MonsterData myData
    {
        get => _data;
    }

    

}
