using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ToolBox
{
    public static bool CheckPos(Vector2 hit, RectTransform rect)
    {
        if(hit.x < rect.transform.position.x - (rect.rect.width / 2))
            return false;
        if (hit.y < rect.transform.position.y - (rect.rect.height / 2))
            return false;

        if (hit.x > rect.transform.position.x + (rect.rect.width / 2))
            return false;
        if (hit.y > rect.transform.position.y + (rect.rect.height / 2))
            return false;


        return true;
    }

    public static bool CheckPos(Vector2 hit, GameObject gameObject)
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(hit);
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                return raycastHit.transform.gameObject == gameObject;
            }
        }
        return false;
    }

    public static bool CheckPos(Vector2 hit, Transform transform)
    {
        if (transform is RectTransform)
            return CheckPos(hit, transform as RectTransform);

        if (transform is Transform)
            return CheckPos(hit, transform.gameObject);

        Debug.LogError("Check Pos Failed");
        return false;
    }
}
