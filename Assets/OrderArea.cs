using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderArea : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    private List<GameObject> heldItems = new List<GameObject>();
    //TODO don't stash game objects, stash the actual data

    private string BuildString()
    {
        string result = "";
        foreach (GameObject item in heldItems)
        {
            //TODO parse the actual string from the Item object's fields
            result += item.name;
        }
        return result;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with " + other.gameObject.name);
        if (other.gameObject.tag == "Ingredient")
        {
            heldItems.Add(other.gameObject);
            Debug.Log("Added " + other.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            Debug.Log("Added " + other.gameObject.name);
            heldItems.Remove(other.gameObject);
        }
    }

    public void SubmitOrder()
    {
        string order = BuildString();
        gameManager.SubmitOrder(order);
        Debug.Log(BuildString());
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
