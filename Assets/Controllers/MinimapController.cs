using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the minimap UI display and the camera to capture the minimap
/// </summary>
public class MinimapController : MonoBehaviour
{

    /// <summary>
    /// The UI element that is the parent and border of the minimap display image
    /// </summary>
    public Image minimapBorder;

    /// <summary>
    /// The actual UI image that is rendered to by the minimap camera
    /// </summary>
    public RawImage minimapImage;

    /// <summary>
    /// Adjusts minimap display size to fit the number of tiles in the stage data
    /// </summary>
    /// <param name="stageData"></param>
    public void UpdateStageData(StageData stageData)
    {
        // Get the width and height for the minimap display based on number of tiles in stage
        float minimapWidth = stageData.Width * 12;
        float minimapHeight = stageData.Height * 12;

        // Creates a new render texture with this size for the camera to render to
        RenderTexture minimapRenderTexture;
        minimapRenderTexture = new RenderTexture((int)minimapWidth * 20, (int)minimapHeight * 20, 0);
        minimapRenderTexture.Create();
        GetComponent<Camera>().targetTexture = minimapRenderTexture;
        minimapImage.texture = minimapRenderTexture;

        // Adjust the minimap display sizes for the size of the render texture
        minimapBorder.rectTransform.sizeDelta = new Vector2(minimapWidth + 5, minimapHeight + 5);
        minimapImage.rectTransform.sizeDelta = new Vector2(minimapWidth, minimapHeight);

        // Adjust camera settings to fit the stage correctly
        GetComponent<Camera>().orthographicSize = Mathf.Min(minimapWidth, minimapHeight) / 24;
    }
}
