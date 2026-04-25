using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Profile : MonoBehaviour
{
    #region Singleton
    public static Profile Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [System.Serializable]
    public class Avatar
    {
        public Sprite Image;
        public int shopIndex;
    }

    public List<Avatar> AvatarsList = new List<Avatar>();

    [SerializeField] private GameObject AvatarUITemplate;
    [SerializeField] private Transform AvatarsScrollView;
    [SerializeField] private Color ActiveAvatarColor = Color.green;
    [SerializeField] private Color DefaultAvatarColor = Color.white;
    [SerializeField] private Image CurrentAvatar;

    private int newSelectedIndex = -1;
    private int previousSelectedIndex = -1;

    private void Start()
    {
        // لا تستدعي LoadAvailableAvatars هنا
        // ShopManager هو الذي سيستدعيها بعد أن ينتهي
    }

    public void LoadAvailableAvatars()
    {
        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance غير موجود");
            return;
        }

        if (AvatarUITemplate == null || AvatarsScrollView == null || CurrentAvatar == null)
        {
            Debug.LogError("هناك عناصر غير مربوطة في Profile Inspector");
            return;
        }

        AvatarsList.Clear();

        foreach (Transform child in AvatarsScrollView)
        {
            if (child.gameObject != AvatarUITemplate)
                Destroy(child.gameObject);
        }

        AvatarUITemplate.SetActive(false);

        for (int i = 0; i < ShopManager.Instance.shopItemsList.Count; i++)
        {
            var item = ShopManager.Instance.shopItemsList[i];

            if (GameData.IsItemPurchased(item.itemName, i))
            {
                AddAvatar(item.image, i);
            }
        }

        if (AvatarsList.Count == 0)
        {
            Debug.LogWarning("لا يوجد أفاتارات مشتراة");
            return;
        }

        int savedShopIndex = GameData.GetSelectedAvatarIndex();
        int localIndex = 0;

        for (int i = 0; i < AvatarsList.Count; i++)
        {
            if (AvatarsList[i].shopIndex == savedShopIndex)
            {
                localIndex = i;
                break;
            }
        }

        SelectAvatar(localIndex);
    }

    public void AddAvatar(Sprite img, int shopIndex)
    {
        if (AvatarUITemplate == null)
        {
            Debug.LogError("AvatarUITemplate غير مربوط");
            return;
        }

        if (AvatarsScrollView == null)
        {
            Debug.LogError("AvatarsScrollView غير مربوط");
            return;
        }

        Avatar av = new Avatar
        {
            Image = img,
            shopIndex = shopIndex
        };

        AvatarsList.Add(av);

        GameObject g = Instantiate(AvatarUITemplate, AvatarsScrollView);
        g.SetActive(true);

        Button btn = g.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("العنصر Avatar Template لا يحتوي Button");
            return;
        }

        Transform iconTransform = g.transform.Find("Icon");
        if (iconTransform == null)
        {
            Debug.LogError("لم يتم العثور على عنصر Icon داخل Avatar Template");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError("عنصر Icon لا يحتوي Image component");
            return;
        }

        iconImage.sprite = av.Image;

        int index = AvatarsList.Count - 1;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnAvatarClick(index));
    }

    private void OnAvatarClick(int avatarIndex)
    {
        SelectAvatar(avatarIndex);
    }

    private void SelectAvatar(int avatarIndex)
    {
        if (avatarIndex < 0 || avatarIndex >= AvatarsList.Count)
            return;

        previousSelectedIndex = newSelectedIndex;
        newSelectedIndex = avatarIndex;

        if (previousSelectedIndex >= 0)
        {
            int oldChildIndex = previousSelectedIndex + 1;
            if (oldChildIndex < AvatarsScrollView.childCount)
            {
                Image oldBg = AvatarsScrollView.GetChild(oldChildIndex).GetComponent<Image>();
                if (oldBg != null)
                    oldBg.color = DefaultAvatarColor;
            }
        }

        int newChildIndex = newSelectedIndex + 1;
        if (newChildIndex < AvatarsScrollView.childCount)
        {
            Image newBg = AvatarsScrollView.GetChild(newChildIndex).GetComponent<Image>();
            if (newBg != null)
                newBg.color = ActiveAvatarColor;
        }

        if (CurrentAvatar != null)
            CurrentAvatar.sprite = AvatarsList[newSelectedIndex].Image;

        GameData.SetSelectedAvatarIndex(AvatarsList[newSelectedIndex].shopIndex);
    }
}