using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class EzLockOnTarget : MonoBehaviour
{
    // need AABB and  transform
    //public GameObject TargetIcon { get { return m_targetIcon; } }
    [SerializeField] private GameObject m_targetIcon = null;
    [SerializeField] private EzCamera m_playerCamera = null;
    [SerializeField] private Color32 m_inactiveColor = new Color32(127,  127, 127, 127);
    [SerializeField] private Color32 m_activeColor = new Color32(255, 0,  0, 255);
    [SerializeField] private float m_activationDistance = 10f;

    private SphereCollider m_collider = null;
    private Renderer m_iconRenderer = null;

    private void Awake()
    {
        m_collider = this.GetComponent<SphereCollider>();
        m_collider.isTrigger = true;
        m_collider.radius = m_activationDistance;
    }

    private void Start()
    {
        if (m_playerCamera == null)
        {
            m_playerCamera = GameObject.FindObjectOfType<EzCamera>();
        }

        m_iconRenderer = m_targetIcon.GetComponent<Renderer>();
        m_iconRenderer.enabled = false;

        SetIconActive(false);

        // Set the physics layer as not to interfere with Camera Occlusion
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EzMotor>() != null)
        {
            SetIconVisible(true);

            if (m_playerCamera != null)
            {
                EzLockOnState lockonState = m_playerCamera.LockOnState;
                if (lockonState != null)
                {
                    lockonState.AddTarget(this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EzMotor>() != null)
        {
            SetIconVisible(false);

            EzLockOnState lockonState = m_playerCamera.LockOnState;
            if (lockonState != null)
            {
                lockonState.RemoveTarget(this);
            }
        }
    }

    public void SetIconActive(bool isActive = true)
    {
        if (m_targetIcon != null)
        {
            //m_targetIcon.SetActive(isActive);
            m_iconRenderer.material.color = (isActive) ? m_activeColor : m_inactiveColor;
        }
    }

    private void SetIconVisible(bool isVisible)
    {
        m_iconRenderer.enabled = isVisible;
    }
}
