using TMPro;
using UnityEngine;

public class TotalStarsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalStarsText;

    private void Start()
    {
        RefreshDisplay();
    }

    private void OnEnable()
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (totalStarsText != null)
        {
            totalStarsText.text = GameData.GetTotalStars().ToString();
        }
    }
}