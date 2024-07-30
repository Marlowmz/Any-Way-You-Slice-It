using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

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
        maxPointY = float.MinValue;
        Vector2[] points = polyCollider.GetPath(0);
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y > maxPointY)
            {
                maxPointY = points[i].y;
            }
        }
    }
    public bool inWorkArea = false;
    private void Awake() {

        if(rb == null) rb = GetComponent<Rigidbody2D>();
        if(polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();
        if(render == null) render = GetComponent<Renderer>();
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
        text.rectTransform.sizeDelta = new Vector2(render.bounds.size.x*Mathf.Abs(visibleVector.y - visibleVector.x), text.rectTransform.sizeDelta.y);
        float maxPointYWorld = transform.lossyScale.y * maxPointY;
        // using visible vector, find center of visible part
        float center = visibleVector.x + (visibleVector.y - visibleVector.x) / 2.0f;
        float centerLocal = Mathf.Lerp(minPointX, maxPointX, center);
        text.gameObject.transform.position = transform.TransformPoint(new Vector3(centerLocal, 0, 0)) + Vector3.up * (maxPointYWorld+0.5f);;
        UpdateMaxY();
    }

    private void Update() {
        
        float alpha_lerp = Time.deltaTime * 5.0f;

        if (showText) {
            float maxPointYWorld = transform.lossyScale.y * maxPointY;
            // using visible vector, find center of visible part
            float center = visibleVector.x + (visibleVector.y - visibleVector.x) / 2.0f;
            float centerLocal = Mathf.Lerp(minPointX, maxPointX, center);
            Vector3 target_pos = transform.TransformPoint(new Vector3(centerLocal, 0, 0)) + Vector3.up * (maxPointYWorld+0.5f);
            text.gameObject.transform.position = Vector3.Lerp(text.gameObject.transform.position, target_pos, 0.1f);
            text.alpha = Mathf.Lerp(text.alpha, 1.0f, alpha_lerp);
        }
        else {
            text.alpha = Mathf.Lerp(text.alpha, 0.0f, alpha_lerp);
        }

    }
}
