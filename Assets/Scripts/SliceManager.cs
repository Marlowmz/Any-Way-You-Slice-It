using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class SliceManager : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer preview_sprite;
    Ingredient hovered_ingredient;

    [SerializeField]
    private TriggerEventEmitter triggerEnterArea;

    [SerializeField]
    private TriggerEventEmitter triggerExitArea;

    private List<Rigidbody2D> colliders = new List<Rigidbody2D>();
    void Start()
    {
        triggerEnterArea.onTriggerEnter.AddListener(IngredientEntered);
        triggerExitArea.onTriggerExit.AddListener(IngredientExited);
    }



    void SlicePoly(Ingredient ing, float min, float max)
    {
        // get renderer bounds
        min = Mathf.Lerp(ing.minPointX, ing.maxPointX, min);
        max = Mathf.Lerp(ing.minPointX, ing.maxPointX, max);

        // draw lines at min and max
        Vector3 minWorldSpace = transform.TransformPoint(new Vector3(min, 0, 0));
        Vector3 maxWorldSpace = transform.TransformPoint(new Vector3(max, 0, 0));
        Debug.DrawLine(minWorldSpace, minWorldSpace + new Vector3(0, 1, 0), Color.blue, 10);
        Debug.DrawLine(maxWorldSpace, maxWorldSpace + new Vector3(0, 1, 0), Color.blue, 10);

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


    void SliceMat(Renderer rend, float min, float max)
    {
        Material mat = rend.material;
        mat.SetVector("_VisibleVector", new Vector4(min, max, 0, 0));
    }

    float GetSlicePosition(Ingredient i, float sliceAt)
    {
        // round slice at to a character position
        // this math is definitely rounding wrong
        if (i.unit == 0)
        {
            i.unit = Mathf.Abs(i.visibleVector.y - i.visibleVector.x) / i.name.Length;
        }
        float unit = i.unit;
        // round sliceAt to the nearest unit
        int characterPosition = Mathf.RoundToInt(sliceAt / unit);
        sliceAt = characterPosition * unit;
        return sliceAt;
    }


    void Slice(Ingredient i, float sliceAt)
    {
        sliceAt = GetSlicePosition(i, sliceAt);

        // if sliceat is not within ingredient visible bounds, error
        if (sliceAt <= i.visibleVector.x || sliceAt >= i.visibleVector.y)
        {
            Debug.LogError("SliceAt is not within ingredient visible bounds");
            return;
        }

        // duplicate the ingredient
        Ingredient newIngredient = Instantiate(i.gameObject, i.gameObject.transform.parent).GetComponent<Ingredient>();
        newIngredient.name = i.name;
        int trueCharacterPosition = Mathf.RoundToInt((sliceAt - i.visibleVector.x) / i.unit);

        i.visibleVector.x = sliceAt;
        newIngredient.visibleVector.y = sliceAt;

        SliceMat(i.render, i.visibleVector.x, i.visibleVector.y);
        SliceMat(newIngredient.render, newIngredient.visibleVector.x, newIngredient.visibleVector.y);

        SlicePoly(i, i.visibleVector.x, i.visibleVector.y);
        SlicePoly(newIngredient, newIngredient.visibleVector.x, newIngredient.visibleVector.y);

        i.name = i.name.Substring(trueCharacterPosition);
        newIngredient.name = newIngredient.name.Substring(0, trueCharacterPosition);

        i.RefreshText();
        newIngredient.text = null;
        newIngredient.CreateText();

        i.showText = true;
        newIngredient.showText = true;
    }
    void FixedUpdate()
    {
        foreach (Rigidbody2D c in colliders)
        {
            //slerp rotation to zero

            if (Mathf.Abs(c.rotation) < 0.05 && Mathf.Abs(c.angularVelocity) != 0f)
            { //if it has gotten sufficiently close to zero and is still spinning, stop it
                c.angularVelocity = 0;
                c.rotation = 0;
            }
            else
            {
                //slow down rotation
                c.angularVelocity *= 0.9f;
                c.rotation *= 0.9f;
            }
            //if it's moving, slow it down
            if (c.velocity.magnitude > 0.1f)
            {
                c.velocity *= 0.5f;
            }
            else
            {
                //if it's moving slightly, stop it
                if (c.velocity.magnitude != 0) c.velocity = Vector2.zero;
            }
        }
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {

            if (Input.GetMouseButtonDown(1))
            {
                Ingredient found_ingredient = hit.collider.GetComponent<Ingredient>();
                if (found_ingredient != null)
                {
                    {
                        Vector3 localSpace = found_ingredient.transform.InverseTransformPoint(mousePos);
                        float sliceAt = Mathf.InverseLerp(found_ingredient.minPointX, found_ingredient.maxPointX, localSpace.x);
                        Slice(found_ingredient, sliceAt);
                    }
                }
            }
            else if (hit.transform.tag == "Ingredient")
            {
                if (!hovered_ingredient)
                {
                    hovered_ingredient = hit.collider.GetComponent<Ingredient>();
                    hovered_ingredient.showText = true;
                } else if (hovered_ingredient.gameObject != hit.collider.gameObject) {
                    hovered_ingredient.showText = false;
                    hovered_ingredient = hit.collider.GetComponent<Ingredient>();
                    hovered_ingredient.showText = true;
                }
                else
                {
                    mousePos.z = 0;
                    float total_width = MathF.Abs(hovered_ingredient.maxPointX - hovered_ingredient.minPointX) * MathF.Abs(hovered_ingredient.visibleVector.y - hovered_ingredient.visibleVector.x) * hovered_ingredient.transform.lossyScale.x;
                    Vector3 start_pos_local = new Vector3(hovered_ingredient.minPointX +(MathF.Abs(hovered_ingredient.maxPointX - hovered_ingredient.minPointX) * hovered_ingredient.visibleVector.x), 0, 0);
                    Vector3 start_pos_world = hovered_ingredient.transform.TransformPoint(start_pos_local);
                    float mouse_diff = mousePos.x - start_pos_world.x;
                    int name_length = hovered_ingredient.name.Length;

                    float world_unit_x = total_width / name_length;
                    // round mouse to nearest character position
                    int character_position = Mathf.RoundToInt(mouse_diff / world_unit_x);
                    Debug.Log(character_position);
                    // no edge case
                    if (character_position <= 0 || character_position == name_length) {
                        return;
                    }
                    float character_position_world = character_position * world_unit_x;
                    
                    Vector3 preview_pos_world = new Vector3(start_pos_world.x + character_position_world, start_pos_world.y, start_pos_world.z);
                    
                    preview_sprite.transform.position = Vector3.Lerp(preview_sprite.transform.position, preview_pos_world, 0.1f);
                    preview_sprite.gameObject.SetActive(true);

                    // draw debug line at each character position
                    for (int i = 0; i < name_length; i++) {
                        Vector3 character_pos_world = new Vector3(start_pos_world.x + i * world_unit_x, start_pos_world.y, start_pos_world.z);
                        Debug.DrawLine(character_pos_world, character_pos_world + new Vector3(0, 1, 0), Color.green, 0.01f);
                    }
                    // draw debug line at start pos world
                    Debug.DrawLine(start_pos_world, start_pos_world + new Vector3(0, 1, 0), Color.red, 0.01f);
                }
            }
        }     
        else {
            if (hovered_ingredient) {
                hovered_ingredient.showText = false;
                hovered_ingredient = null;
            }
            preview_sprite.gameObject.SetActive(false);
        }
    }

    private void IngredientEntered(Collider2D c)
    {
        Debug.Log("Ingredient entered");
        Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("No Rigidbody2D found on object");
            return;
        }

        //make kinematic, rotate to zero
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        c.GetComponent<Ingredient>().inWorkArea = true;

        bool alreadyIn = colliders.Contains(rb);
        if (alreadyIn)
        {
            Debug.Log("Already in colliders");
            return;
        }
        colliders.Add(rb);

    }

    private void IngredientExited(Collider2D c)
    {
        Debug.Log("Ingredient exited");
        Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("No Rigidbody2D found on object");
            return;
        }
        c.GetComponent<Ingredient>().inWorkArea = false;
        //make dynamic
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        colliders.Remove(rb);
        Debug.Log(colliders.Count);
    }

}
