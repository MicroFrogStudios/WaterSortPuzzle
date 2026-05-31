using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScript : MonoBehaviour
{

    public ParticleSystem confetti;
    public Image[] stars;
    public Color earnedStar;
    public Color failedStar;
    public GameObject container;
    public void LevelWonEffects(bool bonus1, bool bonus2)
    {
        stars[1].color = !bonus1 ? earnedStar : failedStar;
        stars[2].color = !bonus2 ? earnedStar : failedStar;
        StartCoroutine(DelayedVictory());
    }

   IEnumerator DelayedVictory()
    {
        yield return new WaitForSeconds(0.5f);
        container.SetActive(true);
        confetti.Play();
    }

    public void HideEffects()
    {
        container.SetActive(false);
    }
    
}
