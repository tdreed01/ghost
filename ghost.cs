using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 50f;

    public Transform ground;

    public Transform face;
    public Transform rightHand;
    public Transform leftHand;

    public Material trailMaterial;
    public Material rightHandMaterial;
    public Material leftHandMaterial;

    private Rigidbody rb;
    private Collider groundCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundCollider = ground.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMovement();

        Vector3 closestOnGround = groundCollider.ClosestPointOnBounds(transform.position);
        LeaveTrail(closestOnGround, .1f, trailMaterial);
    }

    private void ProcessMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * Time.deltaTime * moveSpeed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * Time.deltaTime * moveSpeed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotateSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(transform.up * Time.deltaTime * rotateSpeed);
        }
    }

    /// <summary>
    /// Called when the ghost collides with a solid object
    /// </summary>
    /// <param name="collision">The collision information</param>
    private void OnCollisionStay(Collision collision)
    {
        // Make an empty list to hold contact points
        ContactPoint[] contacts = new ContactPoint[10];

        // Get the contact points for this collision
        int numContacts = collision.GetContacts(contacts);

        // Iterate through each contact point
        for (int i = 0; i < numContacts; i++)
        {
            // Test the distance from the contact point to the right hand
            if (Vector3.Distance(contacts[i].point, rightHand.position) < .2f)
            {
                LeaveTrail(contacts[i].point, 0.1f, rightHandMaterial);
            }

            // Test the distance from the contact point to the left hand
            if (Vector3.Distance(contacts[i].point, leftHand.position) < .2f)
            {
                LeaveTrail(contacts[i].point, 0.1f, leftHandMaterial);
            }
        }
    }

    /// <summary>
    /// Called when the ghost is inside a trigger
    /// </summary>
    /// <param name="other">The trigger collider of the other object</param>
    private void OnTriggerStay(Collider other)
    {
        // Find the closest point on the collider to the left hand
        Vector3 closestToLeftHand = other.ClosestPoint(leftHand.position);
        if (Vector3.Distance(closestToLeftHand, leftHand.position) < .03f)
        {
            LeaveTrail(closestToLeftHand, 0.1f, leftHandMaterial);
        }

        // Find the closest point on the collider to the right hand
        Vector3 closestToRightHand = other.ClosestPoint(rightHand.position);
        if (Vector3.Distance(closestToRightHand, rightHand.position) < .03f)
        {
            LeaveTrail(closestToRightHand, 0.1f, rightHandMaterial);
        }
    }

    /// <summary>
    /// Places a single sphere at a specific point in space, and sets it to auto-destroy
    /// </summary>
    /// <param name="point">The world point at which to spawn the sphere</param>
    /// <param name="scale">The local scale of the sphere</param>
    /// <param name="material">The material to apply to the sphere</param>
    private void LeaveTrail(Vector3 point, float scale, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * scale;
        sphere.transform.position = point;
        sphere.transform.parent = transform.parent;
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().material = material;
        Destroy(sphere, 10f);
    }
}