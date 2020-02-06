using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    public GameObject staticObject;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //normal rolling update
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = right*y - forward*x;

        rb.AddTorque(movement * speed);

        Transform temp = rb.transform;

        staticObject.transform.position = rb.transform.position;
        //staticObject.transform.Rotate(Vector3.up, rb.transform.rotation.y);
    }
}
