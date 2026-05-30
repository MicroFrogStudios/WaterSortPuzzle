using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Bottle = GameState.Bottle;

/// <summary>
/// /// Clase con la l�gica del puzzle, implementa Command Pattern para poder deshacer movimientos,
/// </summary>
public class PuzzleController : MonoBehaviour
{
    /// <summary>
    /// Interfaz para comandos, realmente no hace falta en este prototipo pues solo tenemos un comando, pero lo incluyo por organizacion y escalabilidad futura.
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    /// <summary>
    /// Comando para verter agua de una botella a otra, implementa ICommand para poder deshacer el movimiento.
    /// </summary>
    public class PourCommand : ICommand
    {
        int fromIndex,
            toIndex;
        Color color;
        int units;

        public PourCommand(int fromIndex, int toIndex, Color color, int units)
        {
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.color = color;
            this.units = units;
        }

        public void Execute()
        {
            var from = instance.state.bottles[fromIndex];
            var to = instance.state.bottles[toIndex];

            from.RemoveTop(units);
            to.AddTop(units, color);
        }

        public void Undo()
        {
            var from = instance.state.bottles[fromIndex];
            var to = instance.state.bottles[toIndex];

            to.RemoveTop(units);
            from.AddTop(units, color);

            //event invoke in the future?
        }
    }

    public GameState state;
    public Stack<ICommand> commandStack;

    public static PuzzleController instance;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        commandStack = new();

        
       
    }

    public void AddNewCommand(ICommand command)
    {
        commandStack.Push(command);
        command.Execute();
    }

    public void undoLastCommand()
    {
        if (commandStack.Count <= 0)
            return;

        commandStack.Pop().Undo();
        GetComponent<BottleContainer>().UpdateShaders();
    }

    public void ResetLevel()
    {
        while (commandStack.Count > 0)
        {
            undoLastCommand();
        }
        
    }

    public bool TryPour(int fromIndex, int toIndex)
    {
        Bottle from = instance.state.bottles[fromIndex];
        Bottle to = instance.state.bottles[toIndex];
        if (IsValidPour(from, to))
        {
            int units = Mathf.Min(from.TopCount, to.AvailableSpace);

            AddNewCommand(new PourCommand(fromIndex, toIndex, from.TopColor, units));
            return true;
        }
        return false;
    }

    public bool IsValidPour(Bottle from, Bottle to)
    {
        if (to.IsFull)
            return false;

        if (from.IsEmpty)
            return false;

        if (to.IsEmpty)
            return true;

        if (from.colorLayers.Peek().color != to.colorLayers.Peek().color)
            return false;

        if (to.IsSolved)
            return false;

        

        return from.TopColor == to.TopColor;
    }
}
