using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase que almacena los datos de la partida actual, sin lógica, solo almacena info.
/// </summary>
public class GameState
{
    public struct ColorLayer
    {
        public Color color;
        public int count;
    }

    public struct Bottle
    {
        public Stack<ColorLayer> colorLayers;
        public int capacity;

    }

    public List<Bottle> bottles;

    //void test()
    //{
    //    var p1 = new ColorLayer { color = Color.white, count = 1 };
    //}
}
