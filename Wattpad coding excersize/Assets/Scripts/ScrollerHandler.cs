using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerHandler : MonoBehaviour
{
    //inspector exposed varibules
    public GameObject ScrollerListParent;
    public GameObject PlatePrefab;
    public GameObject LoadingScreen;

    private bool isLoadingIndicatorActive = false;
    private List<GameObject> Scrollables;

    public void AddNewScrollerPlate(StoryMetaData plateToAdd) 
    {
        //allows duplicates to be added
        GameObject newStoryPlate = GameObject.Instantiate(PlatePrefab, ScrollerListParent.transform);
        Scrollables.Add(newStoryPlate);

        PlateObject storyPlatePlateObjectComponent = newStoryPlate.GetComponent<PlateObject>();
        storyPlatePlateObjectComponent.SetupPlate(plateToAdd);
        
    }

    public void ToggleLoadingIndicator() 
    {
        isLoadingIndicatorActive = !isLoadingIndicatorActive;
        LoadingScreen?.SetActive(isLoadingIndicatorActive);
    }

    private void Awake()
    {
        //Pre-conditions for startup
        UnityEngine.Assertions.Assert.IsNotNull(ScrollerListParent);
        UnityEngine.Assertions.Assert.IsNotNull(PlatePrefab);
        UnityEngine.Assertions.Assert.IsNotNull(LoadingScreen);

        Scrollables = new List<GameObject>();
        isLoadingIndicatorActive = false;
    } 
}
