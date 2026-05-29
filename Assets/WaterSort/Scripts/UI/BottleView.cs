using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static GameState;

public class BottleView : MonoBehaviour
{
    Animator animator;
    Animation anim;
    public int index;
    BaseBottleState state;
    public GameObject colorsGO;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        anim = GetComponent<Animation>();
    }

    public void OnSelected()
    {
        anim.Play("selectBottle");
    }

    public void UpdateColorShader()
    {
        Bottle b = PuzzleController.instance.state.bottles[index];
        var colors = b.GetColorsList();

        for (int i = 0; i < 4; i++)
        {
            if (i < colors.Count)
                colorsGO.GetComponent<SpriteRenderer>().material.SetColor($"_Color{i+1}", colors[i]);
            else
                colorsGO.GetComponent<SpriteRenderer>().material.SetColor($"_Color{i + 1}", Color.clear);
        }
        
    }

    private void ChangeState(BaseBottleState newState) { }

    internal abstract class BaseBottleState
    {
        public BottleView context;
        public abstract void StateClick();

        public BaseBottleState(BottleView context)
        {
            this.context = context;
        }
    }

    internal class IdleBottleState : BaseBottleState
    {
        public IdleBottleState(BottleView context)
            : base(context) { }

        public override void StateClick()
        {
            if (PuzzleController.instance.state.bottles[context.index].IsSolved)
                return;

            if (PuzzleController.instance.state.bottles[context.index].IsEmpty)
                return;

            context.ChangeState(new SelectedBottleState(context));
        }
    }

    internal class SelectedBottleState : BaseBottleState
    {
        public SelectedBottleState(BottleView context)
            : base(context) { }

        public override void StateClick()
        {
            throw new System.NotImplementedException();
        }
    }
}
