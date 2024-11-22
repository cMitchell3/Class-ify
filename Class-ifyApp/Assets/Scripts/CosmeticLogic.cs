using System.Collections;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CosmeticLogic : MonoBehaviour
{
    public static CosmeticLogic Instance { get; private set; }

    [HideInInspector] public bool topHatEquipped = false;
    [HideInInspector] public bool cowHatEquipped = false;
    [HideInInspector] public bool bucketHatEquipped = false;
    [HideInInspector] public bool treeEquipped = false;
    [HideInInspector] public bool paintingEquipped = false;

    private GameObject topHat;
    private GameObject cowHat;
    private GameObject bucketHat;
    private GameObject tree;
    private GameObject painting;

    private GameObject player;
    private GameObject fileCabinet;


    // Ensure only one instance of this object at all times
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            FindPlayer();
            FindFileCabinetCanvas();
            LoadCosmetics();
            LoadRoomDecor();
            EquipDecor();
            EquipCosmetics();
        }
    }

    private void FindPlayer()
    {
        if (player != null)
        {
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FindFileCabinetCanvas()
    {
        if (fileCabinet != null)
        {
            return;
        }

        fileCabinet = GameObject.FindGameObjectWithTag("FileCabinet");
    }

    private void LoadCosmetics()
    {
        if (bucketHat != null)
        {
            return;
        }
        else if (cowHat != null)
        {
            return;
        }
        else if (topHat != null)
        {
            return;
        }

        bucketHat = player.transform.GetChild(0).gameObject;
        cowHat = player.transform.GetChild(1).gameObject;
        topHat = player.transform.GetChild(2).gameObject;
    }

    private void EquipCosmetics()
    {
        topHat.SetActive(topHatEquipped);
        cowHat.SetActive(cowHatEquipped);
        bucketHat.SetActive(bucketHatEquipped);
    }

    private void LoadRoomDecor()
    {
        if (tree != null)
        {
            return;
        }
        if (painting != null)
        {
            return;
        }

        tree = fileCabinet.transform.GetChild(1).gameObject;
        painting = fileCabinet.transform.GetChild(2).gameObject;
    }

    private void EquipDecor()
    {
        tree.SetActive(treeEquipped);
        painting.SetActive(paintingEquipped);
    }
}
