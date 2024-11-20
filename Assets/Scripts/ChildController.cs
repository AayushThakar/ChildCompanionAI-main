using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildController : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        MoveChild();
    }

    void MoveChild()
    {
        // Get input from the keyboard
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Update the child's position
        transform.position += new Vector3(moveX, 0, moveZ);
    }
    // In ChildController.cs
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stranger")) 
        
        {
            DogController.Instance.OnStrangerDetected(other.transform.position);
        }
    }
}