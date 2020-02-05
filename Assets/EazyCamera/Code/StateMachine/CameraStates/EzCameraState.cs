using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EzCameraState
{
    public enum State : int
    {
        FOLLOW = 0
        , ORBIT
        , STATIONARY
        , LOCKON
    }

    public State StateName { get; protected set; }
    public bool Enabled { get; set; }


    [SerializeField] protected EzCamera m_controlledCamera = null;
    protected EzCameraSettings m_stateSettings = null;
    protected Transform m_cameraTransform = null;
    protected Transform m_cameraTarget = null;

    protected bool m_initialized = false;

    public EzCameraState(EzCamera camera, EzCameraSettings stateCameraSettings)
    {
        if (!m_initialized)
        {
            m_controlledCamera = camera;
            m_cameraTransform = m_controlledCamera.transform;
            m_cameraTarget = m_controlledCamera.Target;
            m_stateSettings = stateCameraSettings;
            m_initialized = true;
        }
    }

    public abstract void EnterState();
	public abstract void UpdateState();
	public abstract void UpdateStateFixed();
	public abstract void LateUpdateState();
	public abstract void ExitState();

        // Cache any needed strings
    protected const string HORIZONTAL = "Horizontal";
    protected const string VERITCAL = "Vertical";
    protected const string MOUSEX = "Mouse X";
    protected const string MOUSEY = "Mouse Y";
    protected const string MOUSE_WHEEL = "Mouse ScrollWheel";

    public abstract void HandleInput();
    public virtual void Reset() { }
}
