using UnityEngine;
using System.Collections;

public class EzMotor : MonoBehaviour 
{
	[SerializeField] private float m_walkSpeed = 5f;
    [SerializeField] private float m_runSpeed = 15f;
    [SerializeField] private float m_acceleration = 10f;
    [SerializeField] private float m_rotateSpeed = 5f;
    private float m_currentSpeed = 5f;
    private float m_speedDelta = 0f;

    private Vector3 m_moveVector = new Vector3();

    private void Start()
    {
        m_speedDelta = m_runSpeed - m_walkSpeed;
        if (m_speedDelta == 0)
        {
            m_speedDelta = .01f;
        }
    }

    public void MovePlayer(float moveX, float moveZ, bool isRunning)
    {
        // Update the move Deltas
        m_moveVector.x = moveX;
        m_moveVector.z = moveZ;
        m_moveVector.Normalize();

        // gradually move toward the desired speed
        m_currentSpeed = Mathf.MoveTowards(m_currentSpeed, (isRunning ? m_runSpeed : m_walkSpeed), m_acceleration * Time.deltaTime);

        // Scale the vector by the move speed
        m_moveVector *= m_currentSpeed;

        // Move the character
        //m_charController.Move(m_moveVector);

        if (moveX != 0 || moveZ != 0)
        {
            float step = m_rotateSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(m_moveVector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, step);
        }
    }

    //public void Jump()
    //{
    //    //
    //}

    public float GetNormalizedSpeed()
    {
        return m_moveVector.magnitude /  m_runSpeed;
    }
}
