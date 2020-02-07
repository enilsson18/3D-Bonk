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

    public float speed;
    public float jumpPower;
    public float rotationSpeed;
    public float mouseXSpeed;
    public float mouseYSpeed;

    private Vector3 offset;
    private float distToGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        offset = camera.transform.position - rb.transform.position;
    }

    public bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    // Update is called once per frame
    void Update()
    {
        //normal rolling update
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseXSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseYSpeed;

        //rolling shit
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = right * y - forward * x;

        rb.AddTorque(movement * speed);

        //move the camera accordingly
        Vector3 angles = (transform.up * (-1 * Convert.ToInt32(Input.GetKeyDown(KeyCode.LeftArrow)) * rotationSpeed + Convert.ToInt32(Input.GetKeyDown(KeyCode.RightArrow)) * rotationSpeed));

        camera.transform.position = rb.transform.position + offset;
        camera.transform.position = RotatePointAroundPivot(camera.transform.position, rb.transform.position, angles);

        //staticObject.transform.position = rb.transform.position;
        //staticObject.transform.rotation = Quaternion.LookRotation(rb.velocity);

        //jumping
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(transform.up * jumpPower);
        }
    }
}
