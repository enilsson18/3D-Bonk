using UnityEngine;
using System.Collections;

[System.Serializable]
public class EzCamera : MonoBehaviour 
{
    // Values to be set in the inspector
	[SerializeField] private EzCameraSettings m_settings = null;
    public EzCameraSettings Settings { get { return m_settings; } }
    public void ReplaceSettings(EzCameraSettings newSettings)
    {
        if (newSettings != null)
        {
            m_settings = newSettings;
            m_settings.StoreDefaultValues();
        }
    }

    [SerializeField] private Transform m_target = null;
    public Transform Target { get { return m_target; } }
    private Transform m_transform = null;

    private Vector3 m_relativePosition = Vector3.zero;

    // State Machine and default state
    private EzStateMachine m_stateMachine = null;
    private EzCameraState.State m_defaultState = EzCameraState.State.FOLLOW;
    public EzCameraState.State DefaultState { get { return m_defaultState; } }

    // State for a stationary camera that rotates to look at a target but does not follow it
    private EzStationaryState m_stationaryState = null;
    public EzStationaryState StationaryState
    {
        get { return m_stationaryState; }
        set { m_stationaryState = value; }
    }

    // State for orbiting around a target
    private EzOrbitState m_orbitState = null;
    public EzOrbitState OrbitState
    {
        get { return m_orbitState; }
        set { m_orbitState = value; }
    }
    [SerializeField] private bool m_orbitEnabled = false;
    /// <summary>
    /// True when the camera is  allowed to orbit the taget
    /// </summary>
    public bool OribtEnabled { get { return m_orbitEnabled; } }
    public void SetOrbitEnabled(bool allowOrbit)
    {
        m_orbitEnabled = allowOrbit;
        if (m_orbitState != null)
        {
            m_orbitState.Enabled = m_orbitEnabled;
        }
        else
        {
            if (m_orbitEnabled)
            {
                m_orbitState = new EzOrbitState(this, m_settings);
                if (CameraController != null)
                {
                    CameraController.HandleInputCallback += m_orbitState.HandleInput;
                }
            }
        }
    }

    // State for tracking a target object's position around the environment
    private EzFollowState m_followState = null;
    public EzFollowState FollowState
    {
        get { return m_followState; }
        set { m_followState = value; }
    }

    [SerializeField] private bool m_followEnabled = false;
    public bool FollowEnabled { get { return m_followEnabled; } }
    public void SetFollowEnabled(bool followEnabled)
    {
        m_followEnabled = followEnabled;
        if (m_followState != null)
        {
            m_followState.Enabled = m_followEnabled;
        }
        else
        {
            if (m_followEnabled)
            {
                m_followState = new EzFollowState(this, m_settings);
                if (CameraController != null)
                {
                    CameraController.HandleInputCallback += m_followState.HandleInput;
                }
            }
        }
    }

    /// <summary>
    /// Set the value to true if you want the camera to be able to track an object while still following the player
    /// </summary>
    private EzLockOnState m_lockOnState = null;
    public EzLockOnState LockOnState
    {
        get { return m_lockOnState; }
        set { m_lockOnState = value; }
    }
        
    [SerializeField] private bool m_lockOnEnabled = true;
    public bool LockOnEnabled { get { return m_lockOnEnabled; } }
    public void SetLockOnEnabled(bool enableLockOn)
    {
        m_lockOnEnabled = enableLockOn;
        if (m_lockOnState != null)
        {
            m_lockOnState.Enabled = m_lockOnEnabled;

        }
        else
        {
            if (m_lockOnEnabled)
            {
                m_lockOnState = new EzLockOnState(this, m_settings);
                if (CameraController != null)
                {
                    CameraController.HandleInputCallback += m_lockOnState.HandleInput;
                }
            }
        }
    }

    public bool ZoomEnabled { get { return m_zoomEnabled; } }
    [SerializeField] private bool m_zoomEnabled = true;
    private float m_zoomDelta = 0f;
    private const float ZOOM_DEAD_ZONE = .01f;
    public void SetZoomEnabled(bool isEnabled) { m_zoomEnabled = isEnabled; }


    [SerializeField] private bool m_checkForCollisions = true;
    public bool CollisionsEnabled { get { return m_checkForCollisions; } }
    public void EnableCollisionCheck(bool checkForCollisions)
    {
        m_checkForCollisions = checkForCollisions;
        if (m_cameraCollilder != null)
        {
            if (!checkForCollisions)
            {
                DestroyImmediate(m_cameraCollilder);
                m_cameraCollilder = null;
            }
            else
            {
                m_cameraCollilder.enabled = m_checkForCollisions;
            }
        }
        else
        {
            if (m_checkForCollisions)
            {
                m_cameraCollilder = this.GetOrAddComponent<EzCameraCollider>();
            }
        }
    }

    private EzCameraCollider m_cameraCollilder = null;

    public EzCameraController CameraController { get; private set; }

