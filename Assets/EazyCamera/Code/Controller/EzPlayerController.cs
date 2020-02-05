using UnityEngine;
using System.Collections;

/// <summary>
/// This is the main class for controlling both camera and player. It is recommended to attach this to the player or camera in the scene, but not necessary
/// </summary>
public class EzPlayerController : MonoBehaviour 
{
	[SerializeField] private EzCamera m_camera = null;
    [SerializeField] private EzMotor m_controlledPlayer = null;

    private void Start()
    {
        // if either the player or camera are null, attempt to find them
        SetUpControlledPlayer();
        SetUpCamera();
    }

    private void Update()
    {
        if (m_controlledPlayer != null && m_camera != null)
        {
            HandleInput();
        }
    }

    private void SetUpControlledPlayer()
    {
        if (m_controlledPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                m_controlledPlayer = playerObj.GetComponent<EzMotor>();
            }
        }
    }

    private void SetUpCamera()
    {
        if (m_camera == null)
        {
            m_camera = Camera.main.GetComponent<EzCamera>();
            if (m_camera == null)
            {
                m_camera = Camera.main.gameObject.AddComponent<EzCamera>();
            }
        }
    }

    private void HandleInput()
    {
        // Update player movement first
        // cache the inputs
        float horz = Input.GetAxis(ExtensionMethods.HORIZONTAL);
        float vert = Input.GetAxis(ExtensionMethods.VERITCAL);
        
        // Convert movement to camera space
        Vector3 moveVector = m_camera.ConvertMoveInputToCameraSpace(horz, vert);

        // Move the Player
        m_controlledPlayer.MovePlayer(moveVector.x, moveVector.z, Input.GetKey(KeyCode.LeftShift));
    }
}
