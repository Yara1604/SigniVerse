using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    public float rotationSpeed = 50f; // السرعة، غيريها زي ما بدك

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}