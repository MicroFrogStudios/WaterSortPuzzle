using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    LevelGenerator levelGenerator;
    BottleContainer bottleContainer;

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        bottleContainer = GetComponent<BottleContainer>();
        PuzzleController.instance.state = levelGenerator.GenerateNewLevel();

        bottleContainer.InitializeBottleViews(PuzzleController.instance.state);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast from the mouse position downward
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.down, 100f);

            if (hit.collider != null)
            {
                
                if (hit.collider.transform.parent.TryGetComponent<BottleView>(out BottleView bottleHit))
                {
                    bottleHit.OnSelected();
                }
            }
        }
    }
}
