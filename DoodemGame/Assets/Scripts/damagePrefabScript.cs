using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damagePrefabScript : MonoBehaviour
{
    public float fadeDuration = 1f;
    private TextMeshPro _textMeshPro;

    void Start()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
        if (_textMeshPro != null)
            StartCoroutine(FadeOut(_textMeshPro));
    }

    private IEnumerator FadeOut(TextMeshPro text)
    {
        Color originalColor = text.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration); 
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up*Time.deltaTime);
    }
}
