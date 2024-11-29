using System.Collections;
using System.Collections.Generic;
using ItemInformation;
using tienda;
using UnityEngine;

public class popUp : MonoBehaviour
{
    public float rotationDuration = 0.1f;
    public float rotationAngle = 90f;  
    
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RotateSection(ItemInfoDisplay item,ScriptableObjectTienda so)
    {
        StartCoroutine(RotatePopUp(item,so));
    }

    private IEnumerator RotatePopUp(ItemInfoDisplay item,ScriptableObjectTienda so)
    {
        yield return RotateTo(new Vector3(rotationAngle, 0, 0));
        
        item.DisplayItem(so);
        
        yield return RotateTo(new Vector3(0, 0, 0));
    }

    private IEnumerator RotateTo(Vector3 targetRotation)
    {
        Vector3 initialRotation = rectTransform.localEulerAngles;
        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / rotationDuration);
            rectTransform.localEulerAngles = Vector3.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        rectTransform.localEulerAngles = targetRotation;
    }
}