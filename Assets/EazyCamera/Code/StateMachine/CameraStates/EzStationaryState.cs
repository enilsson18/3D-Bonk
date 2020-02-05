using UnityEngine;
using System.Collections;
using System;

public class EzStationaryState : EzCameraState
{
    public EzStationaryState(EzCamera camera, EzCameraSettings settings)
        : base(camera, settings) { }

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
        m_controlledCamera.SmoothLookAt();
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
        //
    }
}
