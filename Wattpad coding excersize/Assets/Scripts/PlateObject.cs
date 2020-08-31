using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlateObject : MonoBehaviour
{

    //Inspector exposed varibules 
    public TextMeshProUGUI TitleUIText;
    public TextMeshProUGUI AuthorNameUiText;
    public RawImage BackgroundImage;
    public RawImage AvatarImage;
    public GameObject BackgroundLoadingIndicator;
    public GameObject AvatarLoadingIndicator;

    private StoryMetaData plateData;

    private void Awake()
    {
        //Pre-conditions for startup
        UnityEngine.Assertions.Assert.IsNotNull(TitleUIText);
        UnityEngine.Assertions.Assert.IsNotNull(AuthorNameUiText);
        UnityEngine.Assertions.Assert.IsNotNull(BackgroundImage);
        UnityEngine.Assertions.Assert.IsNotNull(AvatarImage);
        UnityEngine.Assertions.Assert.IsNotNull(BackgroundLoadingIndicator);
        UnityEngine.Assertions.Assert.IsNotNull(AvatarLoadingIndicator);
    }

    public void SetupPlate(StoryMetaData metaData) 
    {
        if (metaData != null)
        {
            UpdatePlateMetaData(metaData);
        }
    }

    /// <summary>
    /// Updates the meta-data that this plate uses for display and forces a refresh of the plates display.
    /// </summary>
    /// <param name="metaData"></param>
    private void UpdatePlateMetaData(StoryMetaData metaData)
    {
        plateData = metaData;
        RefreshPlateDisplayContent();
    }

    private void RefreshPlateDisplayContent() 
    {
        BackgroundLoadingIndicator?.SetActive(true);
        TitleUIText.text = plateData.Title;
        AuthorNameUiText.text = plateData.Author;
        //No caching, downloading new images each time.
        StartCoroutine(UpdateImageDataFromUrl(plateData.CoverUrl, BackgroundImage, BackgroundLoadingIndicator));
        StartCoroutine(UpdateImageDataFromUrl(plateData.HeadshotUrl, AvatarImage, AvatarLoadingIndicator));
    }
    
    private IEnumerator UpdateImageDataFromUrl(string url, RawImage imageDisplayTarget, GameObject LoadingBannerToToggle = null)
    {
        if (imageDisplayTarget == null) 
        {
            yield break;
        }

        UnityWebRequest activeTextureWebRequest = UnityWebRequestTexture.GetTexture(url);
        yield return activeTextureWebRequest.SendWebRequest();

        if (activeTextureWebRequest.isNetworkError)
        {
            Debug.Log(activeTextureWebRequest.error);
            activeTextureWebRequest.Dispose();               
        }
        else
        {
            imageDisplayTarget.texture = DownloadHandlerTexture.GetContent(activeTextureWebRequest);
            LoadingBannerToToggle?.SetActive(false);
            activeTextureWebRequest.Dispose();
        }        
    }
}
