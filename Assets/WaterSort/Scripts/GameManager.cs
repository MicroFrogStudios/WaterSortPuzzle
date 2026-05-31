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
    public VictoryScript victoryUI;
    public TMP_Text levelLabel;
    public int StartBottles = 3;
    public int emptyBottles = 1;
    public int maxBottles = 12;
    public bool didUndo = false;
    public bool extraBottle = false;
    private int levelsSolved = 0;
    private const string LEVELS_SOLVED_STR = "LevelsSolved";

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        bottleContainer = GetComponent<BottleContainer>();

        // Load the number of levels solved from persistent data
        levelsSolved = PlayerPrefs.GetInt(LEVELS_SOLVED_STR, 0);
        levelLabel.text = (levelsSolved + 1).ToString();

        levelGenerator.ConfigureLevel(StartBottles, emptyBottles);
        PuzzleController.instance.state = levelGenerator.GenerateNewLevel();

        bottleContainer.InitializeBottleViews();
        PuzzleController.instance.LevelSolved.AddListener(Won);
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
                if (hit.collider.transform.parent.TryGetComponent(out BottleView bottleHit))
                {
                    bottleHit.OnSelected();
                }
            }
        }
    }

    public void Won()
    {
        victoryUI.LevelWonEffects(didUndo, extraBottle);
    }

    public void NextLevel()
    {
        levelsSolved++;
        PlayerPrefs.SetInt(LEVELS_SOLVED_STR, levelsSolved);
        PlayerPrefs.Save();

        levelLabel.text = (levelsSolved + 1).ToString();
        victoryUI.HideEffects();
        PuzzleController.instance.ResetLevel();
        didUndo = false;
        extraBottle = false;
        levelGenerator.ConfigureLevel(
            Mathf.Clamp(StartBottles + levelsSolved / 2, 3, maxBottles),
            emptyBottles
        );

        PuzzleController.instance.state = levelGenerator.GenerateNewLevel();
        bottleContainer.InitializeBottleViews();
    }

    public void UseUndo()
    {
        didUndo = true;
        PuzzleController.instance.undoLastCommand();
    }

    public void AddExtraBottle()
    {
        if (extraBottle)
            return;

        extraBottle = true;

        PuzzleController.instance.AddExtraBottle();
        bottleContainer.InitializeBottleViews();
    }

    public void Reset()
    {
        didUndo = false;

        PuzzleController.instance.ResetLevel();

        if (extraBottle)
        {
            var Bottles = PuzzleController.instance.state.bottles;
            Bottles.RemoveAt(Bottles.Count - 1);
            extraBottle = false;
            bottleContainer.InitializeBottleViews();
        }
    }
}
