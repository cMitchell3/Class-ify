using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using SFB;
using System.IO;
using Com.CS.Classify;

public class WhiteboardDrawing : MonoBehaviourPun
{
    public Color drawColor = Color.black;
    public Color previousColor = Color.black;
    public int brushSize = 4; // Increased for smoother lines
    public float brushSoftness = 0.5f; // Controls the softness of the brush edges
    private bool isEraserMode = false;
    private Color blueLockColor;
    private Color orangeLockColor;

    private Texture2D drawTexture;
    private RectTransform rectTransform;
    private Vector2 lastDrawPosition;
    private Image imageComponent;
    public GameObject colorPanel;
    public GameObject clearPanel;
    public Button saveButton;
    public Button lockButton;
    private bool isLocked = false;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public GameNetworkManager gameNetworkManager;
    private bool isHost;

    private float updateInterval = 0.1f; // Time interval for updates (in seconds)
    private float timeSinceLastUpdate = 0f;
    private List<Vector2> drawBuffer = new List<Vector2>();

    public bool IsLocked
    {
        get { return isLocked; }
    }

    void Start()
    {
        // Add a listener to the SaveButton
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(DownloadWhiteboard);
        }
        ColorUtility.TryParseHtmlString("#009BFF", out blueLockColor);
        ColorUtility.TryParseHtmlString("#FF7E00", out orangeLockColor);

        if (lockButton != null)
        {
            lockButton.onClick.AddListener(ToggleLock);
        }

        CheckIfUserIsHost();

        rectTransform = GetComponent<RectTransform>();

        imageComponent = GetComponent<Image>();

        // Create a higher resolution texture
        int textureWidth = Mathf.RoundToInt(rectTransform.rect.width * 2);
        int textureHeight = Mathf.RoundToInt(rectTransform.rect.height * 2);
        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Bilinear;

        // Initialize the texture with white color
        Color[] clearColors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        drawTexture.SetPixels(clearColors);
        drawTexture.Apply();

        // Set the sprite of the Image component
        imageComponent.sprite = Sprite.Create(drawTexture, new Rect(0, 0, drawTexture.width, drawTexture.height), Vector2.zero);

