using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;

public class InventoryDisplayController : MonoBehaviour
{
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    public GameObject inventoryElementPrefab; // Reference to your InventoryElement prefab
    public Transform inventoryGrid; // Reference to your grid layout transform
    public Sprite pixelCowboyHat;
    public Sprite pixelTopHat;
    public Sprite pixelBucketHat;

    private Dictionary<string, Sprite> itemSprites;

    private void Start()
    {
        // Initialize Firebase
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        // Initialize sprite dictionary
        itemSprites = new Dictionary<string, Sprite>
        {
            { "1", pixelCowboyHat },
            { "2", pixelTopHat },
            { "3", pixelBucketHat }
        };

        LoadInventoryFromDatabase();
    }

    private async void LoadInventoryFromDatabase()
    {
        if (user == null)
        {
            Debug.LogWarning("No authenticated user found.");
            return;
        }

        DocumentReference docRef = db.Collection("user").Document(user.Email);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists && snapshot.ContainsField("inventory"))
        {
            string inventoryContent = snapshot.GetValue<string>("inventory");
            string[] inventoryItems = inventoryContent.Split(',');

            PopulateInventory(inventoryItems);
        }
        else
        {
            Debug.LogWarning("No inventory field found for user.");
        }
    }

    private void PopulateInventory(string[] inventoryItems)
    {
        foreach (string itemId in inventoryItems)
        {
            if (itemSprites.TryGetValue(itemId, out Sprite itemSprite))
            {
                // Instantiate the InventoryElement prefab
                GameObject inventoryElement = Instantiate(inventoryElementPrefab, inventoryGrid);
                Transform imageComponent = inventoryElement.transform.Find("ElementIcon");

                // Find the Image component under the button and set the sprite
                Image itemImage = imageComponent.GetComponent<Image>();
                if (imageComponent != null)
                {
                    itemImage.sprite = itemSprite;
                    inventoryElement.name = itemSprite.name;
                }
            }
            else
            {
                Debug.LogWarning($"No sprite found for item ID: {itemId}");
            }
        }
    }
}