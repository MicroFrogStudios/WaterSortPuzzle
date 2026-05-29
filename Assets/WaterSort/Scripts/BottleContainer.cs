using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the visual representation of bottles in the game.
/// Instantiates and arranges bottle prefabs in two rows that fit on screen.
/// </summary>
public class BottleContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject bottlePrefab;

    [SerializeField]
    private float horizontalSpacing = 0.5f;

    [SerializeField]
    private float verticalSpacing = 1.5f;

    [SerializeField]
    private float padding = 0.5f;

    private List<GameObject> bottleInstances = new List<GameObject>();

    /// <summary>
    /// Initializes bottle views based on the provided game state.
    /// Instantiates bottle prefabs and arranges them in two rows.
    /// </summary>
    /// <param name="state">The current game state containing bottle data</param>
    public void InitializeBottleViews(GameState state)
    {
        if (bottlePrefab == null)
        {
            Debug.LogError("BottlePrefab is not assigned!");
            return;
        }

        if (state == null || state.bottles == null || state.bottles.Count == 0)
        {
            Debug.LogWarning("GameState has no bottles to display");
            return;
        }

        // Clear any existing bottle instances
        ClearBottles();

        int bottleCount = state.bottles.Count;
        int rowOneBottleCount = Mathf.CeilToInt(bottleCount / 2f);
        int rowTwoBottleCount = bottleCount - rowOneBottleCount;
        // Calculate screen dimensions
        Camera mainCamera = Camera.main;
        // float screenHeight = mainCamera.orthographicSize * 2f;
        // float screenWidth = screenHeight * mainCamera.aspect;

        // Get bottle prefab dimensions (assuming it's 0.16 wide based on the prefab)
        SpriteRenderer spriteRenderer = bottlePrefab.GetComponentInChildren<SpriteRenderer>();
        float bottleWidth = spriteRenderer != null ? spriteRenderer.bounds.size.x : 0.16f;
        float bottleHeight = spriteRenderer != null ? spriteRenderer.bounds.size.y : 0.46f;

        float screenWidth =
            bottleWidth * rowOneBottleCount
            + horizontalSpacing * (rowOneBottleCount - 1)
            + 2 * padding;
        float screenHeight = screenWidth / mainCamera.aspect;
        mainCamera.orthographicSize = screenHeight / 2f;

        // Calculate positions to fit bottles on screen
        float totalWidth =
            (rowOneBottleCount * bottleWidth)
            + ((rowOneBottleCount - 1) * horizontalSpacing)
            + (2 * padding);
        float totalHeight = (2 * bottleHeight) + verticalSpacing;

        // Center the arrangement
        float startX = -(screenWidth / 2f);
        float startY = totalHeight / 2f;

        float BottleSpaceInRow1 =
            startX + (rowOneBottleCount - 1 * (screenWidth / rowOneBottleCount)) + bottleWidth / 2f;
        float BottleSpaceInRow2 =
            startX + (rowTwoBottleCount - 1 * (screenWidth / rowTwoBottleCount)) + bottleWidth / 2f;

        // Instantiate and position bottles
        for (int i = 0; i < bottleCount; i++)
        {
            GameObject bottleInstance = Instantiate(bottlePrefab, transform);
            bottleInstances.Add(bottleInstance);

            // Calculate row and column
            int row = i / rowOneBottleCount;
            int column = i % rowOneBottleCount;
            float rowBottleCount = row == 0 ? rowOneBottleCount : rowTwoBottleCount;
            float spaceWidth = row == 0 ? BottleSpaceInRow1 : BottleSpaceInRow2;
            // Calculate position
            // float xPos = startX + (column * (bottleWidth + horizontalSpacing)) + (bottleWidth / 2f);
            float xPos =
                startX
                + (column * (screenWidth / rowOneBottleCount))
                + (bottleWidth / 2f)
                + (screenWidth - spaceWidth) / 2f
                + row * (totalHeight / 2f - bottleHeight / 2f);
            float yPos = startY - (row * (bottleHeight + verticalSpacing));

            bottleInstance.transform.localPosition = new Vector3(xPos, yPos, 0f);
        }
    }

    /// <summary>
    /// Clears all instantiated bottle instances.
    /// </summary>
    public void ClearBottles()
    {
        foreach (GameObject bottle in bottleInstances)
        {
            Destroy(bottle);
        }
        bottleInstances.Clear();
    }
}
