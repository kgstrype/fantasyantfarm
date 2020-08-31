using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryMetaData
{
    //constants
    private const string USER_GROUP_NAME = "user";
    private const string ID_FIELD_NAME = "id";
    private const string TITLE_FIELD_NAME = "title";
    private const string COVER_URL_FIELD_NAME = "cover";
    private const string NAME_FIELD_NAME = "name";
    private const string AVATAR_FIELD_NAME = "avatar";
    
    public string Title;
    public string Author;
    public int Id; 
    public string CoverUrl = null;
    public string HeadshotUrl = null;

    public StoryMetaData(int Id, string Title, string Author, string coverUrl, string headshotUrl) 
    {
        this.Id = Id;
        this.Title = Title;
        this.Author = Author;
        this.CoverUrl = coverUrl;
        this.HeadshotUrl = headshotUrl;
    }

    /// <summary>
    /// Factories raw Json data into a storyMeta data, or returns null if the data contains errors, printing error to log.
    /// </summary>
    /// <param name="storyDataRaw"></param>
    /// <returns></returns>
    public static StoryMetaData FactorStoryMetaData(Dictionary<string, object> storyDataRaw)
    {
        UnityEngine.Assertions.Assert.IsNotNull(storyDataRaw);

        string title;
        string author;
        int id;
        string storyCoverUrl;
        string authorHeadshotURL;

        if (!(storyDataRaw.ContainsKey(ID_FIELD_NAME) && int.TryParse((string)storyDataRaw[ID_FIELD_NAME], out id)))
        {
            Debug.Log("Failed to create StoryMetaData, missing or non-interger ID");
            return null;
        }

        if (!(storyDataRaw.ContainsKey(TITLE_FIELD_NAME) && !string.IsNullOrEmpty((string)storyDataRaw[TITLE_FIELD_NAME])))
        {
            Debug.Log("Failed to create StoryMetaData, missing or non-string Story Title");
            return null;
        }
        else 
        {
            title = (string)storyDataRaw[TITLE_FIELD_NAME];
        }

        if (!(storyDataRaw.ContainsKey(COVER_URL_FIELD_NAME) && !string.IsNullOrEmpty((string)storyDataRaw[COVER_URL_FIELD_NAME])))
        {
            Debug.Log("Failed to create StoryMetaData, missing or non-string story cover Url");
            return null;
        }
        else
        {
            storyCoverUrl = (string)storyDataRaw[COVER_URL_FIELD_NAME];
        }

        //handle the userdata blob
        if (storyDataRaw.ContainsKey(USER_GROUP_NAME))
        {
            Dictionary<string, object> userDataRaw = storyDataRaw[USER_GROUP_NAME] as Dictionary<string, object>;
            if (userDataRaw != null)
            {
                if (!(userDataRaw.ContainsKey(NAME_FIELD_NAME) && !string.IsNullOrEmpty((string)userDataRaw[NAME_FIELD_NAME])))
                {
                    Debug.Log("Failed to create StoryMetaData, missing or non-string author name!");
                    return null;
                }
                else
                {
                    author = (string)userDataRaw[NAME_FIELD_NAME];
                }

                if (!(userDataRaw.ContainsKey(AVATAR_FIELD_NAME) && !string.IsNullOrEmpty((string)userDataRaw[AVATAR_FIELD_NAME])))
                {
                    Debug.Log("Failed to create StoryMetaData, missing or non-string author name!");
                    return null;
                }
                else
                {
                    authorHeadshotURL = (string)userDataRaw[AVATAR_FIELD_NAME];
                }
            }
            else 
            {
                Debug.LogError("Incorrectly Formatted or bad User data");
                return null;
            }
        }
        else
        {
            Debug.Log("Failed to create StoryMetaData, missing user section of data!");
            return null;
        }


        return new StoryMetaData(id,title,author,storyCoverUrl,authorHeadshotURL);
    }
}
