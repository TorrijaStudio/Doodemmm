using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class obstaculo : MonoBehaviour
{
    private int indexLayerArea;

    public bool isSet;
    // Start is called before the first frame update
    void Start()
    {
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
    
    public void CheckIfItsInMyBiome()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit, 5f, LayerMask.GetMask("casilla")))
        {
            if (indexLayerArea != hit.transform.GetComponent<NavMeshModifier>().area || hit.transform.GetComponent<casilla>().GetBiome().
                    GetComponent<NetworkObject>().OwnerClientId != transform.root.GetComponent<NetworkObject>().OwnerClientId || 
                transform.root.gameObject!=hit.transform.GetComponent<casilla>().GetBiome())
            {
                gameObject.SetActive(false);
            }
        }else
        {
            gameObject.SetActive(false);
        }
    }
}
