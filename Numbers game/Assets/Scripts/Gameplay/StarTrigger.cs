using UnityEngine;
using System.Collections;

public class StarTrigger : MonoBehaviour
{
    public ParticleSystem beamEffect;
    public SpriteRenderer starSprite;

    private bool isCollected = false;

    private void Start()
    {
        if (beamEffect == null)
            beamEffect = GetComponentInChildren<ParticleSystem>();

        if (starSprite == null)
            starSprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            StartCoroutine(PlayEffectAndDestroy());
        }
    }

    private IEnumerator PlayEffectAndDestroy()
    {
        transform.localScale = Vector3.one * 1.5f;

        if (beamEffect != null)
        {
            beamEffect.Play();
        }

        if (starSprite != null)
        {
            starSprite.enabled = false;
        }

        if (NumbersGameManager.Instance != null)
        {
            NumbersGameManager.Instance.AddStar();
        }

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}