using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventEmitter : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent<Collider2D> onTriggerEnter;
    public UnityEvent<Collider2D> onTriggerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit.Invoke(other);
    }
}