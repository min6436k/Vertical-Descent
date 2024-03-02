using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSize : MonoBehaviour
{
    private float _ScreenSize;
    void Awake()
    {
        _ScreenSize = (float)Screen.height/Screen.width*12.375f;
        if (name == "Main Camera")
        {
            GetComponent<Camera>().orthographicSize = _ScreenSize;
        }

        if (name == "Virtual Camera")
        {
            GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = _ScreenSize;
        }

    }
}
