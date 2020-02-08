using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    //public GameObject staticObject;

    public float maxAngularVelocity = 30;
    public float speed = 100;
    public float jumpPower = 300;

    public bool rotateAroundPlayer = true;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    public float mouseSpeed = 10;

    private Vector3 offset;
    private float distToGround;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        offset = camera.transform.position - rb.transform.position;
    }

    public bool IsGrounded() {
        //Collider col = rb.GetComponent<Collider>();
        return Physics.Raycast(transform.position, Vector3.down, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //cursor lock
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //camera stuff
        if (rotateAroundPlayer && Cursor.lockState == CursorLockMode.Locked)
        {
            Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSpeed, Vector3.up);
            //Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * mouseSpeed, -Vector3.right);
            offset = camTurnAngleX * offset;
            //offset = camTurnAngleY * offset;
        }

        camera.transform.position = Vector3.Slerp(camera.transform.position,rb.transform.position + offset, smoothFactor);

        if (rotateAroundPlayer)
        {
            camera.transform.LookAt(rb.transform);
        }

        //normal rolling update
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //rolling shit
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = right * y - forward * x;

        rb.AddTorque(movement * speed);

        //stop rolling
        if (Input.GetKey(KeyCode.LeftShift))
        {
            
        }

        //jumping
        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpPower);
        }
    }
}
