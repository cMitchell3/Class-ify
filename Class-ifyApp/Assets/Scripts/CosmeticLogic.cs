using UnityEngine;
using UnityEngine.SceneManagement;

public class CosmeticLogic : MonoBehaviour
{
    public static CosmeticLogic Instance { get; private set; }

    [HideInInspector] public bool topHatEquipped = false;
    [HideInInspector] public bool cowHatEquipped = false;
    [HideInInspector] public bool bucketHatEquipped = false;

    private GameObject topHat;
    private GameObject cowHat;
    private GameObject bucketHat;

    private GameObject player;


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
            LoadCosmetics();
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
        if (topHatEquipped)
        {
            topHat.SetActive(true);
        }
        cowHat.SetActive(cowHatEquipped);
        bucketHat.SetActive(bucketHatEquipped);
    }
}
