﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public Camera camera;
    public PhysicMaterial noBounce;
    private SphereCollider col;
    private Collider cameraCollider;
    private Vector3 fNormal;

    //ui stuff
    public Text timeUI;

    //variables the player sets
    public float maxAngularVelocity = 30;
    public float speed = 100;
    public float arialSpeed = 0.05f;
    public float drag = 0.5f;
    public float jumpPower = 300;
    public float wallVerticalJump = 0.7f;
    public bool respawnEnabled = true;

    public bool rotateAroundPlayer = true;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    [Range(0f, 10.0f)]
    public float mouseSpeed = 6;

    //respawn stuff
    private Vector3 respawnTransform;

    //timer stop watch stuff
    private float timer;
    private bool timing = false;

    //materials
    private PhysicMaterial currentPhysicsMat;

    private int jumpDelay = 5;
    private int jumpTimer;

    private Vector3 offset;
    private Vector3 cameraPosition;
    private Vector3 pastPosition;
    private float distToGround;

    //particles
    private ParticleSystem skid;

    //networking varaibles
    public bool multiplayer = true;
    private bool driveEnabled = true;

    //Network Setup stuff
    public void setup(bool driveEnabled)
    {
        if (multiplayer)
        {
            //this.camera = camera;
            camera.enabled = driveEnabled;
            this.driveEnabled = driveEnabled;
        }
    }

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
        rb.angularVelocity = Vector3.zero;
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
            Cursor.visible = true;
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        //debug shit
        if (Input.GetKey(KeyCode.P))
        {
            print(SceneManager.GetActiveScene().name);
        }

        //respawn
        if (Input.GetKey(KeyCode.R) && respawnEnabled)
        {
            respawn();
        }

        //stop rolling
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("XboxX"))
        {
            rb.angularDrag = drag * 10;
        }
        else
        {
            rb.angularDrag = drag;
        }

        //stop bouncing
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("XboxLT"))
        {
            col.material = noBounce;
        }
        else
        {
            col.material = currentPhysicsMat;
        }

        //jumping
        if ((Input.GetKey(KeyCode.Space) || Input.GetButton("XboxRT")) && IsGrounded() && jumpTimer == 0)
        {
            Vector3 fN = fNormal;
            //fN.x /= 2;
            //fN.z /= 2;
            fN.y += wallVerticalJump;
            
            rb.AddForce(fN * jumpPower);
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
        if (Input.GetKey(KeyCode.Q) || Input.GetButton("XboxLB"))
        {
            rb.AddTorque(Vector3.up * speed);
        }
        //wall running right
        if (Input.GetKey(KeyCode.E) || Input.GetButton("XboxRB"))
        {
            rb.AddTorque(Vector3.up * -speed);
        }
    }

    public void cameraCollided()
    {
        //camera.transform.position = pastPosition;
    }

    private void updateCamera()
    {
        Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSpeed, Vector3.up);
        Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -mouseSpeed, Vector3.right);

        //camera stuff
        if (rotateAroundPlayer && Cursor.lockState == CursorLockMode.Locked)
        {
            //camTurnAngleX.y = camTurnAngleX.normalized.y;
            offset = camTurnAngleX * offset;
            camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);
            //camTurnAngleY.x = 0;
            //camTurnAngleY.z = 0;
            //offset = camTurnAngleY * offset;
            //camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);

            //Quaternion temp = camTurnAngleX * camTurnAngleY;
            //temp.Normalize();


            //offsetY = camTurnAngleY * offset;
        } else
        {
            camera.transform.position = Vector3.Slerp(camera.transform.position, rb.transform.position + offset, smoothFactor);

        }



        RaycastHit hit;
        if (Physics.Raycast(transform.position, -(rb.transform.position - camera.transform.position).normalized, out hit, col.bounds.size.y / 2 + Vector3.Distance(rb.transform.position, camera.transform.position)))
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, rb.transform.position, Vector3.Distance(rb.transform.position, camera.transform.position) - hit.distance + 0.1f);
        }

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

        //print((Mathf.Abs(rb.angularVelocity.x) + Mathf.Abs(rb.angularVelocity.y) + Mathf.Abs(rb.angularVelocity.z) / 3));
        rb.AddTorque(movement * (speed));

        //if in the air then have arial movement
        if (!IsGrounded())
        {
            movement = right * x + forward * y;
            rb.AddForce(movement * speed * arialSpeed);
        }
    }

    void updateParticles()
    {
        Vector3 vTan = rb.angularVelocity * col.radius / 2;
        vTan = new Vector3(Math.Abs(vTan.z), Math.Abs(vTan.y), Math.Abs(vTan.x));
        Vector3 v = new Vector3(Math.Abs(rb.velocity.x), Math.Abs(rb.velocity.y), Math.Abs(rb.velocity.z));

        if (v != vTan)
        {
            print("drifting" + " velocity: " + rb.velocity + " difference" + rb.angularVelocity * col.radius / 2);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        cameraCollider = camera.GetComponent<Collider>();
        offset = camera.transform.position - rb.transform.position;
        jumpTimer = 0;

        currentPhysicsMat = col.material;
        rb.maxAngularVelocity = maxAngularVelocity;
        respawnTransform = rb.position;

        //make sure particles do not play
        skid = GameObject.Find("Skid").GetComponent<ParticleSystem>();
        skid.Stop();
    }

    //main method
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!multiplayer || driveEnabled)
        {
            updateTimer();

            updateCamera();
            updateKeys();

            updateRoll();

            //updateParticles();
        }
    }

    //getters
    public float getTimer()
    {
        return (float)Math.Round(timer * 100f) / 100f;
    }
}
