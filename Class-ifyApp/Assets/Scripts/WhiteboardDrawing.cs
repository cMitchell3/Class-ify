using UnityEngine;
using UnityEngine.UI;

public class WhiteboardDrawing : MonoBehaviour
{
    public Color drawColor = Color.black;
    public Color previousColor;
    public int brushSize = 4; // Increased for smoother lines
    public float brushSoftness = 0.5f; // Controls the softness of the brush edges

    private Texture2D drawTexture;
    private RectTransform rectTransform;
    private Vector2 lastDrawPosition;
    private Image imageComponent;

    void Start()
    {
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
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);

            // Convert local point to texture coordinates
            int x = Mathf.RoundToInt((localPoint.x / rectTransform.rect.width + 0.5f) * drawTexture.width);
            int y = Mathf.RoundToInt((localPoint.y / rectTransform.rect.height + 0.5f) * drawTexture.height);

            if (x >= 0 && x < drawTexture.width && y >= 0 && y < drawTexture.height)
            {
                if (lastDrawPosition == Vector2.zero)
                {
                    lastDrawPosition = new Vector2(x, y);
                }
                DrawLine(lastDrawPosition, new Vector2(x, y));
                lastDrawPosition = new Vector2(x, y);
            }
        }
        else
        {
            lastDrawPosition = Vector2.zero;
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int steps = Mathf.CeilToInt((end - start).magnitude);
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawPoint(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
        }
        drawTexture.Apply();
    }

    void DrawPoint(int x, int y)
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
                    pixelColor = Color.Lerp(pixelColor, drawColor, alpha * drawColor.a);
                    drawTexture.SetPixel(drawX, drawY, pixelColor);
                }
            }
        }
    }

    public void ClearWhiteboard()
    {
        Color[] clearColors = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        drawTexture.SetPixels(clearColors);
        drawTexture.Apply();
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

    public void SetEraser()
    {
        previousColor = drawColor;
        SetDrawColor(Color.white);
    }

    public void SetPencil()
    {
        drawColor = previousColor;
    }
}