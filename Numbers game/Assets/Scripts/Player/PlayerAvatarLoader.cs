using UnityEngine;

public class PlayerAvatarLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerRenderer;

    private void Start()
    {
        ApplySelectedAvatar();
    }

    public void ApplySelectedAvatar()
    {
        if (playerRenderer == null)
        {
            playerRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerRenderer == null)
        {
            Debug.LogError("PlayerAvatarLoader: لا يوجد SpriteRenderer");
            return;
        }

        if (AvatarDatabase.Instance == null)
        {
            Debug.LogError("PlayerAvatarLoader: AvatarDatabase.Instance غير موجود");
            return;
        }

        int selectedIndex = GameData.GetSelectedAvatarIndex();
        Sprite selectedSprite = AvatarDatabase.Instance.GetAvatarSprite(selectedIndex);

        if (selectedSprite != null)
        {
            playerRenderer.sprite = selectedSprite;
        }
    }
}