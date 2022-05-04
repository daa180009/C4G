using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Class that manages camera movement input and gets raycasts from the camera to determine what objects are hovered over
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// The actual camera being controlled; used for raycasting
    /// </summary>
    public Camera cameraObject;

    /// <summary>
    /// The pause menu to be toggled when pausing
    /// </summary>
    public GameObject pauseMenu;

    /// <summary>
    /// The speed the camera moves at when dragging with the mouse to pan
    /// </summary>
    public float dragPanModifier = .02f;
    /// <summary>
    /// The speed the camera moves at when using the keyboard to pan
    /// </summary>
    public float keyPanModifier = .2f;
    /// <summary>
    /// The speed the camera rotates at
    /// </summary>
    public float rotateModifier = .75f;
    /// <summary>
    /// The speed the camera zoom in and out at
    /// </summary>
    public float zoomModifier = .01f;
    /// <summary>
    /// How far off of 90 degrees each camera rotation should settle at
    /// </summary>
    public float cameraOffset = 0f;

    /// <summary>
    /// The zoom distance that the user wants to go to; uses smooth damp to transition to it smoothly
    /// </summary>
    float goalScrollDistance;
    /// <summary>
    /// Used in the smooth damp function to transition to the desired zoom level
    /// </summary>
    float goalScrollSpeed;

    /// <summary>
    /// The last 3D position of the mouse cursor
    /// </summary>
    Vector3 lastPosition;
    /// <summary>
    /// Whether or not the user's mouse cursor was within the window last frame
    /// </summary>
    bool wasInWindowLast;

    /// <summary>
    /// The last pan direction input from the keyboard
    /// </summary>
    Vector3 keyPan;
    /// <summary>
    /// Whether the user is holding down their pan button for the mouse pan
    /// </summary>
    bool holdingPan;
    /// <summary>
    /// Whenther the user is holding down their rotate button for the mouse rotate
    /// </summary>
    bool holdingRotate;

    void Start()
    {
        goalScrollDistance = transform.position.y;
        pauseMenu.SetActive(false);
    }

    void FixedUpdate()
    {
        ClickDragUpdate();
        
        // not rotating with mouse, so go to nearest 90 degree angle (aligned with squares)
        if(!holdingRotate)
        {
            while (transform.rotation.y < 0)
                transform.Rotate(new Vector3(0, 1f, 0f), 360, Space.World);

            if ((transform.eulerAngles.y - cameraOffset) % 90 >= 45)
                transform.Rotate(new Vector3(0, 1f, 0f), 10, Space.World);
            else if ((transform.eulerAngles.y - cameraOffset) % 90 < 45 && (transform.eulerAngles.y - cameraOffset) % 90 > 10)
                transform.Rotate(new Vector3(0, 1f, 0f), -10, Space.World);
            else if ((transform.eulerAngles.y - cameraOffset) % 90 > 1)
                transform.Rotate(new Vector3(0, 1f, 0f), -1, Space.World);
        }

        // process scroll wheel differences
        transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(transform.position.y, goalScrollDistance, ref goalScrollSpeed, .15f), transform.position.z);

        // move camera with keyboard
        transform.Translate(keyPan * keyPanModifier, Space.Self);

        DetectHover();
    }

    void ClickDragUpdate()
    {
        // if mouse is outside window, dont need to update clicking and dragging functions of mouse
        if(GetMouseOutsideWindow())
        {
            wasInWindowLast = false;
            return;
        }

        // if mouse wasnt in the window last frame, we will have "garbage data" for last position, so just update and use next frame
        if(!wasInWindowLast)
        {
            wasInWindowLast = true;
            lastPosition = GetMousePosition();
            return;
        }

        // if we got this far, we are in the window
        wasInWindowLast = true;

        Vector3 diff = lastPosition - GetMousePosition();
        lastPosition = GetMousePosition();

        // drag the map under the camera with middle mouse button
        if (holdingPan)
            transform.Translate(diff * dragPanModifier, Space.Self);

        // rotate the map around camera with right mouse button
        if (holdingRotate)
            transform.Rotate(new Vector3(0, 1f, 0f), diff.x * rotateModifier, Space.World);

        // zoom up and down with scroll wheel
        goalScrollDistance += -Mouse.current.scroll.ReadValue().y * zoomModifier;

        lastPosition = GetMousePosition();
    }

    void DetectHover()
    {
        RaycastHit hit;
        Ray ray = cameraObject.ScreenPointToRay(Mouse.current.position.ReadValue());

        LayerMask mask = LayerMask.GetMask("Tile");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            TileController tileHovered = hit.transform.parent.GetComponent<TileController>();
            tileHovered?.Hover();
        }

        mask = LayerMask.GetMask("Tower");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            TowerController towerHovered = hit.transform.GetComponent<TowerController>(); 
            towerHovered?.Hover();
        }

        mask = LayerMask.GetMask("Enemy");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            EnemyController enemyHovered = hit.transform.GetComponent<EnemyController>();
            enemyHovered?.Hover();
        }
    }

    /// <summary>
    /// Gets the 3D position of the mouse cursor in space, limited to the edges of the screen
    /// I.E., if the user's cursor is outside the screen, it will return the closest point within the screen to their cursor,
    /// so check with GetMouseOutsideWindow() before using for best results
    /// </summary>
    Vector3 GetMousePosition()
    {
        return new Vector3(Mathf.Clamp(Mouse.current.position.ReadValue().x, 60, Screen.width - 60) - 60, 0, Mathf.Clamp(Mouse.current.position.ReadValue().y, 60, Screen.height - 60) - 60);
    }

    /// <summary>
    /// Returns whether the mouse cursor is outside the bounds of the current window
    /// </summary>
    bool GetMouseOutsideWindow()
    {

        return Mouse.current.position.ReadValue().x < 60 || Mouse.current.position.ReadValue().x > Screen.width - 60 || Mouse.current.position.ReadValue().y < 60 || Mouse.current.position.ReadValue().y > Screen.height - 60;
    }

    void OnPan(InputValue value)
    {
        keyPan = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    void OnPanDrag(InputValue value)
    {
        holdingPan = value.isPressed;
    }

    void OnRotateDrag(InputValue value)
    {
        holdingRotate = value.isPressed;
    }

    void OnRotateRight()
    {
        transform.Rotate(new Vector3(0, 1f, 0f), 50, Space.World);
    }

    void OnRotateLeft()
    {
        transform.Rotate(new Vector3(0, 1f, 0f), -50, Space.World);
    }

    void OnMenuOpen()
    {
        if (Time.timeScale == 0)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
