using UnityEngine;

/// <summary>
/// Wraps a GameObject to the opposite side of the screen
/// whenever it travels beyond the screen boundaries
/// </summary>

public class ScreenWrap : MonoBehaviour
{
    void Update()
    {
        WrapAround();
    }

    // Creates a copy of the objects current position.
    private void WrapAround()
    {
        Vector3 wrappedTransformPosition = transform.position;

        // Checks whether the object has moved past the right side
        // or left side of the screen.
        if (wrappedTransformPosition.x > ScreenBounds.screenRight)
        {
            //Moves the object from the right side to the left side.
            wrappedTransformPosition.x = ScreenBounds.screenLeft;
        }
        else if (wrappedTransformPosition.x < ScreenBounds.screenLeft)
        {
            //Moves the object from the left side to the right side.
            wrappedTransformPosition.x = ScreenBounds.screenRight;
        }

        // Checks whether the object has moved past the top
        // or bottom of the screen.
        if (wrappedTransformPosition.y > ScreenBounds.screenTop)
        {
            //Moves the object from the top to the bottom.
            wrappedTransformPosition.y = ScreenBounds.screenBottom;
        }
        else if (wrappedTransformPosition.y < ScreenBounds.screenBottom)
        {
            //Moves the object from the bottom to the top.
            wrappedTransformPosition.y = ScreenBounds.screenTop;
        }

        // Applies the updated position to the GameObject.
        transform.position = wrappedTransformPosition;
    }
}
