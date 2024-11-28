using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelloWorld;
using tienda;
using Unity.AI.Navigation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Seleccionable : MonoBehaviour, IPointerDownHandler
{
    public GameObject objetoACrear;
    private GameObject objeto;
    public static int ClientID = -1;
    public int indexPrefab;
    public bool CanDropEnemySide;
    public int numCartas;
    [SerializeField] private ScriptableObjectTienda[] info;
    private List<GameObject> _objectsToDelete;


    [SerializeField] private MeshRenderer terreno;
    private Vector2Int _grid;
    private bool _selected;
    private List<Transform> cartas;
    public Inventory inventory;
    public static int MaxTotems;
    public bool isTotem;
    private Transform totemParent;
    private Transform biomaParent;

    private int _activeDestroyCoroutines;
    
    void Start()
    {
        totemParent = GameObject.Find("SeleccionableTotemsParent").transform;
        biomaParent = GameObject.Find("Canvas/cartas/totems").transform;
        if(biomaParent) Debug.LogError("hplaaaa");
        _objectsToDelete = new List<GameObject>();
        cartas = new List<Transform>();
        foreach (Transform t in transform.parent)
        {
            if(t!=transform)
                cartas.Add(t);
        }

        terreno = GameObject.Find("terreno").GetComponent<MeshRenderer>();
        _grid = terreno.gameObject.GetComponent<terreno>().GetGrid();
        // ClientID = -1;
        GameManager.Instance.OnStartMatch += OnStartMatch;
    }
    
    GameObject InstanciarObjeto(Vector3 position)
    {
        
        var a = Instantiate(objetoACrear, position, objetoACrear.transform.rotation);
        if (info.Length >= 3)
        {
            a.GetComponent<Totem>().CreateTotem(info[0], info[1], info[2]);
            a.transform.rotation = Quaternion.Euler(ClientID == 0 ? Vector3.forward : Vector3.back);
        }
        a.name = "Dummy Totem";
        SetChildrenActive(false);
        return a;

    }


    void Update()
    {
        if (Input.GetMouseButton(0) && !(isTotem && MaxTotems <= 0))
        {
            // Debug.Log($"{_selected} + {!GameManager.Instance.startedGame} yy {numCartas}");
            if (_selected && !GameManager.Instance.startedGame && numCartas>0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit,100, LayerMask.GetMask("Terreno")))
                {
                    if (!CanDropEnemySide && (ClientID == 0 && hit.point.z < terreno.transform.position.z  ||
                        ClientID == 1 && hit.point.z > terreno.transform.position.z ))
                    {
                        return;
                    }
                    var corner = hit.transform.position - hit.transform.lossyScale / 2F;
                    var newPos = hit.point - corner;
                    var cellSize = new Vector2(hit.transform.lossyScale.x, hit.transform.lossyScale.z) / _grid;
                    var pos = new Vector2Int((int)(newPos.x / cellSize.x), (int)(newPos.z/cellSize.y) );
                    if(pos.x == _grid.x || pos.y == _grid.y)    return;
                    if (objeto == null) {objeto = InstanciarObjeto(Input.mousePosition);}
                    objeto.transform.position = new Vector3(corner.x + pos.x * cellSize.x + cellSize.x /2f, 
                        1.1f, corner.z + pos.y * cellSize.y + cellSize.y/2f);
                }
            }
        }
    
        if (Input.GetMouseButtonUp(0))
        {
            TryDrop();
            // if (_selected && objeto)
            // {
            //     SpawnServer(objeto.transform.position, ClientID);
            //     GameObject o = objeto;
            //     StartCoroutine(DestroyObject(o));
            //     _selected = false;
            //     objeto = null;
            // }else if (_selected && !objeto)
            // {
            //     // SetChildrenActive(true);
            // }

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("ClientID: " + ClientID);
        }
    }

    private void TryDrop()
    {
        if (_selected && objeto)
        {
            if (isTotem)
                MaxTotems--;
            SpawnServer(objeto.transform.position, ClientID);
            _objectsToDelete.Add(objeto);
            GameObject o = objeto;
            StartCoroutine(DestroyObject(o));
            _activeDestroyCoroutines++;
            _selected = false;
            objeto = null;
            Transform casillas = terreno.transform.GetChild(0);
            foreach (Transform c in casillas)
            {
                c.GetComponent<casilla>().SetPreviousColorSeleccionable();
            }
        }
    }
    
    private IEnumerator DestroyObject(GameObject o)
    {
        yield return  new WaitUntil(()=>GameManager.Instance.startedGame);
        Destroy(o);
        _activeDestroyCoroutines--;
        if (_activeDestroyCoroutines == 0)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnServer(Vector3 pos, int playerId)
    {
        // Debug.Log(playerId);
        // if(IsSpawned)
        if(info.Length>=3)
            GameManager.Instance.SpawnServerRpc(playerId, indexPrefab, pos, info[0].num, info[1].num, info[2].num);
        else
        {
            GameManager.Instance.SpawnServerRpc(playerId, indexPrefab, pos, 0, 0, 0);
            inventory.UseBiome(info[0]);
        }
        numCartas--;
        
        // if(numCartas == 0)  Destroy(gameObject);
    }

    private void SetChildrenActive(bool act)
    {
        if(transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(act);
            }
        }
        else
        {
            GetComponent<Image>().enabled = act;
        }
    }
    
    public void SetInfo(ScriptableObjectTienda h, ScriptableObjectTienda b, ScriptableObjectTienda f)
    {
        info = new[] { h, b, f };
    }
    public void SetInfo(ScriptableObjectTienda b)
    {
        info = new[] { b};
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(GameManager.Instance.startedGame) return;
        if(!_selected)
            SetColorGridHolding();
        GameManager.Instance.objectSelected = null;
        foreach (var c in cartas.Where(c => c))
        {
            if (c.TryGetComponent(out Seleccionable s))
            {
                s.SetFalse();
            }
        }

        foreach (Transform c in totemParent)
        {
            if (c && c.TryGetComponent(out Seleccionable s))
            {
                s.SetFalse();
            }
        }
        foreach (Transform c in biomaParent)
        {
            if (c && c.TryGetComponent(out Seleccionable s))
            {
                s.SetFalse();
            }
        }
        _selected = true;
    }

    private void SetColorGridHolding()
    {
        Debug.LogError("28");
        Transform casillas = terreno.transform.GetChild(0);
        foreach (Transform c in casillas)
        {
            c.GetComponent<casilla>().SetColorSeleccionable(CanDropEnemySide);
        }
    }
    public void SetFalse()
    {
        _selected = false;
    }

    public void AddNumCarta()
    {
        numCartas++;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartMatch -= OnStartMatch;
    }

    private void OnStartMatch()
    {
        TryDrop();
        Transform casillas = terreno.transform.GetChild(0);
        foreach (Transform c in casillas)
        {
            c.GetComponent<casilla>().SetPreviousColorSeleccionable();
        }
        // foreach (GameObject o in _objectsToDelete)
        // {
        //     Destroy(o);
        // }
        // _objectsToDelete.Clear();
        // gameObject.SetActive(false);
        // Destroy(gameObject);
    }
}
