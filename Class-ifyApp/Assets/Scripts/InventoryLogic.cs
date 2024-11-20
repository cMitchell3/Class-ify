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
        CosmeticLogic.Instance.topHatEquipped = true;

        cosmeticLogic.cowHatEquipped = false;
        cosmeticLogic.bucketHatEquipped = false;
    }

    public void EquipCowHat()
    {
        cosmeticLogic.cowHatEquipped = true;

        cosmeticLogic.bucketHatEquipped = false;
        cosmeticLogic.topHatEquipped = false;
    }

    public void EquipBucketHat()
    {
        cosmeticLogic.bucketHatEquipped = true;

        cosmeticLogic.topHatEquipped = false;
        cosmeticLogic.cowHatEquipped = false;
    }

    public void EquipNothing()
    {
        cosmeticLogic.topHatEquipped = false;
        cosmeticLogic.cowHatEquipped = false;
        cosmeticLogic.bucketHatEquipped = false;
    }
}
