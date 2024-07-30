using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;

public class SliceManager : MonoBehaviour
{   
    [SerializeField]
    SpriteRenderer preview_sprite;
    Ingredient hovered_ingredient;

    void SlicePoly(Ingredient ing, float min, float max)
    {
        // get renderer bounds
        min = Mathf.Lerp(ing.minPointX, ing.maxPointX, min);
        max = Mathf.Lerp(ing.minPointX, ing.maxPointX, max);

        // draw lines at min and max
        Vector3 minWorldSpace = transform.TransformPoint(new Vector3(min, 0, 0));
        Vector3 maxWorldSpace = transform.TransformPoint(new Vector3(max, 0, 0));
        Debug.DrawLine(minWorldSpace, minWorldSpace + new Vector3(0, 1, 0), Color.red, 10);
        Debug.DrawLine(maxWorldSpace, maxWorldSpace + new Vector3(0, 1, 0), Color.red, 10);

        Vector2[] originalPoints = ing.polyCollider.points;
        List<Vector2> newPoints = new List<Vector2>();

        for (int i = 0; i < originalPoints.Length; i++)
        {
            Vector2 currentPoint = originalPoints[i];
            Vector2 nextPoint = originalPoints[(i + 1) % originalPoints.Length];

            // If the current point is within the range, add it
            if (currentPoint.x >= min && currentPoint.x <= max)
            {
                newPoints.Add(currentPoint);
            }

            // Check if the line segment intersects with min or max x
            if ((currentPoint.x < min && nextPoint.x > min) || (currentPoint.x > min && nextPoint.x < min))
            {
                float t = (min - currentPoint.x) / (nextPoint.x - currentPoint.x);
                Vector2 intersectionPoint = Vector2.Lerp(currentPoint, nextPoint, t);
                newPoints.Add(intersectionPoint);
            }

            if ((currentPoint.x < max && nextPoint.x > max) || (currentPoint.x > max && nextPoint.x < max))
            {
                float t = (max - currentPoint.x) / (nextPoint.x - currentPoint.x);
                Vector2 intersectionPoint = Vector2.Lerp(currentPoint, nextPoint, t);
                newPoints.Add(intersectionPoint);
            }
        }

        // Ensure the polygon is closed
        if (newPoints.Count > 0 && newPoints[0] != newPoints[newPoints.Count - 1])
        {
            newPoints.Add(newPoints[0]);
        }

        // Update the collider with the new points
        ing.polyCollider.points = newPoints.ToArray();
    }

    
    void SliceMat (Renderer rend, float min, float max) {
        Material mat = rend.material;
        mat.SetVector("_VisibleVector", new Vector4(min, max, 0, 0));
    }

    float GetSlicePosition(Ingredient i, float sliceAt) {
        // round slice at to a character position
        // this math is definitely rounding wrong
        if (i.unit == 0) {
            i.unit = Mathf.Abs(i.visibleVector.y - i.visibleVector.x) / i.name.Length;
        }
        float unit = i.unit;
        // round sliceAt to the nearest unit
        int characterPosition = Mathf.RoundToInt(sliceAt / unit);
        sliceAt = characterPosition * unit;
        return sliceAt;
    }


    void Slice(Ingredient i, float sliceAt) {
        sliceAt = GetSlicePosition(i, sliceAt);

        // if sliceat is not within ingredient visible bounds, error
        if (sliceAt <= i.visibleVector.x || sliceAt >= i.visibleVector.y)
        {
            Debug.LogError("SliceAt is not within ingredient visible bounds");
            return;
        }

        // duplicate the ingredient
        Ingredient newIngredient = Instantiate(i.gameObject).GetComponent<Ingredient>();
        newIngredient.name = i.name;
        int trueCharacterPosition = Mathf.RoundToInt((sliceAt-i.visibleVector.x) / i.unit);

        i.visibleVector.x = sliceAt;
        newIngredient.visibleVector.y = sliceAt;

        SliceMat(i.render, i.visibleVector.x, i.visibleVector.y);
        SliceMat(newIngredient.render, newIngredient.visibleVector.x, newIngredient.visibleVector.y);

        SlicePoly(i, i.visibleVector.x, i.visibleVector.y);
        SlicePoly(newIngredient, newIngredient.visibleVector.x, newIngredient.visibleVector.y);

        i.name = i.name.Substring(trueCharacterPosition);
        newIngredient.name = newIngredient.name.Substring(0, trueCharacterPosition);

        i.RefreshText();
        newIngredient.RefreshText();
    }

    private void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ingredient found_ingredient = hit.collider.GetComponent<Ingredient>();
                if (found_ingredient != null) {
                    {
                        Vector3 localSpace = found_ingredient.transform.InverseTransformPoint(mousePos2D);
                        float sliceAt = Mathf.InverseLerp(found_ingredient.minPointX, found_ingredient.maxPointX, localSpace.x);
                        Slice(found_ingredient, sliceAt);
                    }
                }
            }
            else if (hit.transform.tag == "Ingredient")
            {
                if (!hovered_ingredient) {
                    hovered_ingredient = hit.collider.GetComponent<Ingredient>();
                }
                else {
                    mousePos.z = 0;
                    preview_sprite.transform.position = Vector3.Lerp(preview_sprite.transform.position, mousePos, 0.1f);
                    preview_sprite.gameObject.SetActive(true);
                }
            }
        }     
        else {
            hovered_ingredient = null;
            preview_sprite.gameObject.SetActive(false);
        }  
    }



}
