using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    private Collider col;
    //public GameObject staticObject;

    public float maxAngularVelocity = 30;
    public float speed = 100;
    public float drag = 0.5f;
    public float jumpPower = 300;

    public bool rotateAroundPlayer = true;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    public float mouseSpeed = 10;

    //respawn stuff
    private Vector3 respawnTransform;

    //timer stop watch stuff
    private float timer;
    private bool timing = false;

    //materials
    public PhysicMaterial noBounce;
    private PhysicMaterial currentPhysicsMat;

    private int jumpDelay = 50;
    private int jumpTimer;

    private Vector3 offset;
    private float distToGround;

    //timer shit
    public void startTimer()
    {
        timing = true;
        timer = 0;
        print(timer);
    }

    void updateTimer()
    {
        timer += Time.fixedDeltaTime;
    }

    public void stopTimer()
    {
        timing = false;
        print(timer);
    }

    //respawn method if the person dies or somtin
    public void respawn()
    {
        rb.position = respawnTransform;
        rb.velocity = Vector3.zero;
    }

    public void setRespawn(Vector3 newRespawn)
    {
        respawnTransform = newRespawn;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.maxAngularVelocity = maxAngularVelocity;
        offset = camera.transform.position - rb.transform.position;
        jumpTimer = 0;

        currentPhysicsMat = col.material;

        respawnTransform = rb.position;
    }

    public bool IsGrounded() {
        //Collider col = rb.GetComponent<Collider>();
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.size.y/2 + 0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTimer();

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

        camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);

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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.angularDrag = drag * 10;
        } else
        {
            rb.angularDrag = drag;
        }

        //stop bouncing
        if (Input.GetKey(KeyCode.LeftShift))
        {
            col.material = noBounce;
        }
        else
        {
            col.material = currentPhysicsMat;
        }

        //jumping
        if (Input.GetKey(KeyCode.Space) && IsGrounded() && jumpTimer == 0)
        {
            rb.AddForce(Vector3.up * jumpPower);
            jumpTimer += 1;
        }

        if (jumpTimer != 0)
        {
            jumpTimer += 1;
            if (jumpTimer >= jumpDelay)
            {
                jumpTimer = 0;
            }
        }

        //wall running left
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddTorque(Vector3.up * speed);
        }
        //wall running right
        if (Input.GetKey(KeyCode.E))
        {
            rb.AddTorque(Vector3.up * -speed);
        }
    }

    //getters
    public float getTimer()
    {
        return (float)Math.Round(timer * 100f) / 100f;
    }
}
