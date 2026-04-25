using UnityEngine;

public class FloatingEffect2D : MonoBehaviour
{
    [Header("إعدادات الحركة")]
    public float speed = 1.0f;      // سرعة التأرجح
    public float amplitude = 0.5f;  // مدى الارتفاع والنزول
    public float rotationAmount = 5f; // قوة الميلان

    private Vector3 startPos;
    private float randomOffset; // المتغير المسؤول عن التفاوت الزمني

    void Start()
    {
        startPos = transform.position;

        // توليد قيمة عشوائية كبيرة لضمان اختلاف البداية لكل نجمة
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // إضافة randomOffset داخل الـ Sin تجعل كل نجمة في مرحلة مختلفة من الموجة
        float timeValue = (Time.time * speed) + randomOffset;

        // حساب الموقع الجديد
        float newY = startPos.y + Mathf.Sin(timeValue) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // حساب الدوران الجديد
        float angle = Mathf.Sin(timeValue) * rotationAmount;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}