using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections; // تم إضافة هذا للـ Coroutines

public class ShopManager : MonoBehaviour
{
    #region Singleton
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public Sprite image;
        public int price;

        [HideInInspector] public bool isPurchased;
        [HideInInspector] public Button itemButton;
    }

    [Header("Shop Layout")]
    [SerializeField] public List<ShopItem> shopItemsList;
    [SerializeField] private Transform shopScrollView;
    [SerializeField] private GameObject itemTemplate;

    [Header("UI Display")]
    [SerializeField] private TextMeshProUGUI totalStarsDisplay;
    public TMP_FontAsset arabicFont;

    [Header("Shop Panel")]
    [SerializeField] GameObject ShopPanel;

    // --- التعديل الجديد هنا لتأثير الرسالة والاهتزاز ---
    [Header("No Enough Coins Feedback")]
    [SerializeField] private TextMeshProUGUI noCoinsText; // اسحب عنصر الـ No Coins Message هنا
    [SerializeField] private float shakeDuration = 0.4f;   // مدة الاهتزاز
    [SerializeField] private float shakeMagnitude = 6f;   // قوة الاهتزاز
    private Vector3 noCoinsOriginalPos;
    private Coroutine shakeCoroutine;
    // --------------------------------------------------

    private void Start()
    {
        // حفظ الموقع الأصلي للنص وإخفائه في البداية
        if (noCoinsText != null)
        {
            noCoinsOriginalPos = noCoinsText.transform.localPosition;
            noCoinsText.gameObject.SetActive(false);
        }

        if (shopItemsList != null && shopItemsList.Count > 0)
        {
            GameData.SetItemPurchased(shopItemsList[0].itemName, 0, true);
        }

        LoadShop();
        RefreshStarsUI();

        if (Profile.Instance != null)
        {
            Profile.Instance.LoadAvailableAvatars();
        }
    }

    private void LoadShop()
    {
        if (shopScrollView == null || itemTemplate == null)
        {
            Debug.LogError("shopScrollView غير مربوط في Inspector");
            return;
        }

        for (int i = 0; i < shopItemsList.Count; i++)
        {
            CreateShopItemUI(i);
        }
    }

    private void CreateShopItemUI(int index)
    {
        ShopItem item = shopItemsList[index];
        item.isPurchased = GameData.IsItemPurchased(item.itemName, index);

        GameObject itemUI = Instantiate(itemTemplate, shopScrollView);
        itemUI.SetActive(true);

        Image itemImage = itemUI.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI priceText = itemUI.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        Button buyButton = itemUI.transform.GetChild(2).GetComponent<Button>();

        if (itemImage != null)
            itemImage.sprite = item.image;

        if (priceText != null)
            priceText.text = item.price.ToString();

        item.itemButton = buyButton;

        TextMeshProUGUI btnText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        btnText.font = arabicFont;
        buyButton.onClick.RemoveAllListeners();

        if (item.isPurchased)
        {
            buyButton.interactable = false;
            if (btnText != null) btnText.text = "ﻚﻠﺘﻤﻣ";
        }
        else
        {
            buyButton.interactable = true;
            if (btnText != null) btnText.text = "ءاﺮﺷ";
            buyButton.onClick.AddListener(() => OnShopItemBtnClicked(index));
        }
    }

    private void OnShopItemBtnClicked(int itemIndex)
    {
        ShopItem item = shopItemsList[itemIndex];
        int currentStars = GameData.GetTotalStars();

        if (item.isPurchased)
            return;

        if (currentStars >= item.price)
        {
            currentStars -= item.price;
            GameData.SetTotalStars(currentStars);
            GameData.SetItemPurchased(item.itemName, itemIndex, true);

            item.isPurchased = true;

            if (item.itemButton != null)
            {
                item.itemButton.interactable = false;
                TextMeshProUGUI btnText = item.itemButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = "ﻚﻠﺘﻤﻣ";
            }

            RefreshStarsUI();
            RefreshGlobalStarsDisplay();

            if (Profile.Instance != null)
            {
                Profile.Instance.LoadAvailableAvatars();
            }

            Debug.Log("تم شراء: " + item.itemName);
        }
        else
        {
            Debug.Log("النجوم غير كافية للشراء");

            // --- استدعاء تأثير الاهتزاز عند فشل الشراء ---
            TriggerNoCoinsFeedback();
        }
    }

    // --- كود وظيفة الاهتزاز وعرض الرسالة ---
    private void TriggerNoCoinsFeedback()
    {
        if (noCoinsText != null)
        {
            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            shakeCoroutine = StartCoroutine(ShakeTextRoutine());
        }
    }

    IEnumerator ShakeTextRoutine()
    {
        if (noCoinsText != null)
        {
            noCoinsText.color = Color.red;
        }
        noCoinsText.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // توليد إزاحة عشوائية للاهتزاز
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            noCoinsText.transform.localPosition = new Vector3(noCoinsOriginalPos.x + x, noCoinsOriginalPos.y + y, noCoinsOriginalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // إعادة النص لمكانه الأصلي وثباته
        noCoinsText.transform.localPosition = noCoinsOriginalPos;

        // إبقاء الرسالة ظاهرة لمدة ثانية بعد الاهتزاز ثم إخفائها
        yield return new WaitForSeconds(1f);
        noCoinsText.gameObject.SetActive(false);
    }
    // --------------------------------------------------

    private void RefreshStarsUI()
    {
        if (totalStarsDisplay != null)
        {
            totalStarsDisplay.text = GameData.GetTotalStars().ToString();
        }
    }

    private void RefreshGlobalStarsDisplay()
    {
        TotalStarsDisplay displayScript = FindObjectOfType<TotalStarsDisplay>();
        if (displayScript != null)
        {
            displayScript.RefreshDisplay();
        }
    }

    public void OpenShop()
    {
        Debug.Log("تم الضغط على زر المتجر");

        if (ShopPanel != null)
        {
            ShopPanel.SetActive(true);
            RefreshStarsUI();
        }
        else
        {
            Debug.LogError("ShopPanel غير مربوط في Inspector");
        }
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }

    public int GetAvatarCount()
    {
        return shopItemsList.Count;
    }

    public Sprite GetAvatarSprite(int index)
    {
        if (index >= 0 && index < shopItemsList.Count)
        {
            return shopItemsList[index].image;
        }
        return null;
    }

    public bool IsAvatarPurchasedByIndex(int index)
    {
        if (index < 0 || index >= shopItemsList.Count)
            return false;

        return GameData.IsItemPurchased(shopItemsList[index].itemName, index);
    }

    public void RefreshShop()
    {
        foreach (Transform child in shopScrollView)
        {
            if (child.gameObject != itemTemplate)
                Destroy(child.gameObject);
        }

        LoadShop();
    }
}

