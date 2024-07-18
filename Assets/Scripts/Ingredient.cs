using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ingredient : MonoBehaviour
{
    public Rigidbody2D rb;
    public PolygonCollider2D polyCollider;
    public Renderer render;

    public Vector2 visibleVector = new Vector2(0, 1);

    TextMeshPro text;
    public GameObject textPrefab;
    public float unit;
    public float minPointX = float.MaxValue;
    public float maxPointX = float.MinValue;

    private void Awake() {

        Vector2[] points = polyCollider.GetPath(0);
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].x < minPointX)
            {
                minPointX = points[i].x;
            }
            if (points[i].x > maxPointX)
            {
                maxPointX = points[i].x;
            }
        }

        // why is this necessary?
        text = GetComponentInChildren<TextMeshPro>();
        if (text) return;
        Debug.Log("Creating text for " + gameObject.name);
        text = Instantiate(textPrefab).GetComponent<TextMeshPro>();
        string name_copy = gameObject.name;
        name_copy = name_copy.Replace(" ", "");
        text.text = name_copy;

        // TODO remove this because we dont want rotation
        // only here because it makes debug easier
        text.gameObject.transform.position = transform.position + new Vector3(0, render.bounds.extents.y, 0);
        text.transform.SetParent(gameObject.transform);
        
        text.margin = new Vector4(render.bounds.size.x / 2, 0, render.bounds.size.x / 2, 0); 
        // this is wrong because positive margin decreases text width
    }

    public void RefreshText() {
        // why is this necessary?
        text = GetComponentInChildren<TextMeshPro>();
        text.text = gameObject.name;
        text.gameObject.transform.position = transform.position + new Vector3(0, render.bounds.extents.y, 0);
        // adjust margin to new visiblevector
        text.margin = new Vector4(visibleVector.x, 0, 1.0f-visibleVector.y, 0)*render.bounds.size.x*5.0f;
    }

    private void Update() {
        // original transform logic
        // text.gameObject.transform.position = Vector3.Lerp(text.gameObject.transform.position, transform.position + new Vector3(textOffset, render.bounds.extents.y, 0), 0.1f);
    }
}
