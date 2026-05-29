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
        
    }
}
