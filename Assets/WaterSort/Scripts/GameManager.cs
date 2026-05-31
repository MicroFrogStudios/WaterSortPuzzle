using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    LevelGenerator levelGenerator;
    BottleContainer bottleContainer;

    public Button nextLevelButton;
    public TMP_Text levelLabel;
    public int StartBottles = 3;
    public int emptyBottles = 1;
    public int maxBottles = 12;

    private int levelsSolved = 0;

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        bottleContainer = GetComponent<BottleContainer>();
        levelGenerator.ConfigureLevel(StartBottles, emptyBottles);
        PuzzleController.instance.state = levelGenerator.GenerateNewLevel();

        bottleContainer.InitializeBottleViews(PuzzleController.instance.state);
        PuzzleController.instance.LevelSolved.AddListener(ShowNextLevelButton);
        nextLevelButton.onClick.AddListener(NextLevel);
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
                if (
                    hit.collider
                        .transform
                        .parent
                        .TryGetComponent<BottleView>(out BottleView bottleHit)
                )
                {
                    bottleHit.OnSelected();
                }
            }
        }
    }

    public void ShowNextLevelButton()
    {
        nextLevelButton.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        levelsSolved++;
        levelLabel.text = (levelsSolved + 1).ToString();
        nextLevelButton.gameObject.SetActive(false);
        PuzzleController.instance.ResetLevel();

        levelGenerator.ConfigureLevel(
            Mathf.Clamp(StartBottles + levelsSolved / 2, 3, maxBottles),
            emptyBottles
        );

        PuzzleController.instance.state = levelGenerator.GenerateNewLevel();
        bottleContainer.InitializeBottleViews(PuzzleController.instance.state);
    }
}
