using UnityEngine;
using TMPro;

public class LevelSelectStarsUI : MonoBehaviour
{
    public TextMeshProUGUI totalStarsText;

    // Start تعمل عند تشغيل السين لأول مرة
    void Start()
    {
        UpdateStarsUI();
    }

    // OnEnable تعمل في كل مرة تظهر فيها الشاشة (مفيدة جداً للقوائم)
    void OnEnable()
    {
        UpdateStarsUI();
    }

    public void UpdateStarsUI()
    {
        if (totalStarsText != null)
        {
            // جلب المجموع من مدير الحفظ اللي عندك
            int totalStars = StarsSaveManager.GetTotalStars();
            totalStarsText.text = totalStars.ToString();
        }
    }
}