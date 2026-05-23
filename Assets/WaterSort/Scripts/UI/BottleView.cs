using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BottleView : MonoBehaviour
{
    Animator animator;
    Animation anim;

    BaseBottleState state;
    int index;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        anim = GetComponent<Animation>();
    }
    public void OnSelected()
    {
        anim.Play("selectBottle");

    }

    private void ChangeState(BaseBottleState newState)
    {

    }

    internal abstract class BaseBottleState{

        public BottleView context;
        public abstract void StateClick();

        public BaseBottleState(BottleView context)
        {
            this.context = context;
        }


    }

    internal class IdleBottleState : BaseBottleState
    {
        public IdleBottleState(BottleView context) : base(context)
        {
        }

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
        public SelectedBottleState(BottleView context) : base(context)
        {
        }

        public override void StateClick()
        {
            throw new System.NotImplementedException();
        }
    }

}
