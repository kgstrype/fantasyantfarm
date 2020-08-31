using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using MiniJSON; //3rd party Json parser from the web. See attributions in MiniJSON.cs

public class Main : MonoBehaviour
{
    private const int MaxPages = 10;
    private const string saveDataRootKey = "page_0";
    private const string baseUrl = "https://www.wattpad.com/api/v3/stories?offset=0&limit=10&fields=stories(id,title,cover,user)&filter=new";

    //Inspector Exposed values
    public Text TitleText;
    public Text InformationalText;
    public ScrollerHandler ScrollHandlerRefrence;

    public delegate void OnGotUrlDataDelegate(List<object> storiesData);
    public OnGotUrlDataDelegate OnRetrivedStoriesData = null;

    private UnityWebRequest activeWebRequest;
    private Dictionary<int,StoryMetaData> storiesMetaData = null; //Since we don't need to iterate over this list i'm storing it like this as the endpoint seems to return duplicates occasionally. 
    private string lastUrl = null;
    private int currentPageLoading = 0; 

   
    private void Start()
    {
        //Pre-conditions for startup
        UnityEngine.Assertions.Assert.IsNotNull(TitleText);
        UnityEngine.Assertions.Assert.IsNotNull(InformationalText);
        UnityEngine.Assertions.Assert.IsNotNull(ScrollHandlerRefrence);

        storiesMetaData = new Dictionary<int, StoryMetaData>();
        ScrollHandlerRefrence.ToggleLoadingIndicator();
        TitleText.text = "Wattpad Sample App";
        InformationalText.text = "Downloading more Stories..";
        currentPageLoading = 0;
         
        GetDataFromServer(baseUrl);
    }

    /// <summary>
    /// Fires off a WebRequest to pull the raw json data from the server
    /// </summary>
    private void GetDataFromServer(string url)
    {
        if (currentPageLoading < MaxPages)
        {
            OnRetrivedStoriesData += OnGetStoriesDataFromServer;

            lastUrl = url;
            StartCoroutine(GetURLDataAsynch(url));
        }
    }

    /// <summary>
    /// Coroutine that handles pulling data from the routine. Fires a callback on success or error. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetURLDataAsynch(string url)
    {
        if (activeWebRequest == null)
        {
            activeWebRequest = UnityWebRequest.Get(url);
            yield return activeWebRequest.SendWebRequest();


            if (activeWebRequest.isNetworkError)
            {
                Debug.Log(activeWebRequest.error);
                activeWebRequest.Dispose();
                activeWebRequest = null;

                OnRetrivedStoriesData?.Invoke(null);
            }
            else
            {
                Dictionary<string, object> JsonDataFromRemote = ParseJsonString(activeWebRequest.downloadHandler.text);
               
                if (currentPageLoading == 0)
                {
                    //Handle saving 
                    PlayerPrefs.SetString(saveDataRootKey, activeWebRequest.downloadHandler.text);
                    PlayerPrefs.Save();
                    InformationalText.text = "The App can now be used offline.";
                }

                activeWebRequest.Dispose();
                activeWebRequest = null;

                if (JsonDataFromRemote != null)
                {

                    if (JsonDataFromRemote.ContainsKey("stories"))
                    {
                        OnRetrivedStoriesData?.Invoke((List<object>)JsonDataFromRemote["stories"]);
                        currentPageLoading++; //Only count pages that actually had data. 
                    }

                    OnRetrivedStoriesData -= OnGetStoriesDataFromServer;
                    if (JsonDataFromRemote.ContainsKey("nextUrl"))
                    {
                        string nextURL = (string)JsonDataFromRemote["nextUrl"];
                        if (!string.IsNullOrEmpty(nextURL))
                        {
                            GetDataFromServer(nextURL);
                        }
                    }
                }


            }
        }
    }

    private Dictionary<string, object> ParseJsonString(string rawJson) 
    {
        if (string.IsNullOrEmpty(rawJson))
        {
            Debug.Log(rawJson);
            Debug.LogError("Error: Failed to Parse JSON as dictonary becuase raw json was empty");
            return null;
        }

        //We are using MiniJSON here. See MiniJSON.cs for full attribution. 
        return Json.Deserialize(rawJson) as Dictionary<string, object>;
    }

    private void OnGetStoriesDataFromServer(List<object> storiesData) 
    {
        if (storiesData == null) 
        {
            Debug.Log("Bad Story Data or Network Error");
            if (LoadSaveData())
            {
                SetAppToOfflineMode();
            }
            else 
            {
                InformationalText.text = "Network Error. Please connect to the internet";
            }
            return;
        }

        Dictionary<string, object> singleStoryDataRaw = null;
        for (int i = 0; i < storiesData.Count; i++) 
        {
            singleStoryDataRaw = storiesData[i] as Dictionary<string, object>;
            if (singleStoryDataRaw != null) 
            {
                StoryMetaData metaDataTemp = StoryMetaData.FactorStoryMetaData(singleStoryDataRaw);
                if (metaDataTemp != null && !storiesMetaData.ContainsKey(metaDataTemp.Id))
                {
                    storiesMetaData.Add(metaDataTemp.Id, metaDataTemp);
                    ScrollHandlerRefrence.AddNewScrollerPlate(metaDataTemp);
                }
                else 
                {
                    Debug.Log("Got a duplicate Id for story with title " +metaDataTemp.Title+ " skipping adding it to the display");
                }
            }
        }

        if (currentPageLoading == 0) //We only want to show the loading blocker on initial page load. Beyond that we will allow pop-in of new plates.
        {
            ScrollHandlerRefrence.ToggleLoadingIndicator();
        }
    }
    private bool LoadSaveData()
    {
        if (PlayerPrefs.HasKey(saveDataRootKey))
        {
            Dictionary<string, object> JsonDataFromSaveData = ParseJsonString(PlayerPrefs.GetString(saveDataRootKey));
            if (JsonDataFromSaveData != null && JsonDataFromSaveData.Count > 0)
            {
                if (JsonDataFromSaveData.ContainsKey("stories"))
                {
                    OnGetStoriesDataFromServer((List<object>)JsonDataFromSaveData["stories"]);
                    currentPageLoading++;
                    return true;
                }
            }
        }
        return false;
    }

    private void SetAppToOfflineMode()
    {
        TitleText.text = "Wattpad Sample App (Offline)";
        InformationalText.text = "Viewing Offline Data";
    }
}