        // Load the whiteboard state if available
        LoadWhiteboardFromRoom();

    }

    void Update()
    {
        if (isLocked && isHost == false)
        {
            return;
        }
        timeSinceLastUpdate += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);

            // Convert local point to texture coordinates
            int x = Mathf.RoundToInt((localPoint.x / rectTransform.rect.width + 0.5f) * drawTexture.width);
            int y = Mathf.RoundToInt((localPoint.y / rectTransform.rect.height + 0.5f) * drawTexture.height);

            if (x >= 0 && x < drawTexture.width && y >= 0 && y < drawTexture.height)
            {
                Vector2 currentDrawPosition = new Vector2(x, y);
                if (lastDrawPosition != Vector2.zero)
                {
                    // Interpolate between last and current position
                    int steps = Mathf.CeilToInt((currentDrawPosition - lastDrawPosition).magnitude * 2);
                    for (int i = 0; i <= steps; i++)
                    {
                        float t = i / (float)steps;
                        Vector2 interpolatedPoint = Vector2.Lerp(lastDrawPosition, currentDrawPosition, t);
                        drawBuffer.Add(interpolatedPoint);
                        DrawPoint(Mathf.RoundToInt(interpolatedPoint.x), Mathf.RoundToInt(interpolatedPoint.y), drawColor);
                    }
                }
                else
                {
                    drawBuffer.Add(currentDrawPosition);
                    DrawPoint(x, y, drawColor);
                }

                lastDrawPosition = currentDrawPosition;

                if (timeSinceLastUpdate >= updateInterval && drawBuffer.Count > 0)
                {
                    float[] colorData = { drawColor.r, drawColor.g, drawColor.b, drawColor.a };
                    // Call the ReceiveDrawLine method via RPC
                    photonView.RPC("ReceiveDrawLine", RpcTarget.All, drawBuffer.ToArray(), colorData);
                    drawBuffer.Clear();
                    timeSinceLastUpdate = 0f;
                }
            }
        }
        else if (lastDrawPosition != Vector2.zero)
        {
            if (drawBuffer.Count > 0)
            {
                float[] colorData = { drawColor.r, drawColor.g, drawColor.b, drawColor.a };
                // Call the ReceiveDrawLine method via RPC
                photonView.RPC("ReceiveDrawLine", RpcTarget.All, drawBuffer.ToArray(), colorData);
                drawBuffer.Clear();
            }
            //Save after drawing (lifting up pen)
            SaveWhiteboardToRoom();
            lastDrawPosition = Vector2.zero;
        }
        drawTexture.Apply();
    }

    [PunRPC]
    private void ReceiveDrawLine(Vector2[] points, float[] colorData)
    {
        if (colorData.Length == 4)
        {
            Color receivedColor = new Color(colorData[0], colorData[1], colorData[2], colorData[3]);
            for (int i = 1; i < points.Length; i++)
            {
                DrawLine(points[i - 1], points[i], receivedColor);
            }
        }
        else
        {
            Debug.LogError("Invalid color data received.");
        }
    }

    void DrawLine(Vector2 start, Vector2 end, Color lineColor)
    {
        int steps = Mathf.CeilToInt((end - start).magnitude);
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawPoint(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), lineColor);
        }
        drawTexture.Apply();

    }

    void DrawPoint(int x, int y, Color lineColor)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                float distance = Mathf.Sqrt(i * i + j * j);
                if (distance <= brushSize)
                {
                    int drawX = Mathf.Clamp(x + i, 0, drawTexture.width - 1);
                    int drawY = Mathf.Clamp(y + j, 0, drawTexture.height - 1);

                    // Calculate alpha based on distance from center and brush softness
                    float alpha = Mathf.Clamp01(1 - (distance / brushSize) / brushSoftness);

                    Color pixelColor = drawTexture.GetPixel(drawX, drawY);
                    pixelColor = Color.Lerp(pixelColor, lineColor, alpha * lineColor.a);
                    drawTexture.SetPixel(drawX, drawY, pixelColor);
                }
            }
        }
    }

    public void ClearWhiteboard()
    {
        photonView.RPC("ClearWhiteboardRPC", RpcTarget.All);
        clearPanel.SetActive(!clearPanel.activeSelf);
        
    }

    [PunRPC]
    private void ClearWhiteboardRPC()
    {
        Color[] clearColors = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        drawTexture.SetPixels(clearColors);
        drawTexture.Apply();

        // Save the cleared state
        SaveWhiteboardToRoom();
    }

    public void SaveWhiteboardToRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            byte[] textureData = drawTexture.EncodeToPNG(); // Compress texture
            Hashtable whiteboardData = new Hashtable { { "WhiteboardState", textureData } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(whiteboardData);
            Debug.Log("Whiteboard has been saved");
        }
    }

    private void LoadWhiteboardFromRoom()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WhiteboardState", out object textureData))
        {
            byte[] data = (byte[])textureData;
            Texture2D loadedTexture = new Texture2D(drawTexture.width, drawTexture.height);
            if (loadedTexture.LoadImage(data))
            {
                drawTexture.SetPixels(loadedTexture.GetPixels());
                drawTexture.Apply();
            }
        }
    }

    private void SetDrawColor(Color newColor)
    {
        drawColor = newColor;
    }

    public void SetBrushSize(int newSize)
    {
        brushSize = Mathf.Max(1, newSize);
    }

    public void SetBrushSoftness(float newSoftness)
    {
        brushSoftness = Mathf.Clamp01(newSoftness);
    }

    private bool AreColorsEqual(Color color1, Color color2, float tolerance = 0.01f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance &&
               Mathf.Abs(color1.a - color2.a) < tolerance;
    }

    public void SetEraser()
    {
        if (!isEraserMode)
        {
            previousColor = drawColor; // Save the current drawing color
            SetDrawColor(Color.white); // Switch to eraser color
            isEraserMode = true; // Track that eraser mode is active
        }
    }

    public void SetPencil()
    {
        if (isEraserMode)
        {
            SetDrawColor(previousColor); // Restore the previous drawing color
            isEraserMode = false; // Switch back to pencil mode
        }
    }

    public void ActivateColorPanel()
    {
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void ActivateClearPanel()
    {
        if (isLocked && isHost == false)
        {
            return;
        }
        clearPanel.SetActive(!clearPanel.activeSelf);
    }

    public void OnRedButtonClick()
    {
        isEraserMode = false;
        SetDrawColor(Color.red);
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void OnBlackButtonClick()
    {
        isEraserMode = false;
        SetDrawColor(Color.black);
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void OnGreenButtonClick()
    {
        isEraserMode = false;
        Color green = new Color(0, 0.75f, 0, 1);
        SetDrawColor(green);
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void OnBlueButtonClick()
    {
        isEraserMode = false;
        Color blue = new Color(0, 0, 0.95f, 1);
        SetDrawColor(blue);
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void OnPurpleButtonClick()
    {
        isEraserMode = false;
        Color purple = new Color(0.7f, 0, 0.7f, 1);
        SetDrawColor(purple);
        colorPanel.SetActive(!colorPanel.activeSelf);
    }

    public void DownloadWhiteboard()
    {
        // Open a file save dialog to choose the location and file name
        string path = StandaloneFileBrowser.SaveFilePanel("Save Whiteboard", "", "Whiteboard", "png");

        // If the user selected a location, save the file
        if (!string.IsNullOrEmpty(path))
        {

            // Encode the current whiteboard texture as a PNG file
            byte[] textureData = drawTexture.EncodeToPNG();

            // Write the PNG data to the chosen file
            File.WriteAllBytes(path, textureData);
            Debug.Log("Whiteboard saved to " + path);
        }
    }

    public void ToggleLock()
    {
        isLocked = !isLocked;

        ColorBlock buttonColors = lockButton.colors;

        buttonColors.normalColor = isLocked ? orangeLockColor : blueLockColor;
        lockButton.colors = buttonColors;

        Transform childImageTransform = lockButton.transform.Find("Image");
        if (childImageTransform != null)
        {
            Image childImage = childImageTransform.GetComponent<Image>();
            if (childImage != null)
            {
                // Update the sprite of the child image
                childImage.sprite = isLocked ? lockedSprite : unlockedSprite;
            }
        }
        // Inform all users that the whiteboard has been locked or unlocked
        photonView.RPC("UpdateLockState", RpcTarget.All, isLocked);
    }

    [PunRPC]
    private void UpdateLockState(bool locked)
    {
        isLocked = locked;
    }

    private async void CheckIfUserIsHost()
    {
        if (gameNetworkManager != null)
        {
            // Fetch the host status asynchronously
            isHost = await gameNetworkManager.FetchHost();
        }
    }
}