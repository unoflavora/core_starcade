using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ExecuteAlways]
[RequireComponent (typeof(Canvas))]
public class ForceSetWorldCamera : MonoBehaviour
{
    [SerializeField] private Camera m_WorldCamera;
    void Update()
    {
        this.gameObject.GetComponent<Canvas>().worldCamera = Camera.main; 
    }
}
