using UnityEngine;
using System.Collections;
using System;

public class EzFollowState : EzCameraState
{
    private Vector3 m_targetPosition = Vector3.zero;

    public EzFollowState(EzCamera camera, EzCameraSettings settings)
        : base(camera, settings) 
    {
        //
    }

    //
    public override void EnterState()
    {
        //
    }

    public override void ExitState()
    {
        //
    }

    public override void LateUpdateState()
    {
        if (m_controlledCamera.FollowEnabled)
        {
            UpdateCameraPosition();
            UpdateCameraRotation();
        }
    }

    public override void UpdateState()
    {
        //
    }

    public override void UpdateStateFixed()
    {
        //
    }
    
    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBehindPlayer();
        }
    }

    private void UpdateCameraPosition()
    {
        m_stateSettings.OffsetDistance = Mathf.MoveTowards(m_stateSettings.OffsetDistance, m_stateSettings.DesiredDistance, Time.deltaTime * m_stateSettings.ZoomSpeed);
        m_targetPosition = m_cameraTarget.position + ((m_cameraTarget.up * m_stateSettings.OffsetHeight) + (m_cameraTarget.right * m_stateSettings.LateralOffset) + (m_cameraTransform.forward * -m_stateSettings.OffsetDistance));
        m_cameraTransform.position = Vector3.Lerp(m_cameraTransform.position, m_targetPosition, m_stateSettings .RotateSpeed * Time.deltaTime);
    }

    private void UpdateCameraRotation()
    {
        Vector3 relativePos = (m_cameraTarget.position + (Vector3.right * m_stateSettings.LateralOffset) + (Vector3.up * m_stateSettings.OffsetHeight)) - m_cameraTransform.position;
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        m_cameraTransform.rotation = Quaternion.Lerp(m_cameraTransform.rotation, lookRotation, m_stateSettings.RotateSpeed * Time.deltaTime);
    }

    public void ResetBehindPlayer()
    {
        m_targetPosition = m_cameraTarget.position + ((m_cameraTarget.up * m_stateSettings.OffsetHeight) + (m_cameraTarget.right * m_stateSettings.LateralOffset) + (m_cameraTarget.forward * -m_stateSettings.OffsetDistance));
        m_cameraTransform.position = m_targetPosition;

        Vector3 relativePos = (m_cameraTarget.position + (Vector3.right * m_stateSettings.LateralOffset) + (Vector3.up * m_stateSettings.OffsetHeight)) - m_cameraTransform.position;
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        m_cameraTransform.rotation = lookRotation;
    }
}
