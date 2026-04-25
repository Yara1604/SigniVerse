using UnityEngine;
// يجب إضافة هذا السطر للتعرف على مكونات الإضاءة في URP
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))] // لضمان وجود مكون إضاءة على نفس الكائن
public class StarFlicker2D : MonoBehaviour
{
    [Header("إعدادات الوميض")]
    public float minIntensity = 2f;    // أدنى شدة للإضاءة (خافتة)
    public float maxIntensity = 6f;    // أقصى شدة للإضاءة (ساطعة)
    public float flickerSpeed = 3f;    // سرعة تغير الإضاءة

    private Light2D starLight;         // مرجع لمكون الإضاءة
    private float randomOffset;        // لجعل كل نجمة تومض بوقت مختلف

    void Start()
    {
        // الحصول على مكون الإضاءة
        starLight = GetComponent<Light2D>();

        // توليد رقم عشوائي لكسر الرتابة (كما فعلنا في حركة الطفو)
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (starLight == null) return; // حماية ضد الأخطاء

        // استخدام Sin لحساب قيمة تتأرجح بين 0 و 1 بمرور الوقت
        // أضفنا الـ randomOffset لجعل التوقيت عشوائي لكل نسخة
        float timeValue = (Time.time * flickerSpeed) + randomOffset;

        // Mathf.Sin تعطي قيم بين -1 و 1، سنحولها لتكون بين 0 و 1
        float noise = Mathf.Sin(timeValue); // هذه تعطي من -1 لـ 1
        float t = (noise + 1f) / 2f; // هذه تحول النطاق ليصبح من 0 لـ 1

        // دالة Lerp تقوم بالانتقال بسلاسة بين قيمة الدنيا والقصوى بناءً على 't'
        starLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}