    private void Start()
    {
        m_transform = this.transform;

        // reset the offset distance be 1/3 of the distance from the min to max
        if (m_settings != null)
        {
            m_settings.OffsetDistance = (m_settings.MaxDistance - m_settings.MinDistance) / 3f;
            m_settings.DesiredDistance = m_settings.OffsetDistance;
            m_settings.StoreDefaultValues();

            m_relativePosition = (m_target.position + (Vector3.up * m_settings.OffsetHeight)) + (m_transform.rotation * (Vector3.forward * -m_settings.OffsetDistance)) + (m_transform.right * m_settings.LateralOffset);
            m_transform.position = m_relativePosition;
        }

        CameraController = this.GetOrAddComponent<EzCameraController>();
        CameraController.Init(this);

        SetLockOnEnabled(m_lockOnEnabled);
        SetFollowEnabled(m_followEnabled);
        SetOrbitEnabled(m_orbitEnabled);

        if (m_checkForCollisions)
        {
            m_cameraCollilder = this.GetOrAddComponent<EzCameraCollider>();
        }

        m_stateMachine = new EzStateMachine();

        m_defaultState = m_followEnabled ? EzCameraState.State.FOLLOW : EzCameraState.State.STATIONARY;
        SetState(m_defaultState);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            if (m_stateMachine != null)
            {
                HandleInput();
                m_stateMachine.UpdateState();
            }
#if UNITY_EDITOR
        }
#endif
    }

    private void LateUpdate()
    {
        if (m_target != null && m_settings != null) // prevent updating if the target is null
        {
            if (m_stateMachine != null)
            {
                if (m_zoomEnabled && Mathf.Abs(m_zoomDelta) > ZOOM_DEAD_ZONE)
                {
                    ZoomCamera(m_zoomDelta);
                }
                m_stateMachine.LateUpdateState();
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_settings.ResetCameraSettings();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_lockOnEnabled && !IsLockedOn)
            {
                SetState(EzCameraState.State.LOCKON);
            }
        }

        // Zoom the camera using the middle mouse button + drag
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.Z))
        {
            m_zoomDelta = Input.GetAxis(ExtensionMethods.MOUSEY);
        }
        else
        {
            m_zoomDelta = 0;
        }
    }

    public void UpdatePosition()
    {
        // Update the position of the camera to reflect any rotation changes
        m_settings.OffsetDistance = Mathf.MoveTowards(m_settings.OffsetDistance, m_settings.DesiredDistance, Time.deltaTime * m_settings.ZoomSpeed);
        m_relativePosition = (m_target.position + (Vector3.up * m_settings.OffsetHeight)) + (m_transform.rotation * (Vector3.forward * -m_settings.OffsetDistance)) + (m_transform.right * m_settings.LateralOffset);
        this.transform.position = m_relativePosition;
    }

    public void SmoothLookAt()
    {
        Vector3 relativePlayerPosition = m_target.position - m_transform.position + m_transform.right * m_settings.LateralOffset;

        Vector3 destDir = Vector3.ProjectOnPlane(relativePlayerPosition, m_transform.up);
        Quaternion lookAtRotation = Quaternion.LookRotation(destDir, Vector3.up);
        m_transform.rotation = Quaternion.Lerp(m_transform.rotation, lookAtRotation, m_settings.RotateSpeed * Time.deltaTime);
    }

    public void ZoomCamera(float zDelta)
    {
        // clamp the value to the min/max ranges
        if (!IsOccluded)
        {
            float step = Time.deltaTime * m_settings.ZoomSpeed * zDelta;
            m_settings.DesiredDistance = Mathf.Clamp(m_settings.OffsetDistance + step, m_settings.MinDistance, m_settings.MaxDistance);
        }
    }

    public void SetCameraTarget(Transform target)
    {
        m_target = target;
    }

    public void SetState(EzCameraState.State nextState)
    {
        switch (nextState)
        {
            case EzCameraState.State.FOLLOW:
                SetFollowEnabled(true);
                m_stateMachine.SetCurrentState(m_followState);                
                break;
            case EzCameraState.State.ORBIT:
                SetOrbitEnabled(true);
                m_stateMachine.SetCurrentState(m_orbitState);
                break;
            case EzCameraState.State.LOCKON:
                SetLockOnEnabled(true);
                m_stateMachine.SetCurrentState(m_lockOnState);
                break;
            case EzCameraState.State.STATIONARY:
            default:
                if (m_stationaryState == null)
                {
                    m_stationaryState = new EzStationaryState(this, m_settings);
                    if (CameraController != null)
                    {
                        CameraController.HandleInputCallback = null;
                    }
                }

                m_stateMachine.SetCurrentState(m_stationaryState);
                break;
        }
    }

    
    public bool IsOccluded
    {
        get
        {
            if (!m_checkForCollisions)
            {
                return false;
            }

            return m_cameraCollilder.IsOccluded;
        }
    }

    public bool IsInOrbit 
    { 
        get 
        {
            if (m_orbitState != null)
            {
                return m_stateMachine.CurrentState == m_orbitState;
            }

            return false;
        } 
    }

    public bool IsLockedOn 
    { 
        get 
        {
            if (m_lockOnState != null)
            {
                return m_stateMachine.CurrentState == m_lockOnState;
            }

            return false;
        } 
    }

    public Vector3 ConvertMoveInputToCameraSpace(float horz, float vert)
    {
        float moveX = (horz * m_transform.right.x) + (vert * m_transform.forward.x);
        float moveZ = (horz * m_transform.right.z) + (vert * m_transform.forward.z);
        return new Vector3(moveX, 0f, moveZ);
    }
}
