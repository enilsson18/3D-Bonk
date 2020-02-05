using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(EzCamera))]
public class EzCameraController : MonoBehaviour
{
    EzCamera m_controlledCamera = null;
    public Action HandleInputCallback = null;

    public void Init(EzCamera camera)
    {
        m_controlledCamera = camera;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (m_controlledCamera != null)
        {
            if (HandleInputCallback != null)
            {
                HandleInputCallback();
            }
        }
    }
}
