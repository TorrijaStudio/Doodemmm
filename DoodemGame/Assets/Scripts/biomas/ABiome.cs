using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.AI.Navigation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public abstract class ABiome : NetworkBehaviour
{
    private Transform obstaculos;
    private Transform recursos;
    public terreno terreno;
    private Vector2 cellSize;
    private List<Vector2> pos;//posiciones disponibles del bioma
    public NetworkVariable<int> _idPlayer = new NetworkVariable<int>(writePerm:NetworkVariableWritePermission.Server);
    //tamaño del bioma. Numero de celdas a cada lado
    public int xSize;
    public int zSize;
    public Material mat;
    public int indexLayerArea;
    public Recursos[] typeResource;
    public MeshFilter[] meshFilters;

    private static Random random;
    public BiomeType type;
    
    public enum BiomeType
    {
        Forest,
        Desert,
        Glacier,
        Mountain,
        Lake,
    }
    
    
    private void Awake()
    {
        //GameManager.Instance.playerObjects.Add(gameObject);
        //gameObject.SetActive(false);
        GameManager.Instance.playerObjects.Add(gameObject);
        DisableMeshesRecursively(gameObject);

    }
    void DisableMeshesRecursively(GameObject obj)
    {
        // Desactiva el MeshRenderer si el objeto lo tiene
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // Recorre todos los hijos del objeto y llama recursivamente a esta función
        foreach (Transform child in obj.transform)
        {
            DisableMeshesRecursively(child.gameObject);
        }
    }
    public void EnableMeshesRecursively(GameObject obj)
    {
        // Desactiva el MeshRenderer si el objeto lo tiene
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Debug.LogError("ACTIVO");
            meshRenderer.enabled = true;
        }

        // Recorre todos los hijos del objeto y llama recursivamente a esta función
        foreach (Transform child in obj.transform)
        {
            EnableMeshesRecursively(child.gameObject);
        }
    }
    
    public void SetColorsGridBiome()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale/2f, transform.rotation,
            LayerMask.GetMask("casilla"));
        // Debug.LogError(colliders.Length);
        foreach (var c in colliders)
        {
            var casillaMesh = c.GetComponent<MeshRenderer>();
            var casilla = c.GetComponent<casilla>();
            var materialBiome = casilla.GetBiome().GetComponent<ABiome>().mat;
            casillaMesh.material = materialBiome;
        }
        foreach (Transform r in recursos)
        {
            if(r.gameObject)
            {
                // Debug.LogError(r.name+" : "+r.position);
                r.GetComponent<recurso>().originPosition = r.position;
            }
        }

    }
    
    // Start is called before the first frame update
     void Start()
    {
        HashSet<Vector2> positions = new HashSet<Vector2>();
        pos = new List<Vector2>();
        terreno = GameObject.Find("terreno").GetComponent<terreno>();
        cellSize = new Vector2(terreno.gameObject.transform.lossyScale.x, terreno.gameObject.transform.lossyScale.z) /
                   terreno.GetGrid();
        for (int i = 0; i <= xSize; i++)
        {
            for (int j = 0; j <= zSize; j++)
            {
                positions.Add(new Vector2(i, j));
                positions.Add(new Vector2(-i, -j));
                positions.Add(new Vector2(i, -j));
                positions.Add(new Vector2(-i, j));
            }
        }
        
        pos = positions.ToList();
        transform.localScale = new Vector3(2*xSize*cellSize.x+cellSize.x,transform.localScale.y*4,2*zSize*cellSize.y+cellSize.y);
        
        SetHijos();
        //if (IsOwner)
        //{
        //    Debug.LogError("holaaaa");
        //    SetHijos();
        //}

        StartCoroutine(UpdateEntities());
        
        //GameManager.Instance.playerObjects[_idPlayer.Value].Add(gameObject);
        //if (GameManager.Instance.clientId != _idPlayer.Value)
        //{
        //    gameObject.SetActive(false);
        //}
    }

    

     private IEnumerator UpdateEntities()
     {
         yield return new WaitForSeconds(0.5f);
         GameManager.Instance.UpdateBiomeThings();
     }


     private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent &&
                   other.transform.parent.name == "Tiles" 
                   //&& (!other.GetComponent<casilla>().GetBiome() || gameObject!=other.GetComponent<casilla>().GetBiome())
                   )
        {
            var biomaCasilla = other.GetComponent<casilla>().GetBiome();
            var mesh = other.transform.GetComponent<MeshRenderer>();
            if (IsOwner)
                mesh.material = mat;
            //Debug.LogError(biomaCasilla.name);
            //if(biomaCasilla.GetComponent<NetworkObject>().OwnerClientId == GetComponent<NetworkObject>().OwnerClientId)
            //{
            if(!other.GetComponent<casilla>().GetBiome() || GetComponent<NetworkObject>().OwnerClientId == biomaCasilla.GetComponent<NetworkObject>().OwnerClientId)
            {
                var casilla = other.GetComponent<casilla>();
                var index = casilla.GetAreaNav();
                casilla.SetPreviousIndexArea(index);
                casilla.SetPreviousMat(mesh.material);
                casilla.SetPreviousBiome(casilla.GetBiome());
                
                casilla.SetMat(mat);
                casilla.SetBiome(gameObject);
                casilla.SetAreaNav(indexLayerArea);
            }
            else
            {
                var positionCasilla = terreno.PositionToGrid(other.transform.position);
                if ((positionCasilla.y < 8 && _idPlayer.Value == 1) || (positionCasilla.y >= 8 && _idPlayer.Value == 0) )//terreno cliente / host
                {
                    var casilla = other.GetComponent<casilla>();
                    var index = casilla.GetAreaNav();
                    casilla.SetPreviousIndexArea(index);
                    casilla.SetPreviousMat(mesh.material);
                    casilla.SetPreviousBiome(casilla.GetBiome());
                
                    casilla.SetMat(mat);
                    casilla.SetBiome(gameObject);
                    casilla.SetAreaNav(indexLayerArea);
                }
            }
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent && other.transform.parent.name == "Tiles"&& other.GetComponent<casilla>().GetBiome() == gameObject)
        {
            var casilla = other.GetComponent<casilla>();
            other.transform.GetComponent<MeshRenderer>().material = casilla.GetPrevousMat();
            casilla.SetBiome(casilla.GetPreviousBiome());
            casilla.SetAreaNav(casilla.GetPreviousIndexArea());
        }
    }
    

    public void SetHijos()
    {
        obstaculos = transform.GetChild(0);
        int aux1 = 0;
        foreach (Transform t in obstaculos)
        {
            obstaculos.localScale = Vector3.one;
            if(!t.GetComponent<obstaculo>().isSet)
            {
                int index = UnityEngine.Random.Range(0, pos.Count);
                Vector2 v = pos[index];
                //pos.Remove(v);
                //Vector3 newPos = new Vector3(v.x * cellSize.x + transform.position.x, transform.position.y,v.y * cellSize.y + transform.position.z);
                
                var u = index%2==0 ? SetT(v, obstaculos, aux1) :SetL(v,obstaculos,aux1);
                //var u = SetL(v, obstaculos, aux1);
                // Debug.LogError(u+" : "+v);
                if (!u)
                {
                    t.GetComponent<obstaculo>().isSet = true;   
                    pos.Remove(v);
                    Vector3 newPos = new Vector3(v.x * cellSize.x + transform.position.x, obstaculos.GetChild(aux1).position.y,
                    v.y * cellSize.y + transform.position.z);
                    if (terreno.IsInside(newPos))
                    {
                        t.position = newPos;
                    }
                    else
                        t.position = newPos;
                }
                
                //if (terreno.IsInside(newPos))
                //{
                //    t.position = newPos;
                //}
                //else
                //    t.gameObject.SetActive(false);
            }
            aux1++;
        }

        recursos = transform.GetChild(1);
        int aux = 0;
        foreach (Transform r in recursos)
        {
            recursos.localScale = Vector3.one;
            int index = UnityEngine.Random.Range(0, pos.Count);
            Vector2 v =pos[index];
            pos.Remove(v);
            Vector3 newPos = new Vector3(v.x*cellSize.x+transform.position.x,recursos.GetChild(aux).position.y,v.y*cellSize.y+transform.position.z);
            if(terreno.IsInside(newPos))
            {
                r.position = newPos;
            }else
                r.position = newPos;
            
            if (IsHost)
            {
                GameManager.Instance.GenerateRandomNumberServerRpc(typeResource.Length-1,GetComponent<NetworkObject>(),aux);
            }
            aux++;
        }
        //terreno.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    
    public IEnumerator SetResourcesDespawn(int time)
    {
        yield return new WaitForSeconds(time);
        foreach (Transform r in recursos)
        {
            if(r!=null)
            {
                r.GetComponent<MeshRenderer>().enabled = false;
                if (IsHost)
                    r.GetComponent<recurso>().ResetResource();
            }
        }
    }

    private bool SetT(Vector2 v,Transform obs,int numHijo)
    {
        if (obs.childCount - numHijo < 4) return false;//no hay obstaculos suficientes; 5 bloques forman una T
        //comprobamos izq y derecha
        
        if (CheckBlocksAvailability(v, Vector2.left, 1) && CheckBlocksAvailability(v, Vector2.right, 1))
        {
            //comprobamos 2 arriba 
            if (CheckBlocksAvailability(v, Vector2.up, 1))
            {
                //asignar posiciones a la izq, der, y 2 arriba y centro
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.up);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
            //comprobamos 2 abajo
            if (CheckBlocksAvailability(v, Vector2.down, 1))
            {
                //asignar posiciones a la izq, der y 2 abajo y centro
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.down);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
        }
        else
        {
            //comprobamos arriba y abajo
            if (CheckBlocksAvailability(v, Vector2.up, 1) && CheckBlocksAvailability(v, Vector2.down, 1))
            {
                //Comprobamos 2 derecha
                if (CheckBlocksAvailability(v, Vector2.right, 1))
                {
                    //asignamos posiciones arriba, abajo y 2 derecha y centro
                    SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.up);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.down);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.right);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                    return true;
                }
                //comprobamos 2 izquierda
                if (CheckBlocksAvailability(v, Vector2.left, 1))
                {
                    //asignamos posiciones arriba, abajo y 2 izquierda y centro
                    SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.up);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.down);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.left);
                    SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                    return true;
                }
            }
        }
        return false;
    }

    private bool SetL(Vector2 v,Transform obs,int numHijo)
    {
        if (obs.childCount - numHijo < 4) return false;
        if (CheckBlocksAvailability(v, Vector2.up, 2))
        {
            if (CheckBlocksAvailability(v, Vector2.left, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.up);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.up*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }

            if (CheckBlocksAvailability(v, Vector2.right, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.up);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.up*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
        }
        if (CheckBlocksAvailability(v, Vector2.down, 2))
        {
            if (CheckBlocksAvailability(v, Vector2.left, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.down);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.down*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }

            if (CheckBlocksAvailability(v, Vector2.right, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.down);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.down*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
        }
        if (CheckBlocksAvailability(v, Vector2.right, 2))
        {
            if (CheckBlocksAvailability(v, Vector2.up, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.right*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.up);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }

            if (CheckBlocksAvailability(v, Vector2.down, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.right);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.right*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.down);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
        }
        if (CheckBlocksAvailability(v, Vector2.left, 2))
        {
            if (CheckBlocksAvailability(v, Vector2.up, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.left*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.up);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }

            if (CheckBlocksAvailability(v, Vector2.down, 1))
            {
                SetPositionObstacle(v,obs.GetChild(numHijo),Vector2.left);
                SetPositionObstacle(v,obs.GetChild(numHijo + 1),Vector2.left*2);
                SetPositionObstacle(v,obs.GetChild(numHijo + 2),Vector2.down);
                SetPositionObstacle(v,obs.GetChild(numHijo + 3),Vector2.zero);
                return true;
            }
        }
        return false;
    }
    private void SetPositionObstacle(Vector2 v,Transform t,Vector2 dir)
    {
        var newV = v + dir;
        var newPos = new Vector3(newV.x*cellSize.x+transform.position.x,t.transform.position.y,newV.y*cellSize.y+transform.position.z);
        t.position = newPos;
        t.GetComponent<obstaculo>().isSet = true;
        pos.Remove(newV);
    }
    private bool CheckBlocksAvailability(Vector2 v,Vector2 dir,int numBlocks)
    {
        for (int i = 1; i <= numBlocks; i++)
        {
            var newV = v + dir * i;
            var newPos = new Vector3(newV.x*cellSize.x+transform.position.x,transform.position.y,newV.y*cellSize.y+transform.position.z);
            if (!pos.Contains(newV) || !terreno.IsInside(newPos))
            {
                return false;
            }
        }
        return true;
    }

    public Transform GetRecursos()
    {
        return recursos;
    }

    void OnDestroy()
    {
        //GameManager.Instance.playerObjects[_idPlayer.Value].Remove(gameObject);
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale/2f, transform.rotation,LayerMask.GetMask("casilla"));
       
       foreach (var c in colliders)
       {
           var casilla = c.GetComponent<casilla>();
           if (casilla.GetBiome() == gameObject)
           {
               casilla.ResetCasilla();
           }
       }
       GameManager.Instance.UpdateBiomeThings();
    }

    public abstract void ActionBioma(GameObject o);
    public abstract void LeaveBiome(GameObject o);


}
