using UnityEngine;
using System.Collections;

public class EzCameraCollider : MonoBehaviour
{
    private EzCamera m_controlledCamera = null;
    private Camera m_cameraComponent = null;
    private Transform m_cameraTransform = null;
    public bool IsOccluded { get; private set; }
    
    private Vector3[] m_nearClipPlanePoints = new Vector3[4];
    private Vector3[] m_originalClipPlanePoints = new Vector3[4];
    private Vector3 m_pointBehindCamera = new Vector3();

    private float m_nearPlaneDistance = 0f;
    private float m_aspectHalfHeight = 0f;
    private float m_aspectHalfWidth = 0f;

    [SerializeField] private string m_playerLayer = "Player";
    private int m_layermask = 0;


    private void Start()
    {
        m_controlledCamera = this.GetComponent<EzCamera>();
        m_cameraComponent = this.GetComponent<Camera>();
        m_cameraTransform = this.transform;

        m_nearPlaneDistance = m_cameraComponent.nearClipPlane;
        
        m_layermask = (1 << LayerMask.NameToLayer(m_playerLayer)) | (1 << LayerMask.NameToLayer("Ignore Raycast"));
        m_layermask = ~m_layermask;

        UpdateNearClipPlanePoints();
    }

    private void LateUpdate()
    {
        if (m_controlledCamera.CollisionsEnabled)
        {
            HandleCollisions();
        }
    }

    public void HandleCollisions()
    {
        UpdateNearClipPlanePoints();
#if UNITY_EDITOR
        DrawNearPlane();
#endif
        if (IsOccluded)
        {
            CheckOriginalPlanePoints();
#if UNITY_EDITOR
            DrawOriginalPlane();
#endif
        }

        CheckCameraPlanePoints();
    }

    //
    // Camera Occlusion Functions
    //

    private void UpdateNearClipPlanePoints()
    {
        Vector3 nearPlaneCenter = m_cameraTransform.position + m_cameraTransform.forward * m_nearPlaneDistance;
        m_pointBehindCamera = m_cameraTransform.position - m_cameraTransform.forward * m_nearPlaneDistance;

        float halfFOV = Mathf.Deg2Rad * (m_cameraComponent.fieldOfView / 2);
        m_aspectHalfHeight = Mathf.Tan(halfFOV) * m_nearPlaneDistance;
        m_aspectHalfWidth = m_aspectHalfHeight * m_cameraComponent.aspect;

        m_nearClipPlanePoints[0] = nearPlaneCenter + m_cameraTransform.rotation * new Vector3(-m_aspectHalfWidth, m_aspectHalfHeight);
        m_nearClipPlanePoints[1] = nearPlaneCenter + m_cameraTransform.rotation * new Vector3(m_aspectHalfWidth, m_aspectHalfHeight);
        m_nearClipPlanePoints[2] = nearPlaneCenter + m_cameraTransform.rotation * new Vector3(m_aspectHalfWidth , -m_aspectHalfHeight);
        m_nearClipPlanePoints[3] = nearPlaneCenter + m_cameraTransform.rotation * new Vector3(-m_aspectHalfWidth, -m_aspectHalfHeight);
    }

    #region Editor Only Functions
#if UNITY_EDITOR
    private void DrawNearPlane()
    {
        Debug.DrawLine(m_nearClipPlanePoints[0], m_nearClipPlanePoints[1], Color.red);
        Debug.DrawLine(m_nearClipPlanePoints[1], m_nearClipPlanePoints[2], Color.red);
        Debug.DrawLine(m_nearClipPlanePoints[2], m_nearClipPlanePoints[3], Color.red);
        Debug.DrawLine(m_nearClipPlanePoints[3], m_nearClipPlanePoints[0], Color.red);
        Debug.DrawLine(m_pointBehindCamera, m_controlledCamera.Target.position, Color.red);
    }

    private void DrawOriginalPlane()
    {
        Debug.DrawLine(m_originalClipPlanePoints[0], m_originalClipPlanePoints[1], Color.cyan);
        Debug.DrawLine(m_originalClipPlanePoints[1], m_originalClipPlanePoints[2], Color.cyan);
        Debug.DrawLine(m_originalClipPlanePoints[2], m_originalClipPlanePoints[3], Color.cyan);
        Debug.DrawLine(m_originalClipPlanePoints[3], m_originalClipPlanePoints[0], Color.cyan);
        Debug.DrawLine(m_pointBehindCamera, m_controlledCamera.Target.position, Color.cyan);
    }
#endif
    #endregion

