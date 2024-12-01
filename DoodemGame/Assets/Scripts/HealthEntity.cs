using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;

public class HealthEntity : NetworkBehaviour
{
    public TextMeshPro textMeshPro;
    public Transform cameraTransform;
    public Transform green;
    public float maxHealth;
    private float currentHealth;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    private Bounds originalBounds;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = IsHost ? GameObject.Find("Main Camera1").transform : GameObject.Find("Main Camera").transform;
        maxHealth = GetComponent<Entity>().health;
        initialScale = green.parent.transform.localScale;
        initialPosition = green.transform.localPosition;
        currentHealth = maxHealth;
        originalBounds = green.GetComponent<SpriteRenderer>().bounds;
        if (IsOwner)
        {
            textMeshPro.color = new Color(0.07f, 0.59f, 0.02f);
            green.GetComponent<SpriteRenderer>().color = new Color(0.22f, 0.53f, 0f);
        }
        else
        {
            textMeshPro.color = new Color(0.68f, 0.08f, 0.16f);
            green.GetComponent<SpriteRenderer>().color = new Color(0.56f, 0.12f, 0.15f);;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        textMeshPro.transform.forward = cameraTransform.forward;
       
        
    }

    public void UpdateText(double h)
    {
        currentHealth = (float)(h);
        var t = (int)h;
        textMeshPro.text = t.ToString();
        float healthPercent = currentHealth / maxHealth;
        Debug.LogError("ME UPDATE");
        green.parent.localScale = new Vector3(initialScale.x * healthPercent, initialScale.y, initialScale.z);
        //var offset = originalBounds.extents.x - green.GetComponent<SpriteRenderer>().bounds.extents.x;
        var offset = initialScale.x - green.transform.localScale.x;
        //var offsetX = 0.5f*(initialScale.x - green.lossyScale.x);
        //Debug.LogError(offsetX);
        //green.localPosition = new Vector3(green.localPosition.x-offsetX,green.localPosition.y,green.localPosition.z);
        //float offsetX = (1 - healthPercent) * initialScale.x * 0.5f; // Mitad del cambio en escala
        //green.parent.localPosition = new Vector3(initialPosition.x - offset/2f, initialPosition.y, initialPosition.z);

    }
}
