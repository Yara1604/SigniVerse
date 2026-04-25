using UnityEngine;

public class AvatarDatabase : MonoBehaviour
{
    public static AvatarDatabase Instance;

    [SerializeField] private Sprite[] avatarSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetAvatarSprite(int index)
    {
        if (avatarSprites == null || avatarSprites.Length == 0)
        {
            Debug.LogError("AvatarDatabase: لا توجد صور مربوطة");
            return null;
        }

        if (index < 0 || index >= avatarSprites.Length)
        {
            index = 0;
        }

        return avatarSprites[index];
    }
}