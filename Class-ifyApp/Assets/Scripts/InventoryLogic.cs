using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InventoryLogic : MonoBehaviour
{
    [SerializeField] private GameObject playerCosmeticsMenu;
    [SerializeField] private GameObject roomCosmeticsMenu;
    private CosmeticLogic cosmeticLogic;

    private Button topHatButton;
    private Button bucketHatButton;
    private Button cowHatButton;

    private Image topHatBackground;
    private Image bucketHatBackground;
    private Image cowHatBackground;


    void Start()
    {
        cosmeticLogic = GameObject.FindGameObjectWithTag("CosmeticLogic").GetComponent<CosmeticLogic>();

        StartCoroutine(SetupButtons());
    }

    private IEnumerator SetupButtons()
    {
        // Wait until the buttons exist
        yield return new WaitUntil(() => GameObject.Find("pixelTopHat") != null);

        topHatButton = GameObject.Find("pixelTopHat").GetComponent<Button>();
        bucketHatButton = GameObject.Find("pixelBucketHat").GetComponent<Button>();
        cowHatButton = GameObject.Find("pixelCowboyHat").GetComponent<Button>();

        topHatButton.onClick.AddListener(EquipTopHat);
        bucketHatButton.onClick.AddListener(EquipBucketHat);
        cowHatButton.onClick.AddListener(EquipCowHat);

        topHatBackground = topHatButton.GetComponent<Image>();
        bucketHatBackground = bucketHatButton.GetComponent<Image>();
        cowHatBackground = cowHatButton.GetComponent<Image>();

        if (cosmeticLogic.topHatEquipped)
        {
            topHatBackground.color = Color.green;
        }
        if (cosmeticLogic.bucketHatEquipped)
        {
            bucketHatBackground.color = Color.green;
        }
        if (cosmeticLogic.cowHatEquipped)
        {
            cowHatBackground.color = Color.green;
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void PlayerCosmeticsButton()
    {
        roomCosmeticsMenu.SetActive(false);
        playerCosmeticsMenu.SetActive(true);
    }

    public void RoomCosmeticsButton()
    {
        playerCosmeticsMenu.SetActive(false);
        roomCosmeticsMenu.SetActive(true);
    }

    public void EquipTopHat()
    {
        cosmeticLogic.topHatEquipped = true;

        cosmeticLogic.cowHatEquipped = false;
        cosmeticLogic.bucketHatEquipped = false;

        // Change color on select
        topHatBackground.color = Color.green;
        bucketHatBackground.color = Color.magenta;
        cowHatBackground.color = Color.magenta;
    }

    public void EquipCowHat()
    {
        cosmeticLogic.cowHatEquipped = true;

        cosmeticLogic.bucketHatEquipped = false;
        cosmeticLogic.topHatEquipped = false;

        // Change color on select
        topHatBackground.color = Color.magenta;
        bucketHatBackground.color = Color.magenta;
        cowHatBackground.color = Color.green;
    }

    public void EquipBucketHat()
    {
        cosmeticLogic.bucketHatEquipped = true;

        cosmeticLogic.topHatEquipped = false;
        cosmeticLogic.cowHatEquipped = false;

        // Change color on select
        topHatBackground.color = Color.magenta;
        bucketHatBackground.color = Color.green;
        cowHatBackground.color = Color.magenta;
    }

    public void EquipNothing()
    {
        cosmeticLogic.topHatEquipped = false;
        cosmeticLogic.cowHatEquipped = false;
        cosmeticLogic.bucketHatEquipped = false;

        // Change color on select
        topHatBackground.color = Color.magenta;
        bucketHatBackground.color = Color.magenta;
        cowHatBackground.color = Color.magenta;
    }
}
