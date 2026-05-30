using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase que almacena los datos de la partida actual, solo almacena info.
/// </summary>
///

public class GameState
{
    public class ColorLayer
    {
        public Color color;
        public int count;
    }

    public class Bottle
    {
        public Stack<ColorLayer> colorLayers;
        public int capacity;

        public bool IsFull
        {
            get => CurrentCapacity >= capacity;
        }

        public bool IsEmpty
        {
            get { return colorLayers.Count <= 0; }
        }

        public bool IsSolved
        {
            get { return colorLayers.Count == 1 && colorLayers.Peek().count == capacity; }
        }

        public void RemoveTop(int units)
        {
            var top = colorLayers.Peek();
            top.count -= units;
            if (top.count <= 0)
                colorLayers.Pop();
        }

        public void AddTop(int units, Color color)
        {
            if (IsEmpty)
            {
                colorLayers.Push(new ColorLayer() { color = color, count = units });
                    return;
            }
            var top = colorLayers.Peek();
            if (top.color == color)
            {
                top.count += units;
            }
            else
            {
                colorLayers.Push(new ColorLayer() { color = color, count = units });
            }
        }

        public List<Color> GetColorsList()
        {
            var list = new List<Color>();
            foreach (var layer in colorLayers)
            {
                for (int i = 0; i < layer.count; i++)
                {
                    list.Add(layer.color);
                }
            }
            list.Reverse();
            return list;
        }

        public Color TopColor
        {
            get => colorLayers.Peek().color;
        }
        public int TopCount
        {
            get => colorLayers.Peek().count;
        }
        public int CurrentCapacity
        {
            get
            {
                var currentCapacity = 0;
                foreach (var layer in colorLayers)
                {
                    currentCapacity += layer.count;
                }
                return currentCapacity;
            }
        }
        public int AvailableSpace
        {
            get => capacity - CurrentCapacity;
        }
    }

    public List<Bottle> bottles;
}
