using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottleLayoutGroup : MonoBehaviour
{

    public int bottlesToAdd = 5;
    public GameObject bottlePrefab;
    private HorizontalLayoutGroup[] subgroups;
    private VerticalLayoutGroup vGroup;
    public float spacing = 20f;
    [Header("Padding")]
    public int leftPadding;
    public int rightPadding;
    public int topPadding;
    public int bottomPadding;
    // Start is called before the first frame update
    void Start()
    {
        vGroup = GetComponent<VerticalLayoutGroup>();
        vGroup.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
        subgroups = GetComponentsInChildren<HorizontalLayoutGroup>();
        var secondGroupBottleNum = bottlesToAdd / 2;
        var firstGroupBottlesNum = bottlesToAdd - secondGroupBottleNum;
        float bottleWidth = (vGroup.GetComponent<RectTransform>().position.x  - leftPadding - rightPadding - spacing * (firstGroupBottlesNum - 1)) / firstGroupBottlesNum;
        float bottleHeight = bottleWidth * 3;
        subgroups[0].spacing = spacing;
        subgroups[1].spacing = spacing;

        for (int i = 0; i < firstGroupBottlesNum; i++)
        {
            GameObject bottle = Instantiate(bottlePrefab, subgroups[0].transform);
            bottle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottleWidth);
            bottle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bottleHeight);
        }

        for (int i = 0; i < secondGroupBottleNum; i++)
        {
            GameObject bottle = Instantiate(bottlePrefab, subgroups[1].transform);
            bottle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottleWidth);
            bottle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bottleHeight);
            
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(vGroup.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
