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
    private Button treeButton;
    private Button paintingButton;

    private Image topHatBackground;
    private Image bucketHatBackground;
    private Image cowHatBackground;
    private Image treeBackground;
    private Image paintingBackground;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inventoryMenu;


    void Start()
    {
        cosmeticLogic = GameObject.FindGameObjectWithTag("CosmeticLogic").GetComponent<CosmeticLogic>();

        if (SceneManager.GetActiveScene().name == "InventoryMenu")
        {
            StartCoroutine(SetupButtons());
        }
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            StartCoroutine(SetupDecorButtons());
        }
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

    private IEnumerator SetupDecorButtons()
    {
        // Wait until the buttons exist
        yield return new WaitUntil(() => GameObject.Find("pixelTree") != null);

        treeButton = GameObject.Find("pixelTree").GetComponent<Button>();
        paintingButton = GameObject.Find("pixelWallPainting").GetComponent<Button>();

        treeButton.onClick.AddListener(EquipTree);
        paintingButton.onClick.AddListener(EquipPainting);

        treeBackground = treeButton.GetComponent<Image>();
        paintingBackground = paintingButton.GetComponent<Image>();

        if (cosmeticLogic.treeEquipped)
        {
            treeBackground.color = Color.green;
        }
        if (cosmeticLogic.paintingEquipped)
        {
            paintingBackground.color = Color.green;
        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void BackToPauseMenu()
    {
        inventoryMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void InventoryButton()
    {
        inventoryMenu.SetActive(true);
        pauseMenu.SetActive(false);
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

    public void EquipTree()
    {
        cosmeticLogic.treeEquipped = true;

        treeBackground.color = Color.green;
    }

    public void EquipPainting()
    {
        cosmeticLogic.paintingEquipped = true;

        paintingBackground.color = Color.green;
    }
}
