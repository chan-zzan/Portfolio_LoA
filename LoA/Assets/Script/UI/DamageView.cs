using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageView : MonoBehaviour
{
    private void Update()
    {
        if (this.GetComponent<RectTransform>().anchoredPosition3D.z < 1.2f)
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime * 0.5f, Space.World);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
