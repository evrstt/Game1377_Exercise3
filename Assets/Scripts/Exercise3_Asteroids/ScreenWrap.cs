using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    void Update()
    {
        WrapAround();
    }

    private void WrapAround()
    {
        Vector3 wrappedTransformPosition = transform.position;

        if (wrappedTransformPosition.x > ScreenBounds.screenRight)
        {
            wrappedTransformPosition.x = ScreenBounds.screenLeft;
        }
        else if (wrappedTransformPosition.x < ScreenBounds.screenLeft)
        {
            wrappedTransformPosition.x = ScreenBounds.screenRight;
        }

        if (wrappedTransformPosition.y > ScreenBounds.screenTop)
        {
            wrappedTransformPosition.y = ScreenBounds.screenBottom;
        }
        else if (wrappedTransformPosition.y < ScreenBounds.screenBottom)
        {
            wrappedTransformPosition.y = ScreenBounds.screenTop;
        }

        transform.position = wrappedTransformPosition;
    }
}
