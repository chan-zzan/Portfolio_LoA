using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEvent : MonoBehaviour
{
    public UnityAction Attack = null;

    public void OnAttack()
    {
        Attack?.Invoke();
    }
}
