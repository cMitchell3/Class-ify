using UnityEngine;
using UnityEngine.UI;

public class LoginRewardsController : MonoBehaviour
{
    public GameObject[] rewardObjects;
    public int currentDay;

    void Start()
    {
        UpdateRewardStates();
    }

    public void UpdateRewardStates()
    {
        for (int i = 0; i < rewardObjects.Length; i++)
        {
            GameObject reward = rewardObjects[i];
            Image background = reward.GetComponent<Image>();

            if (i < currentDay - 1)
            {
                background.color = Color.gray;
            }
            else if (i == currentDay - 1)
            {
                background.color = Color.yellow;
            }
            else
            {
                background.color = Color.white;
            }
        }
    }
}
