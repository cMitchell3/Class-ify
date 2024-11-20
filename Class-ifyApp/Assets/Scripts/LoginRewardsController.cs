using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class LoginRewardsController : MonoBehaviour
{
    public Transform rewardsParent;
    public Button closeButton;

    private UserLoginInfo userLoginInfo;
    private string userEmail;

    private Dictionary<int, string> rewardDictionary = new Dictionary<int, string>
    {
        {1, "5"},
        {2, "10"},
        {3, "15"},
        {4, "20"},
        {5, "25"},
        {6, "30"},
        {7, "cowboy hat"},
    };

    void Start()
    {
        if (rewardsParent == null)
        {
            Debug.LogError("RewardsParent is not assigned in the Inspector!");
            return;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();

        //TODO Get current datetime now
        GetLastUpdatedAndDayStreak();
        //TODO compare last updated with now
            //if is today or later, do nothing
            //if was yesterday, update streak value by 1 and update last updated, set is claimed false 
            //if was before yesterday, set streak value to 1 and update last updated, set is claimed false
        //TODO update db
        UpdateRewards();
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    private void UpdateRewards()
    {
        int totalChildren = rewardsParent.childCount;
        Debug.Log(rewardsParent.name);

        for (int i = 0; i < totalChildren; i++)
        {
            GameObject child = rewardsParent.GetChild(i).gameObject;
            TextMeshProUGUI rewardText = child.GetComponentInChildren<TextMeshProUGUI>(true);
            Button claimButton = child.GetComponentInChildren<Button>(true);
            Image rewardImage = child.GetComponent<Image>();

            if (claimButton != null && rewardImage != null)
            {
                claimButton.onClick.AddListener(OnClaimButtonClicked);

                ColorUtility.TryParseHtmlString("#FFFFFF", out Color white);
                ColorUtility.TryParseHtmlString("#403333", out Color imageGray);
                ColorUtility.TryParseHtmlString("#505050", out Color textGray);

                int currentStreakDay = userLoginInfo.GetStreakNumber();

                if (i + 1 == currentStreakDay)
                {
                    claimButton.gameObject.SetActive(true);
                    rewardImage.color = white;
                    rewardText.color = white;
                }
                else if (i + 1 < currentStreakDay)
                {
                    rewardImage.color = imageGray;
                    rewardText.color = textGray;
                }
                else
                {
                    claimButton.gameObject.SetActive(false);
                    rewardImage.color = white;
                    rewardText.color = white;
                }
            }
        }
    }

    private void OnClaimButtonClicked()
    {
        //TODO update isclaimed in db
        int streakNumber = this.userLoginInfo.GetStreakNumber();
        string reward = "";
        this.rewardDictionary.TryGetValue(streakNumber, out reward);
        if (reward.Equals("cowboy"))
        {
            //TODO give cowboy hat
        }
        else
        {
            int coins = Int32.Parse(reward);
            FirestoreManager.Instance.UpdateUserCurrency(userEmail, coins);
        }
    }

    private async void GetLastUpdatedAndDayStreak()
    {
        this.userLoginInfo = await FirestoreManager.Instance.GetLoginLastUpdatedAndStreakNumber(userEmail);
    }
}

public class UserLoginInfo
{
    private DateTime lastUpdated;
    private int streakNumber;
    private bool isClaimed;

    public UserLoginInfo(DateTime lastUpdated, int streakNumber, bool isClaimed)
    {
        this.lastUpdated = lastUpdated;
        this.streakNumber = streakNumber;
        this.isClaimed = isClaimed;
    }

    public DateTime GetLastUpdated()
    {
        return this.lastUpdated;
    }

    public int GetStreakNumber()
    {
        return this.streakNumber;
    }

    public bool GetIsClaimed()
    {
        return this.isClaimed;
    }
}
