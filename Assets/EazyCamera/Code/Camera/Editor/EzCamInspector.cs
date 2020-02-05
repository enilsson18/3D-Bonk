using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EzCamera))]
public class EzCamInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //return;

        EzCamera cam = (EzCamera)target;

        if (cam != null)
        {
            Transform camTarget = EditorGUILayout.ObjectField("Target", cam.Target, typeof(Transform), true) as Transform;
            if (camTarget != cam.Target)
            {
                cam.SetCameraTarget(camTarget);
            }

            EzCameraSettings settings = EditorGUILayout.ObjectField("Camera Settings", cam.Settings, typeof(EzCameraSettings), false) as EzCameraSettings;
            if (settings != cam.Settings)
            {
                cam.ReplaceSettings(settings);
            }

            string toggleText = null;

            // Additional States
            toggleText = "Orbit Enabled";
            bool isEnabled = EditorGUILayout.Toggle(toggleText, cam.OribtEnabled);
            if (isEnabled != cam.OribtEnabled)
            {
                cam.SetOrbitEnabled(isEnabled);
                cam.SetFollowEnabled(isEnabled); // An orbit cam includes follow logic
            }


            if (!cam.OribtEnabled)
            {
                toggleText = "Follow Enabled";
                isEnabled = EditorGUILayout.Toggle(toggleText, cam.FollowEnabled);
                if (isEnabled != cam.FollowEnabled)
                {
                    cam.SetFollowEnabled(isEnabled);
                }
            }

            toggleText = "Lock On Enabled";
            isEnabled = EditorGUILayout.Toggle(toggleText, cam.LockOnEnabled);
            if (isEnabled != cam.LockOnEnabled)
            {
                cam.SetLockOnEnabled(isEnabled);
            }

            // Cmaera Options
            toggleText = "Zoom Enabled";
            isEnabled = EditorGUILayout.Toggle(toggleText, cam.ZoomEnabled);
            if (isEnabled != cam.ZoomEnabled)
            {
                cam.SetZoomEnabled(isEnabled);
            }

            toggleText = "Collisions Enabled";
            isEnabled = EditorGUILayout.Toggle(toggleText, cam.CollisionsEnabled);
            if (isEnabled != cam.CollisionsEnabled)
            {
                cam.EnableCollisionCheck(isEnabled);
            }

            EditorUtility.SetDirty(cam);
        }
    }
}
