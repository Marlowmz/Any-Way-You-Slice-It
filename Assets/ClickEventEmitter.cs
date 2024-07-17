using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickEventEmitter : MonoBehaviour
{
    [SerializeField]
    private UnityEvent clickEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        Debug.Log("Click");
        clickEvent.Invoke();
    }
}
