using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public Rigidbody playerRB;
    public SimpleFSM healthCheck;
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = proportionalHeight * hoverForce * Vector3.up;
            playerRB.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        if(healthCheck.GetHealth() <= 0)
        {
            hoverForce = 0f;
            hoverHeight = 0f;
        }
    }
}
