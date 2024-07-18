
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleMouse : MonoBehaviour
{
    // Start is called before the first frame update

    //dragged object
    private GameObject draggedObject;
    //offset relative to the grabbed object, used for establishing pivot point for drag physics
    private Vector2 localOffset;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            Debug.Log(hit.collider);
            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    Debug.Log("No Rigidbody2D found on object");
                    return;
                }

                if (rb.bodyType == RigidbodyType2D.Dynamic)
                {
                    Debug.Log("Kinematic Rigidbody2D found on object");
                    Grab(hit);
                }
                else
                {
                    if (hit.collider.GetComponent<ClickEventEmitter>() != null)
                    {
                        Debug.Log("ClickEventEmitter found on object");
                        hit.collider.GetComponent<ClickEventEmitter>().Click();
                    }
                    else
                    {
                        Debug.Log("No ClickEventEmitter found on object");

                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            DropHeld();
        }
    }
    void FixedUpdate()
    {

        if (draggedObject != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 attractionVector = mousePos2D - (Vector2)draggedObject.transform.position;
            draggedObject.GetComponent<Rigidbody2D>().velocity = attractionVector * 10;
            // draggedObject.transform.position = mousePos2D - (Vector2)draggedObject.transform.TransformVector(localOffset);
        }
    }
    void Grab(RaycastHit2D hit)
    {
        draggedObject = hit.collider.gameObject;
        // draggedObject.GetComponent<Rigidbody2D>().simulated = false;
        localOffset = draggedObject.transform.InverseTransformPoint(hit.point);
    }
    void DropHeld()
    {
        if (draggedObject != null)
        {
            // draggedObject.GetComponent<Rigidbody2D>().simulated = true;
            //zero out forces
            // draggedObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // draggedObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            draggedObject = null;
        }
    }
}
