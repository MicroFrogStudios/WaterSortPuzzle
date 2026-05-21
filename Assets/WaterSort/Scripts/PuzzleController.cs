using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase con la lógica del puzzle, implementa Command Pattern para poder deshacer movimientos, 
/// </summary>
public class PuzzleController
{
    /// <summary>
    /// Interfaz para comandos, realmente no hace falta en este prototipo pues solo tenemos un comando, pero lo incluyo por organizacion y escalabilidad futura.
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    public class PourCommand : ICommand
    {

        private GameState.Bottle origin, target;
        public PourCommand(GameState.Bottle origin, GameState.Bottle target) 
        {
            this.origin = origin;
            this.target = target;
        }
        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
   
}
