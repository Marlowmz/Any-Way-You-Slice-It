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

    public TextMeshPro text;
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
        RefreshText();
    }

    public void RefreshText() {
        text.text = gameObject.name;
        float offset = (visibleVector.y - (1.0f-visibleVector.x));
        text.rectTransform.sizeDelta = new Vector2(render.bounds.size.x*Mathf.Abs(visibleVector.y - visibleVector.x), text.rectTransform.sizeDelta.y);
        text.gameObject.transform.position = transform.position + new Vector3(render.bounds.extents.x*offset, render.bounds.extents.y, 0);
        // adjust margin to new visiblevector
        // text.margin = new Vector4(visibleVector.x, 0, 1.0f-visibleVector.y, 0)*render.bounds.size.x*4.0f;
    }

    private void Update() {
        // original transform logic
        // text.gameObject.transform.position = Vector3.Lerp(text.gameObject.transform.position, transform.position + new Vector3(textOffset, render.bounds.extents.y, 0), 0.1f);
    }
}