//using UnityEngine;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine.UI;

//public class ShopManager : MonoBehaviour
//{
//    #region Singleton
//    public static ShopManager Instance;

//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//        else
//            Destroy(gameObject);
//    }
//    #endregion

//    [System.Serializable]
//    public class ShopItem
//    {
//        public string itemName;
//        public Sprite image;
//        public int price;

//        [HideInInspector] public bool isPurchased;
//        [HideInInspector] public Button itemButton;
//    }

//    [Header("Shop Layout")]
//    [SerializeField] public List<ShopItem> shopItemsList;
//    [SerializeField] private Transform shopScrollView;
//    [SerializeField] private GameObject itemTemplate;

//    [Header("UI Display")]
//    [SerializeField] private TextMeshProUGUI totalStarsDisplay;
//    public TMP_FontAsset arabicFont; // تأكد من تعيين هذا في Inspector إلى خط يدعم العربية

//    [Header("Shop Panel")]
//    [SerializeField] GameObject ShopPanel;
//    private void Start()
//    {
//        // أول أفاتار مفتوح دائمًا
//        if (shopItemsList != null && shopItemsList.Count > 0)
//        {
//            GameData.SetItemPurchased(shopItemsList[0].itemName, 0, true);
//        }

//        LoadShop();
//        RefreshStarsUI();

//        if (Profile.Instance != null)
//        {
//            Profile.Instance.LoadAvailableAvatars();
//        }
//    }

//    private void LoadShop()
//    {
//        if (shopScrollView == null || itemTemplate == null)
//        {
//            Debug.LogError("shopScrollView غير مربوط في Inspector");
//            return;
//        }
//        /*
//        if (itemTemplate == null)
//        {
//            if (shopScrollView.childCount > 0)
//                itemTemplate = shopScrollView.GetChild(0).gameObject;
//            else
//            {
//                Debug.LogError("itemTemplate غير مربوط ولا يوجد عنصر داخل shopScrollView");
//                return;
//            }
//        }*/

