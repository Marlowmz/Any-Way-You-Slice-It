using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouse : MonoBehaviour
{
    // Start is called before the first frame update
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
            if (hit.collider != null)
            {
                if(hit.collider.GetComponent<ClickEventEmitter>() != null)
                {
                    hit.collider.GetComponent<ClickEventEmitter>().Click();
                }
            }
        }
    }
}
