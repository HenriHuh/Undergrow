using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Facebook.Unity;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;

    float gameTime;

    //Custom events can have max 10 value pairs
    static Dictionary<string, object> dict = new Dictionary<string, object>();

    public enum EventType
    {
        Custom,
        OpenTask,
        OpenStore,
        Open
    }

    public enum EventName
    {
        task_complete,
        item_use,
        seed_complete,
        seed_fail,
        plant_planted
    }


    private void Start()
    {
        if (instance == null)
        {
            FB.Init(InitCallback, OnHideUnity);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        gameTime += Time.unscaledDeltaTime;
    }

    public static void SendEvent(EventType type, EventName name)
    {
        dict.Clear();
        switch (type)
        {
            case EventType.Custom:
                switch (name)
                {
                    case EventName.task_complete:
                        dict["task_complete"] = GVar.completedTasks.Count;
                        dict["player_level"] = GVar.completedGoals.Count;
                        Analytics.CustomEvent("completed_tasks", dict);
                        break;
                    case EventName.item_use:
                        dict["items_used"] = GVar.itemsUsed;
                        Analytics.CustomEvent("items_used_ses", dict);
                        break;
                    case EventName.seed_complete:
                        dict["seed_complete"] = GVar.seedFinished;
                        Analytics.CustomEvent("seed_planted_ses", dict);
                        break;
                    case EventName.seed_fail:
                        dict["seed_fail"] = GVar.seedFinished;
                        break;
                    case EventName.plant_planted:
                        break;
                    default:
                        break;
                }
                break;
            case EventType.OpenTask:
                break;
            case EventType.OpenStore:
                break;
            case EventType.Open:
                break;
            default:
                break;
        }
    }

    public static void SendEvent(EventType type, EventName name, object obj)
    {
        dict.Clear();
        switch (type)
        {
            case EventType.Custom:
                switch (name)
                {
                    case EventName.task_complete:
                        dict["task_complete"] = GVar.completedTasks.Count;
                        dict["player_level"] = GVar.completedGoals.Count;
                        Analytics.CustomEvent("completed_tasks", dict);
                        break;
                    case EventName.item_use:
                        dict["items_used"] = GVar.itemsUsed;
                        Analytics.CustomEvent("items_used_ses", dict);
                        break;
                    case EventName.seed_complete:
                        dict["seed_complete"] = obj;
                        Analytics.CustomEvent("seed_planted_ses", dict);
                        break;
                    case EventName.seed_fail:
                        dict["seed_fail"] = obj;
                        break;
                    case EventName.plant_planted:
                        dict["seed_index"] = obj;
                        Analytics.CustomEvent("seeds_planted", dict);
                        break;
                    default:
                        break;
                }
                break;
            case EventType.OpenTask:
                break;
            case EventType.OpenStore:
                break;
            case EventType.Open:
                break;
            default:
                break;
        }
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void LoginStatusCallback(ILoginStatusResult result)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error: " + result.Error);
        }
        else if (result.Failed)
        {
            Debug.Log("Failure: Access Token could not be retrieved");
        }
        else
        {
            // Successfully logged user in
            // A popup notification will appear that says "Logged in as <User Name>"
            Debug.Log("Success: " + result.AccessToken.UserId);
        }
    }
}