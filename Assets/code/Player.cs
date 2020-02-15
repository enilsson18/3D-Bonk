using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    public PhysicMaterial noBounce;
    private Collider col;
    private Vector3 fNormal;

    //ui stuff
    public Text timeUI;

    //variables the player sets
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
    private PhysicMaterial currentPhysicsMat;

    private int jumpDelay = 50;
    private int jumpTimer;

    private Vector3 offset;
    private float distToGround;

    //timer stuff
    public void startTimer()
    {
        timing = true;
        timer = 0;
        print(timer);
    }

    void updateTimer()
    {
        if (timing)
        {
            timer += Time.fixedDeltaTime;
        }
        //print(timer);
        timeUI.text = "Time: " + getTimer();
        timeUI.SetAllDirty();
    }

    public void stopTimer()
    {
        timing = false;
        print(timer);
    }

    //collision cases
    private void OnCollisionEnter(Collision collision)
    {
        fNormal = collision.contacts[0].normal;
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

    public bool IsGrounded() {
        //Collider col = rb.GetComponent<Collider>();
        return Physics.Raycast(transform.position, -fNormal, col.bounds.size.y/2 + 0.1f);
    }

    private void updateKeys()
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

        //stop rolling
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.angularDrag = drag * 10;
        }
        else
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
            
            rb.AddForce(fNormal * jumpPower);
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

    private void updateCamera()
    {
        //camera stuff
        if (rotateAroundPlayer && Cursor.lockState == CursorLockMode.Locked)
        {
            
            Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSpeed, Vector3.up);
            //Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -mouseSpeed, Vector3.right);
            offset = camTurnAngleX * offset;
        }

        camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);

        if (rotateAroundPlayer)
        {
            camera.transform.LookAt(rb.transform);
        }
    }

    private void updateRoll()
    {
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

    //main method
    // Update is called once per frame
    void FixedUpdate()
    {
        updateTimer();

        updateCamera();
        updateKeys();

        updateRoll();
    }

    //getters
    public float getTimer()
    {
        return (float)Math.Round(timer * 100f) / 100f;
    }
}
