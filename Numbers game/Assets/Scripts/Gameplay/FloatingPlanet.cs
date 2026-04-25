using UnityEngine;

public class FloatingPlanet : MonoBehaviour
{
    [Header("إعدادات الطفو (صعود ونزول)")]
    public float verticalSpeed = 0.8f;
    public float verticalAmplitude = 15.0f; // في الـ UI نستخدم قيم أكبر قليلاً لأنها تعتمد على البيكسل

    [Header("إعدادات التمايل")]
    public float tiltSpeed = 0.5f;
    public float tiltAmount = 3.0f;

    private RectTransform rectTransform; // استخدام RectTransform بدلاً من Transform
    private Vector2 startAnchoredPos;    // حفظ الموقع المربوط بالـ Anchors
    private float randomOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPos = rectTransform.anchoredPosition; // حفظ الموقع النسبي
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float timeValue = (Time.time * verticalSpeed) + randomOffset;

        // 1. حركة الصعود والنزول باستخدام anchoredPosition
        // هذا يضمن أن الحركة تتم "حول" نقطة الـ Anchor التي حددتها
        float newY = startAnchoredPos.y + Mathf.Sin(timeValue) * verticalAmplitude;
        rectTransform.anchoredPosition = new Vector2(startAnchoredPos.x, newY);

        // 2. التمايل الجانبي
        float tiltValue = (Time.time * tiltSpeed) + randomOffset;
        float angle = Mathf.Sin(tiltValue) * tiltAmount;

        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}