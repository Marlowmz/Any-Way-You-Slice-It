
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
/*
**Game Manager (script on camera / GM null):**
Keeps track of time. Keeps track of score. Generates tickets.
Keeps references to tickets. Has public functions for marking tickets as resolved,
removing them from ticket references and toggling them as completed.*/
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<String> orders = new List<String>();
    private string[] possibleOrders = {"burger", "fries", "drink"};
    private List<String> completedOrders = new List<String>();
    [SerializeField]
    private TextMeshProUGUI orderText;
    void Start()
    {
        for(int i = 0; i < possibleOrders.Length; i++)
        {
            AddOrder(possibleOrders[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitOrder(string order)
    {
        if (orders.Contains(order))
        {
            ResolveOrder(order);
        }
    }

    public void ResolveOrder(String order)
    {
        completedOrders.Add(order);
        orders.Remove(order);
        UpdateOrders();
    }

    public void AddOrder(String order)
    {
        orders.Add(order);
        UpdateOrders();
    }

    public void UpdateOrders()
    {
        orderText.text = "";
        foreach (String order in orders)
        {
            orderText.text += order + "\n";
        }
        foreach (String order in completedOrders)
        {
            orderText.text += "<s>" + order + "</s>\n";
        }
    }
}
