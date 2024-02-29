using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TetrisNetwork;
using UnityEngine;

public class PoolingObject : MonoBehaviour, IPooling
{
    public string ObjectName { get { return ""; } }

    public bool IsUsing { get; set; }

    public void OnCollect()
    {
        IsUsing = true;
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        IsUsing = false;
        gameObject.SetActive(false);
    }
}
