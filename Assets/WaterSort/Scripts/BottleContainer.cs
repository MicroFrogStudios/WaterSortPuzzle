using System.Collections.Generic;
using UnityEngine;
using static GameState;

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

    public List<GameObject> bottleViewInstances = new List<GameObject>();

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
            (rowOneBottleCount - 1) * (screenWidth / rowOneBottleCount) + bottleWidth;
        // Instantiate and position bottles
        for (int i = 0; i < bottleCount; i++)
        {
            GameObject bottleInstance = Instantiate(bottlePrefab, transform);
            bottleViewInstances.Add(bottleInstance);
            Bottle b = PuzzleController.instance.state.bottles[i];
            BottleView view = bottleInstance.GetComponent<BottleView>();
            view.index = i;
            view.UpdateColorShader();
            // Calculate row and column
            int row = i / rowOneBottleCount;
            int column = i % rowOneBottleCount;
            // Calculate position
            // float xPos = startX + (column * (bottleWidth + horizontalSpacing)) + (bottleWidth / 2f);
            float xPos =
                startX
                + (column * (screenWidth / rowOneBottleCount))
                + (bottleWidth / 2f)
                + (screenWidth - BottleSpaceInRow1) / 2f
                + row * (rowTwoBottleCount < rowOneBottleCount ? 1 : 0) * (screenWidth / rowOneBottleCount) / 2f;
            float yPos = startY - (row * (bottleHeight + verticalSpacing));

            bottleInstance.transform.localPosition = new Vector3(xPos, yPos, 0f);
            view.orignalPos = bottleInstance.transform.localPosition;
            view.pouredToFromRight = column >= rowOneBottleCount / 2;
        }
    }

    public void UpdateShaders()
    {
        foreach (var bv in bottleViewInstances)
        {
            bv.GetComponent<BottleView>().UpdateColorShader();
        }
    }

    /// <summary>
    /// Clears all instantiated bottle instances.
    /// </summary>
    public void ClearBottles()
    {
        foreach (GameObject bottle in bottleViewInstances)
        {
            Destroy(bottle);
        }
        bottleViewInstances.Clear();
    }
}
