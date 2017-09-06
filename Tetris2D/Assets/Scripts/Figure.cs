using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour {

    public Transform[] parts;
    private void Awake()
    {
        parts = GetComponentsInChildren<Transform>();
    }
    public bool GenerateRay(Vector2 side)
    {
        for (int i = 1; i < 5; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(parts[i].position, side, 0.5f);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.tag == "FigurePart") continue;
                Debug.DrawLine(parts[i].position, hit.point, Color.red, 10f);
                if (hit.transform.tag == "Figure")
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void UpdateTags()
    {
        for (int i = 1; i < 5; i++)
        {
            if (parts[i].tag == "FigurePart")
            {
                parts[i].tag = "Figure";
            }
        }
    }
    public bool isCanRotate(float angle)
    {
        transform.Rotate(new Vector3(0, 0, angle));
        if (GenerateRay(Vector2.left) || GenerateRay(Vector2.right) || GenerateRay(Vector2.down))
        {
            transform.Rotate(new Vector3(0, 0, -angle));
            return false;
        }
        transform.Rotate(new Vector3(0, 0, -angle));
        return true;
    }
    public void DestroyFigureComponent()
    {
        Destroy(GetComponent<Figure>());
    }

}