    private void CheckCameraPlanePoints()
    {
#if UNITY_EDITOR
        Color lineColor = Color.black;
#endif
        RaycastHit hit;
        float hitDistance = 0;

        for (int i = 0; i < m_nearClipPlanePoints.Length; ++i)
        {

            if (Physics.Linecast(m_controlledCamera.Target.position, m_nearClipPlanePoints[i], out hit, m_layermask))
            {
                if (hit.collider.gameObject.transform.root != m_controlledCamera.Target.root)
                {
                    if (hit.distance > hitDistance)
                    {
                        hitDistance = hit.distance;

                        if (!IsOccluded) // Only store the original position on the original hit
                        {
                            m_controlledCamera.Settings.ResetPositionDistance = m_controlledCamera.Settings.OffsetDistance;
                            //m_controlledCamera.Settings.ResetPositionDistance = m_controlledCamera.Settings.DesiredDistance;
                        }

                        IsOccluded = true;
                        m_controlledCamera.Settings.DesiredDistance = hitDistance - m_nearPlaneDistance;

#if UNITY_EDITOR
                        lineColor = Color.red;
                        Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#else
                        return;
#endif


                    }
                }
            }

#if UNITY_EDITOR
            Debug.DrawLine(m_nearClipPlanePoints[i], m_controlledCamera.Target.position, lineColor);
#endif
        }

        if (!IsOccluded)
        {
            if (Physics.Linecast(m_controlledCamera.Target.position, m_pointBehindCamera, out hit, m_layermask))
            {
#if UNITY_EDITOR
                lineColor = Color.red;
                Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#endif
                IsOccluded = true;
                m_controlledCamera.Settings.ResetPositionDistance = m_controlledCamera.Settings.OffsetDistance;
                m_controlledCamera.Settings.DesiredDistance = hit.distance - m_nearPlaneDistance;
            }
        }   
    }

    private void UpdateOriginalClipPlanePoints()
    {
        Vector3 originalCameraPosition = (m_controlledCamera.Target.position + (Vector3.up * m_controlledCamera.Settings.OffsetHeight)) + (m_cameraTransform.rotation * (Vector3.forward * -m_controlledCamera.Settings.ResetPositionDistance)) + (m_cameraTransform.right * m_controlledCamera.Settings.LateralOffset);
        Vector3 originalPlaneCenter = originalCameraPosition + m_cameraTransform.forward * m_nearPlaneDistance;

        float halfFOV = Mathf.Deg2Rad * (m_cameraComponent.fieldOfView / 2);
        m_aspectHalfHeight = Mathf.Tan(halfFOV) * m_nearPlaneDistance;
        m_aspectHalfWidth = m_aspectHalfHeight * m_cameraComponent.aspect;

        m_originalClipPlanePoints[0] = originalPlaneCenter + m_cameraTransform.rotation * new Vector3(-m_aspectHalfWidth, m_aspectHalfHeight);
        m_originalClipPlanePoints[1] = originalPlaneCenter + m_cameraTransform.rotation * new Vector3(m_aspectHalfWidth, m_aspectHalfHeight);
        m_originalClipPlanePoints[2] = originalPlaneCenter + m_cameraTransform.rotation * new Vector3(m_aspectHalfWidth, -m_aspectHalfHeight);
        m_originalClipPlanePoints[3] = originalPlaneCenter + m_cameraTransform.rotation * new Vector3(-m_aspectHalfWidth, -m_aspectHalfHeight);

        //Vector3 rearPlaneCenter = m_transform.position - m_transform.forward * m_nearPlaneDistance;
        m_pointBehindCamera = m_cameraTransform.position - m_cameraTransform.forward * m_nearPlaneDistance;
    }

    
    private void CheckOriginalPlanePoints()
    {
        UpdateOriginalClipPlanePoints();

        bool objectWasHit = false;
        RaycastHit hit;
        float closestHitDistance = float.MaxValue;

        for (int i = 0; i < m_originalClipPlanePoints.Length; ++i)
        {
#if UNITY_EDITOR
            Color lineColor = Color.blue;
#endif
            if (Physics.Linecast(m_controlledCamera.Target.position, m_originalClipPlanePoints[i], out hit, m_layermask))
            {
                lineColor = Color.red;
                objectWasHit = true;

                if (hit.distance < closestHitDistance)
                {
                    closestHitDistance = hit.distance;
                }
            }

            Debug.DrawLine(m_controlledCamera.Target.position, m_originalClipPlanePoints[i], lineColor);
        }

        if (!objectWasHit)
        {
            m_controlledCamera.Settings.DesiredDistance = m_controlledCamera.Settings.ResetPositionDistance;
            IsOccluded = false;
        }
        else
        {
            if (closestHitDistance > m_controlledCamera.Settings.DesiredDistance)
            {
                m_controlledCamera.Settings.DesiredDistance = closestHitDistance;
            }
        }
    }
}
