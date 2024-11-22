using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class LoginRewardsController : MonoBehaviour
{
    public Transform rewardsParent;
    public Button closeButton;

    private UserLoginRewardInfo userLoginRewardInfo;
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

    private Color white;
    private Color imageGray;
    private Color textGray;
    private Color claimedButtonColor;

    async void Start()
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

        this.white = Color.white;

        Color color;
        ColorUtility.TryParseHtmlString("#403333", out color);
        this.imageGray = color;
        ColorUtility.TryParseHtmlString("#505050", out color);
        this.textGray = color;
        ColorUtility.TryParseHtmlString("#848484", out color);
        this.claimedButtonColor = color;

        userEmail = FirebaseAuthManager.Instance.GetUserEmail();

        DateTime now = DateTime.Now;

        this.userLoginRewardInfo = await FirestoreManager.Instance.GetLoginRewardInfo(userEmail);
        if (userLoginRewardInfo == null)
        {
            userLoginRewardInfo = new UserLoginRewardInfo(now, 1, false);
            FirestoreManager.Instance.UpdateUserLoginRewardInfo(userEmail, userLoginRewardInfo);
        }
        else
        {
            DateTime lastUpdated = userLoginRewardInfo.GetLastUpdated();
            DateTime yesterday = now.AddDays(-1);

            if (lastUpdated.Date == yesterday.Date)
            {
                int streakNumber = userLoginRewardInfo.GetStreakNumber();
                if (streakNumber == 7)
                {
                    streakNumber = 1;
                }
                else
                {
                    streakNumber++;
                }
                userLoginRewardInfo.SetAll(now, streakNumber, false);
                FirestoreManager.Instance.UpdateUserLoginRewardInfo(userEmail, userLoginRewardInfo);
            }
            else if (lastUpdated.Date <= yesterday.Date)
            {
                userLoginRewardInfo.SetAll(now, 1, false);
                FirestoreManager.Instance.UpdateUserLoginRewardInfo(userEmail, userLoginRewardInfo);
            }
        }
        
        UpdateRewards();
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    private void UpdateRewards()
    {
        int totalChildren = rewardsParent.childCount;

        for (int i = 0; i < totalChildren; i++)
        {
            GameObject child = rewardsParent.GetChild(i).gameObject;
            
            Button claimButton = child.GetComponentInChildren<Button>(true);
            TextMeshProUGUI rewardText = child.GetComponentInChildren<TextMeshProUGUI>(true);
            Image rewardImage = child.GetComponent<Image>();

            bool isClaimed = userLoginRewardInfo.GetIsClaimed();

            if (claimButton != null && rewardImage != null)
            {
                TextMeshProUGUI claimButtonText = claimButton.GetComponentInChildren<TextMeshProUGUI>();
                Image claimButtonImage = claimButton.GetComponent<Image>();
                claimButton.onClick.AddListener(() => OnClaimButtonClicked(claimButton, claimButtonText, claimButtonImage, rewardText, rewardImage));

                int currentStreakDay = userLoginRewardInfo.GetStreakNumber();

                if (i + 1 == currentStreakDay) //current reward available
                {
                    claimButton.gameObject.SetActive(true);
                    
                    if (isClaimed)
                    {
                        claimButton.interactable = false;
                        claimButtonText.text = "Claimed";
                        claimButtonImage.color = claimedButtonColor;
                        rewardImage.color = imageGray;
                        rewardText.color = textGray;
                    }
                    else
                    {
                        rewardImage.color = white;
                        rewardText.color = white;
                    }
                }
                else if (i + 1 < currentStreakDay) //rewards already passed/claimed
                {
                    rewardImage.color = imageGray;
                    rewardText.color = textGray;
                }
                else //rewards not available yet
                {
                    claimButton.gameObject.SetActive(false);
                    rewardImage.color = white;
                    rewardText.color = white;
                }
            }
        }
    }

    private void OnClaimButtonClicked(Button claimButton, TextMeshProUGUI claimButtonText, Image claimButtonImage, TextMeshProUGUI rewardText, Image rewardImage)
    {
        userLoginRewardInfo.SetLastUpdated(DateTime.Now);
        userLoginRewardInfo.SetIsClaimed(true);
        FirestoreManager.Instance.UpdateUserLoginRewardInfo(userEmail, userLoginRewardInfo);

        claimButton.interactable = false;
        claimButtonText.text = "Claimed";
        claimButtonImage.color = claimedButtonColor;
        rewardImage.color = imageGray;
        rewardText.color = textGray;

        int streakNumber = this.userLoginRewardInfo.GetStreakNumber();
        string reward = "";
        this.rewardDictionary.TryGetValue(streakNumber, out reward);

        if (int.TryParse(reward, out int coins))
        {
            FirestoreManager.Instance.UpdateUserCurrency(userEmail, coins);
        }
        else
        {
            GiveUserCosmeticReward(reward);
        }
    }

    private void GiveUserCosmeticReward(string cosmetic)
    {
        int cosmeticId = 0;
        if (cosmetic.Equals("cowboy hat"))
        {
            cosmeticId = 1;
        }
        
        FirestoreManager.Instance.UpdateUserInventory(userEmail, cosmeticId);
    }
}

public class UserLoginRewardInfo
{
    private DateTime lastUpdated;
    private int streakNumber;
    private bool isClaimed;

    public UserLoginRewardInfo(DateTime lastUpdated, int streakNumber, bool isClaimed)
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

    public void SetAll(DateTime lastUpdated, int streakNumber, bool isClaimed)
    {
        this.lastUpdated = lastUpdated;
        this.streakNumber = streakNumber;
        this.isClaimed = isClaimed;
    }

    public void SetLastUpdated(DateTime lastUpdated)
    {
        this.lastUpdated = lastUpdated;
    }

    public void SetStreakNumber(int streakNumber)
    {
        this.streakNumber = streakNumber;
    }

    public void SetIsClaimed(bool isClaimed)
    {
        this.isClaimed = isClaimed;
    }
}
