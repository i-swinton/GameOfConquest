using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour
{
    Canvas canvas;

    // The transfomr being moved
    [SerializeField] Transform targetTransform;

    Vector2 lastPosition;

    public System.Action<Vector2, Transform, Vector2> OnDragEnd;

    private void Start()
    {
        if(canvas==null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        lastPosition = targetTransform.position;
    }

    public void DragHandler(BaseEventData data)
    {
        // Cast the event data
        PointerEventData pointerData = (PointerEventData)data;

        // Transform the position of the cursor to be in canvas space
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position);

        // Apply the canvas space position to the object
        targetTransform.position = canvas.transform.TransformPoint(position);

        Debug.Log("Drag Start On Card: " + targetTransform.gameObject.name);

    }

    public void DragEndHandler(BaseEventData data)
    {
        // Get the event data
        PointerEventData pointerData = (PointerEventData)data;

        // Report the last position of the object
        if (OnDragEnd != null)
        {
            OnDragEnd.Invoke(targetTransform.position, targetTransform, lastPosition);
        }

        lastPosition = targetTransform.position;

        Debug.Log("Drag Start On Card: " + targetTransform.gameObject.name);

    }

    public void ForceLastPosition(Vector2 pos)
    {
        lastPosition = pos;
    }
}
