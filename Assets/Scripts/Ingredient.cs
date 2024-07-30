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
    public float maxPointY = float.MinValue;
    public bool showText = false;

    public void CreateText() {
        if (text) return;
        Debug.Log("Creating text for " + gameObject.name);
        text = Instantiate(textPrefab).GetComponent<TextMeshPro>();
        string name_copy = gameObject.name;
        name_copy = name_copy.Replace(" ", "");
        text.text = name_copy;
        RefreshText();
    }

    void UpdateMaxY() {
        Vector2[] points = polyCollider.GetPath(0);
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y > maxPointY)
            {
                maxPointY = points[i].y;
            }
        }
    }

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
        UpdateMaxY();
        CreateText();
    }

    public void RefreshText() {
        text.text = gameObject.name;
        float offset = (visibleVector.y - (1.0f-visibleVector.x));
        text.rectTransform.sizeDelta = new Vector2(render.bounds.size.x*Mathf.Abs(visibleVector.y - visibleVector.x), text.rectTransform.sizeDelta.y);
        // text.gameObject.transform.position = transform.position + new Vector3(render.bounds.extents.x*offset, render.bounds.extents.y, 0);
        UpdateMaxY();
    }

    private void Update() {

        if (showText) {
            float offset = (visibleVector.y - (1.0f-visibleVector.x));
            float maxPointYWorld = transform.TransformPoint(new Vector3(0, maxPointY, 0)).y;
            text.gameObject.transform.position = transform.position + transform.up * maxPointYWorld + new Vector3(render.bounds.extents.x*offset, render.bounds.extents.y, 0);
            text.alpha = Mathf.Lerp(text.alpha, 1.0f, 0.1f);
        }
        else {
            text.alpha = Mathf.Lerp(text.alpha, 0.0f, 0.1f);
        }

    }
}
