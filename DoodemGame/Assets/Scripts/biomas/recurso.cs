using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class recurso : MonoBehaviour
{
    public bool isSelected;
    private int indexLayerArea;
    public Recursos _typeRecurso;
    public Vector3 originPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(transform.parent.parent.name);
        indexLayerArea = transform.parent.parent.GetComponent<ABiome>().indexLayerArea;
        StartCoroutine(AddPosition());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator AddPosition()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.AddPositionSomething(transform.position,gameObject);
        GameManager.Instance.entidatesPrueba.Add(gameObject);
    }
    public bool GetSelected()
    {
        return isSelected;
    }

    public void SetSelected(bool b)
    {
        isSelected = b;
    }

    public void ResetResource()
    {
        Debug.LogWarning("me vuelvo a "+originPosition+" desde: "+transform.position);
        transform.position=originPosition;
        isSelected = false;
    }   
    public void PickRecurso()
    {
        if (GetComponent<MeshRenderer>().enabled == false) return;
        // gameObject.SetActive(false);
        //Destroy(gameObject);
        transform.position += Vector3.right*100;
    }

    public void CheckIfItsInMyBiome()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit, 1f, LayerMask.GetMask("casilla")))
        {
            if (indexLayerArea != hit.transform.GetComponent<NavMeshModifier>().area || hit.transform.GetComponent<casilla>().GetBiome().
                    GetComponent<NetworkObject>().OwnerClientId != transform.root.GetComponent<NetworkObject>().OwnerClientId || 
                transform.root.gameObject!=hit.transform.GetComponent<casilla>().GetBiome())
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
