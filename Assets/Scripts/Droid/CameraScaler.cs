using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    public float targetAspect = 16f / 9f; // Your gameplay area's target aspect ratio (without counting the UI)
    public float gameplayPortion = 0.6f;  // Percentage of screen used for gameplay (left 60%)

    void Update()
    {
        Camera cam = GetComponent<Camera>();

        float windowAspect = (float)Screen.width / (float)Screen.height;
        float adjustedWindowAspect = (windowAspect * gameplayPortion); // Only the left 60% matters

        float scaleHeight = adjustedWindowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Add letterboxing (bars top and bottom inside the 60%)
            Rect rect = new Rect();
            rect.width = gameplayPortion;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Add pillarboxing (bars inside the 60%)
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = new Rect();
            rect.width = gameplayPortion * scaleWidth;
            rect.height = 1.0f;
            rect.x = (gameplayPortion - rect.width) / 2.0f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }
}