//        //itemTemplate.SetActive(false);

//        for (int i = 0; i < shopItemsList.Count; i++)
//        {
//            CreateShopItemUI(i);
//        }
//    }


//    private void CreateShopItemUI(int index)
//    {
//        ShopItem item = shopItemsList[index];
//        item.isPurchased = GameData.IsItemPurchased(item.itemName, index);

//        GameObject itemUI = Instantiate(itemTemplate, shopScrollView);
//        itemUI.SetActive(true);

//        Image itemImage = itemUI.transform.GetChild(0).GetComponent<Image>();
//        TextMeshProUGUI priceText = itemUI.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
//        Button buyButton = itemUI.transform.GetChild(2).GetComponent<Button>();

//        if (itemImage != null)
//            itemImage.sprite = item.image;

//        if (priceText != null)
//            priceText.text = item.price.ToString();

//        item.itemButton = buyButton;

//        TextMeshProUGUI btnText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
//        btnText.font = arabicFont; 
//        buyButton.onClick.RemoveAllListeners();

//        if (item.isPurchased)
//        {
//            buyButton.interactable = false;
//            if (btnText != null) btnText.text = "ﻚﻠﺘﻤﻣ";
//        }
//        else
//        {
//            buyButton.interactable = true;
//            if (btnText != null) btnText.text = "ءاﺮﺷ";
//            buyButton.onClick.AddListener(() => OnShopItemBtnClicked(index));
//        }
//    }

//    private void OnShopItemBtnClicked(int itemIndex)
//    {
//        ShopItem item = shopItemsList[itemIndex];
//        int currentStars = GameData.GetTotalStars();

//        if (item.isPurchased)
//            return;

//        if (currentStars >= item.price)
//        {
//            currentStars -= item.price;
//            GameData.SetTotalStars(currentStars);
//            GameData.SetItemPurchased(item.itemName, itemIndex, true);

//            item.isPurchased = true;

//            if (item.itemButton != null)
//            {
//                item.itemButton.interactable = false;
//                TextMeshProUGUI btnText = item.itemButton.GetComponentInChildren<TextMeshProUGUI>();
//                if (btnText != null)
//                    btnText.text = "ﻚﻠﺘﻤﻣ";
//            }

//            RefreshStarsUI();
//            RefreshGlobalStarsDisplay();

//            if (Profile.Instance != null)
//            {
//                Profile.Instance.LoadAvailableAvatars();
//            }

//            Debug.Log("تم شراء: " + item.itemName);
//        }
//        else
//        {
//            Debug.Log("النجوم غير كافية للشراء");
//        }
//    }

//    private void RefreshStarsUI()
//    {
//        if (totalStarsDisplay != null)
//        {
//            totalStarsDisplay.text = GameData.GetTotalStars().ToString();
//        }
//    }

//    private void RefreshGlobalStarsDisplay()
//    {
//        TotalStarsDisplay displayScript = FindObjectOfType<TotalStarsDisplay>();
//        if (displayScript != null)
//        {
//            displayScript.RefreshDisplay();
//        }
//    }
//    /*
//    public void OpenShop()
//    {
//        ShopPanel.SetActive(true);
//        RefreshStarsUI();
//    }
//    */
//    public void OpenShop()
//    {
//        Debug.Log("تم الضغط على زر المتجر");

//        if (ShopPanel != null)
//        {
//            ShopPanel.SetActive(true);
//            RefreshStarsUI();
//        }
//        else
//        {
//            Debug.LogError("ShopPanel غير مربوط في Inspector");
//        }
//    }
//    public void CloseShop()
//    {
//        ShopPanel.SetActive(false);
//    }

//    public int GetAvatarCount()
//    {
//        return shopItemsList.Count;
//    }

//    public Sprite GetAvatarSprite(int index)
//    {
//        if (index >= 0 && index < shopItemsList.Count)
//        {
//            return shopItemsList[index].image;
//        }
//        return null;
//    }

//    public bool IsAvatarPurchasedByIndex(int index)
//    {
//        if (index < 0 || index >= shopItemsList.Count)
//            return false;

//        return GameData.IsItemPurchased(shopItemsList[index].itemName, index);
//    }

//    public void RefreshShop()
//    {
//        foreach (Transform child in shopScrollView)
//        {
//            if (child.gameObject != itemTemplate)
//                Destroy(child.gameObject);
//        }

//        LoadShop();
//    }
//}
