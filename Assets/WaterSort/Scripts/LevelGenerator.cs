using System.Collections.Generic;
using UnityEngine;
using Bottle = GameState.Bottle;

/// <summary>
/// Generates new game levels by creating solved states and then scrambling them with random reversed pours.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int minReversePours = 10;
    [SerializeField] private int maxReversePours = 50;
    [SerializeField] private int filledBottles = 4;
    [SerializeField] private int extraEmptyBottles = 1;
    
    private const int bottleCapacity = 4; // Each bottle can hold 4 units

    /// <summary>
    /// Generates a new unsolved game level.
    /// Creates a solved state and mixes it until well randomized.
    /// </summary>
    /// <returns>A new unsolved GameState</returns>
    public GameState GenerateNewLevel()
    {
        GameState state = CreateSolvedState();
        
        // Apply minimum reverse pours first
        for (int i = 0; i < minReversePours; i++)
        {
            ReverseRandomPour(state);
        }

        int pours = 0;
        // Continue mixing until well mixed
        while ( pours < minReversePours || !CheckMixed(state) && pours < maxReversePours)
        {
            ReverseRandomPour(state);
            pours++;
        }
        
        return state;
    }

    /// <summary>
    /// Performs a reversed valid pour on the game state.
    /// Selects a random non-empty bottle to pour from and a random non-full bottle to pour to.
    /// The colors must match or the destination must be empty.
    /// </summary>
    /// <param name="state">The game state to modify</param>
    public void ReverseRandomPour(GameState state)
    {
        if (state.bottles.Count < 2)
            return;

        // Collect all non-empty bottles
        List<int> nonEmptyIndices = new List<int>();
        for (int i = 0; i < state.bottles.Count; i++)
        {
            if (!state.bottles[i].IsEmpty)
                nonEmptyIndices.Add(i);
        }

        if (nonEmptyIndices.Count == 0)
            return;

        // Select random non-empty bottle to pour from
        int fromIndex = nonEmptyIndices[Random.Range(0, nonEmptyIndices.Count)];

        // Collect all non-full bottles excluding the source
        List<int> nonFullIndices = new List<int>();
        for (int i = 0; i < state.bottles.Count; i++)
        {
            if (!state.bottles[i].IsFull && i != fromIndex)
                nonFullIndices.Add(i);
        }

        if (nonFullIndices.Count == 0)
            return;

        // Select random non-full bottle to pour to
        int toIndex = nonFullIndices[Random.Range(0, nonFullIndices.Count)];

        Bottle from = state.bottles[fromIndex];
        Bottle to = state.bottles[toIndex];

        // Check if it's a valid pour (colors must match or destination must be empty)
        if (!IsValidReversePour(from, to))
            return;
        //if (!to.IsEmpty && from.TopColor != to.TopColor)
        //    return;

        // Perform the pour
        int units = 1;
        Color color = from.TopColor;

        from.RemoveTop(units);
        to.AddTop(units, color);
    }

    bool IsValidReversePour(Bottle from, Bottle to)
    {
        bool couldHavePouredOrWillBeEmpty = (from.TopCount > 1) || from.CurrentCapacity == 1;

        if (!couldHavePouredOrWillBeEmpty)
            return false;

        if (to.IsEmpty) 
            return true;

        bool colorIsDifferent = from.TopColor != to.TopColor;

        return colorIsDifferent;
    }

    /// <summary>
    /// Checks if the game state is well mixed.
    /// Returns true if all non-empty bottles have at least 3 different colors.
    /// Empty bottles are considered well mixed.
    /// </summary>
    /// <param name="state">The game state to check</param>
    /// <returns>True if well mixed, false otherwise</returns>
    public bool CheckMixed(GameState state)
    {
        foreach (Bottle bottle in state.bottles)
        {
            // Empty bottles are considered well mixed
            if (bottle.IsEmpty)
                continue;

            // Count different colors (ColorLayers) in the bottle
            int colorCount = bottle.colorLayers.Count;
            
            // A bottle is well mixed if it has at least 3 different colors
            if (colorCount < 3)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a solved game state with the configured parameters.
    /// Each filled bottle contains one color completely filled.
    /// Plus additional empty bottles for player interaction.
    /// </summary>
    /// <returns>A new solved GameState</returns>
    private GameState CreateSolvedState()
    {
        GameState state = new GameState();
        state.bottles = new List<Bottle>();

        // Generate unique colors for each filled bottle
        Color[] colors = new Color[filledBottles];
        for (int i = 0; i < filledBottles; i++)
        {
            colors[i] = GetRandomColor();
        }

        // Create filled bottles (each with one solid color)
        for (int i = 0; i < filledBottles; i++)
        {
            Bottle bottle = new Bottle();
            bottle.capacity = bottleCapacity;
            bottle.colorLayers = new Stack<GameState.ColorLayer>();
            bottle.colorLayers.Push(new GameState.ColorLayer 
            { 
                color = colors[i], 
                count = bottleCapacity 
            });

            state.bottles.Add(bottle);
        }

        // Create empty bottles for mixing space
        for (int i = 0; i < extraEmptyBottles; i++)
        {
            Bottle bottle = new Bottle();
            bottle.capacity = bottleCapacity;
            bottle.colorLayers = new Stack<GameState.ColorLayer>();

            state.bottles.Add(bottle);
        }

        return state;
    }

    /// <summary>
    /// Generates a random color for bottle creation.
    /// </summary>
    /// <returns>A random color with full alpha</returns>
    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}
