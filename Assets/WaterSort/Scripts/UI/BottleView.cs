using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using static GameState;

public class BottleView : MonoBehaviour
{
    Animator animator;
    public int index;
    BaseBottleState bottleState;
    public GameObject colorsGO;

    public Vector3 orignalPos;
    public bool pouredToFromRight;

    public static BottleView selectedView = null;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        bottleState = new IdleBottleState(this);
    }

    public void OnSelected()
    {
        Debug.Log(bottleState);
        bottleState.StateClick();
    }

    public void UpdateColorShader()
    {
        Bottle b = PuzzleController.instance.state.bottles[index];
        var colors = b.GetColorsList();

        for (int i = 0; i < 4; i++)
        {
            if (i < colors.Count)
                colorsGO
                    .GetComponent<SpriteRenderer>()
                    .material
                    .SetColor($"_Color{i + 1}", colors[i]);
            else
                colorsGO
                    .GetComponent<SpriteRenderer>()
                    .material
                    .SetColor($"_Color{i + 1}", Color.clear);
        }
    }

    private void ChangeState(BaseBottleState newState)
    {
        bottleState = newState;
    }

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

            if (selectedView != null && selectedView != context)
            {
                selectedView.ChangeState(new PouringBottleState(selectedView, context));
                return;
            }

            if (PuzzleController.instance.state.bottles[context.index].IsEmpty)
                return;

            context.ChangeState(new SelectedBottleState(context));
        }
    }

    internal class SelectedBottleState : BaseBottleState
    {
        public SelectedBottleState(BottleView context)
            : base(context)
        {
            context.animator.SetBool("selected", true);
            selectedView = context;
        }

        public override void StateClick()
        {
            context.animator.SetBool("selected", false);
            context.ChangeState(new IdleBottleState(context));
            selectedView = null;
        }
    }

    internal class PouringBottleState : BaseBottleState
    {
        public PouringBottleState(BottleView context, BottleView to)
            : base(context)
        {
            if (PuzzleController.instance.TryPour(context.index, to.index))
            {
                context.StartCoroutine(context.PourSequence(context, to));
                selectedView = null;
                return;
            }

            selectedView = null;
            context.animator.SetBool("selected", false);
        }

        public override void StateClick()
        {
            //not interactable;
        }
    }

    public IEnumerator PourSequence(BottleView bottleFrom, BottleView bottleTo)
    {
        bool pouringFromRight = bottleTo.pouredToFromRight;
        Vector3 targetPos =
            bottleTo.transform.position
            + Vector3.up * .2f
            + Vector3.right * 0.2f * (pouringFromRight ? -1 : 1);
        while (Vector2.Distance(bottleFrom.transform.position, targetPos) > 0.01f)
        {
            bottleFrom.transform.position = Vector2.Lerp(
                bottleFrom.transform.position,
                targetPos,
                .1f
            );
            yield return null;
        }

        if (pouringFromRight)
            bottleFrom.animator.SetTrigger("pourRight");
        else
            bottleFrom.animator.SetTrigger("pourLeft");

        yield return new WaitForSeconds(0.6f);

        bottleFrom.animator.SetBool("selected", false);

        while (Vector2.Distance(bottleFrom.transform.position, bottleFrom.orignalPos) > 0.01f)
        {
            bottleFrom.transform.position = Vector2.Lerp(
                bottleFrom.transform.position,
                bottleFrom.orignalPos,
                .1f
            );
            yield return null;
        }

        bottleFrom.UpdateColorShader();
        bottleTo.UpdateColorShader();
        bottleFrom.ChangeState(new IdleBottleState(bottleFrom));
    }
}
