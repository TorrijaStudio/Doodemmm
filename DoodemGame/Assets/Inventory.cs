using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using tienda;
using Totems;
using Unity.Mathematics;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

public class Inventory : MonoBehaviour
{
    private List<List<TotemPiece>> _totemPieces;
    private Dictionary<ScriptableObjectTienda, int> _biomes;

    [FormerlySerializedAs("boton")] [SerializeField] private playerInfoStore playerStore;



    [Space(8)][Header("Totem drag variables")]
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject wall;
    [SerializeField] private float distance;
    [FormerlySerializedAs("posToSpawn")] [SerializeField] private Transform totemsPosToSpawn;
    [SerializeField] private Totem totemToInstantiate;

    [Space(5)] [Header("Totem pieces variables")] 
    [SerializeField] private Transform topCornerToSpawnPieces;

    [SerializeField] private Vector2Int totemPiecesDrawerSize;
    [SerializeField] private Vector2 totemPiecesDrawerSeparation;
    private int _totemPiecePage;

        [Space(8)][Header("Biome seleccionable variables")]
    [SerializeField] private Seleccionable seleccionableToSpawn;

    [SerializeField] private Transform seleccionableParent;
    [SerializeField] private Transform selecPosToSpawn;
    [SerializeField] private float selecDistance;
        
    [Space(8)][Header("Totem as seleccionables variables")]
    [SerializeField]
    private Transform totemParent;

    [SerializeField] private Transform seleccionableTotemParent;
    private Transform _seleccionableTotemPositions;
    [Space(8)]
    private int _clientID;
    public int clientID
    {
        get => _clientID ;
        set {
            _clientID = value;
            _seleccionableTotemPositions = GameObject.Find("TotemSeleccionablePositionsP" + (1 - _clientID)).transform;
        }
    }

    public int GetFullTotems()
    {
        return _totemPieces.Count(a => a.Count == 3);
    }
    
    public static Inventory Instance;
    // Start is called before the first frame update
    void Start()
    {
        _biomes = new Dictionary<ScriptableObjectTienda, int>();
        if (Instance) Destroy(gameObject);
        else Instance = this;
        
        _totemPieces = new List<List<TotemPiece>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Contains(TotemPiece piece)
    {
        foreach (var totem in _totemPieces)
        {
            foreach (var totemPiece in totem)
            {
                if (piece == totemPiece) return true;
            }
        }

        return false;
    }
    
    public void SetDrag(bool active)
    {
        pointer.SetActive(active);
        wall.SetActive(active);
    }
    public bool IsDragActive()
    {
        return pointer.activeSelf;
    }
    public void GetItemsFromShop()
    {
        GetTotemsFromShop();
        GetBiomesFromShop();
        playerStore.DestroyBoughtObjects();
    }

    public void GetTotemsFromShop()
    {
        foreach (var obj in playerStore.boughtObjects.Select(objetoTienda => objetoTienda.GetComponent<objetoTienda>()).Where(objT => !objT.info.isBiome))
        {
            Debug.Log(obj.name);
            _totemPieces.Add(obj.info.objectsToSell);
        }
    }

    private void GetBiomesFromShop()
    {
        foreach (var obj in playerStore.boughtObjects.Select(objetoTienda => objetoTienda.GetComponent<objetoTienda>()).Where(objT => objT.info.isBiome))
        {
            Debug.Log(obj.name);
            if (!_biomes.TryAdd(obj.info, 1))
            {
                _biomes[obj.info] += 1;
            }
            // _biomes.Add(obj.info.biomeToSell);
            // _totemPieces.Add(obj.info.objectsToSell);
        }
    }
    
    public void DespawnItems()
    {
        var tempTotemPieces = new List<List<TotemPiece>>();
        for(int i = totemParent.childCount - 1; i >= 0; i--)
        {
            var child = totemParent.GetChild(i);
            var tempInfo = child.GetComponent<Totem>().GetTotem();
            if(tempInfo.Count > 0)
                tempTotemPieces.Add(tempInfo.Select(piece => piece.objectsToSell[0]).ToList());
            // if (tempInfo.Count == 3)
            // {
            //     tempTotemPieces.Add(tempInfo.Select(piece => piece.objectsToSell[0]).ToList());
            // }
            // else  if(tempInfo.Count > 0)
            // {
            //     foreach (var totemPiece in tempInfo)
            //     {
            //         tempTotemPieces.Add(new List<TotemPiece>(){totemPiece.objectsToSell[0]});
            //     }
            // }
            Destroy(child.gameObject);
        }
        _totemPieces.Clear();
        
        _totemPieces = tempTotemPieces;
        foreach (var totemPiece in _totemPieces)
        {
            var txt = "";
            foreach (var piece in totemPiece)
            {
                txt += "_ " + piece;
            }
            Debug.Log(txt);
        }
    }
    
    public void SpawnTotems()
    {
        var objectsToSpawn = _totemPieces.Count;
        var separationDistance = distance / objectsToSpawn;
        var pos = totemsPosToSpawn.position;
        foreach (var totemPiece in _totemPieces)
        { 
            var totem = Instantiate(totemToInstantiate, pos, Quaternion.identity, totemParent);
            // totem.transform.localRotation = Quaternion.Euler(0, 0, 0);
            var aux = new GameObject[] { null, null, null };
            foreach (var piece in totemPiece)
            {
                switch (piece.tag)
                {
                    case "Head":
                        aux[0] = piece.gameObject;
                        break;
                    case "Body":
                        aux[1] = piece.gameObject;
                        break;
                    case "Feet":
                        aux[2] = piece.gameObject;
                        break;
                }
            }
            totem.CreateTotem(aux[0], aux[1], aux[2]);
            pos += Vector3.right * separationDistance;
        }
        SetDrag(true);
        // boton.DeleteShopItems();
    }

    public void SpawnTotemsAsSeleccionables()
    {
        DeleteSeleccionableTotems();
        var infoForSeleccionables = _totemPieces.Where((list => list.Count == 3)).ToList();
        Debug.Log(infoForSeleccionables.Count);
        var objectsToSpawn = infoForSeleccionables.Count;
        
        // var separationDistance = selecDistance / objectsToSpawn;
        var pos = _seleccionableTotemPositions.GetChild(0).position;
        var rotation = _seleccionableTotemPositions.GetChild(0).rotation;
        var offset = Vector3.Distance(pos, _seleccionableTotemPositions.GetChild(1).position) / (objectsToSpawn + 1);
        var dir = (_seleccionableTotemPositions.GetChild(1).position - pos).normalized;
        foreach (var totemPiece in infoForSeleccionables)
        {
            pos += dir * offset;
            var totem = Instantiate(totemToInstantiate, pos, rotation, seleccionableTotemParent);
            totem.gameObject.SetActive(true);
            totem.CreateTotem(totemPiece[0].scriptableObjectTienda, totemPiece[1].scriptableObjectTienda, totemPiece[2].scriptableObjectTienda, true);
            var a = totem.gameObject.AddComponent<Seleccionable>();
            a.indexPrefab = 0;
            a.numCartas = 1;
            a.isTotem = true;
            a.objetoACrear = totemToInstantiate.gameObject;
            a.SetInfo(totemPiece[0].scriptableObjectTienda, totemPiece[1].scriptableObjectTienda, totemPiece[2].scriptableObjectTienda);
            // pos += Vector3.down * separationDistance;
        }

        Seleccionable.MaxTotems = playerStore.currentLevel;
        SetDrag(false);
    }
    public void DeleteSeleccionableTotems()
    {
        for(var i = seleccionableTotemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(seleccionableTotemParent.GetChild(i).gameObject);
        }
    }

    public void SpawnTotemPieces()
    {
        SetDrag(false);
        var totemPiecesList = _totemPieces.Where(a => a.Count == 1).ToList();
        var doublePiecesList = _totemPieces.Where(a => a.Count == 2);
        foreach (var totemList in doublePiecesList)
        {
            throw new NotImplementedException();
        }
        var objectsToSpawn = totemPiecesList.Count();
        var separationDistance = distance / objectsToSpawn;
        var pos = totemsPosToSpawn.position;
        foreach (var totemPiece in totemPiecesList)
        { 
            var totem = Instantiate(totemToInstantiate, pos, Quaternion.identity, totemParent);
            // totem.transform.localRotation = Quaternion.Euler(0, 0, 0);
            var aux = new GameObject[] { null, null, null };
            foreach (var piece in totemPiece)
            {
                switch (piece.tag)
                {
                    case "Head":
                        aux[0] = piece.gameObject;
                        break;
                    case "Body":
                        aux[1] = piece.gameObject;
                        break;
                    case "Feet":
                        aux[2] = piece.gameObject;
                        break;
                }
            }
            totem.TotemOffset = 0;
            totem.TotemPieceHover = 0f;
            totem.CreateTotem(aux[0], aux[1], aux[2]);
            pos += Vector3.right * separationDistance;
        }
    }
    public void SpawnSeleccionables()
    {
        SpawnTotemsAsSeleccionables();
        DeleteSeleccionables();
        var objectsToSpawn = _biomes.Count;
        
        var separationDistance = selecDistance / objectsToSpawn;
        var pos = selecPosToSpawn.position;
        foreach (var b in _biomes)
        { 
            var biome = Instantiate(seleccionableToSpawn, pos, Quaternion.identity, seleccionableParent);
            biome.gameObject.SetActive(true);
            biome.indexPrefab = b.Key.indexBioma;
            biome.numCartas = b.Value;
            biome.inventory = this;
            biome.objetoACrear = b.Key.biomeObject;
            biome.SetInfo(b.Key);
            biome.CanDropEnemySide = true;
            biome.isTotem = false;
            biome.GetComponent<Image>().sprite = b.Key.image;
            // totem.SetInfo(totemPiece[0].scriptableObjectTienda, totemPiece[1].scriptableObjectTienda, totemPiece[2].scriptableObjectTienda);
            pos += Vector3.right * separationDistance;
        }
        // foreach (var totemPiece in infoForSeleccionables)
        // { 
        //     var totem = Instantiate(seleccionableToSpawn, pos, Quaternion.identity, seleccionableParent);
        //     totem.gameObject.SetActive(true);
        //     totem.SetInfo(totemPiece[0].scriptableObjectTienda, totemPiece[1].scriptableObjectTienda, totemPiece[2].scriptableObjectTienda);
        //     pos += Vector3.down * separationDistance;
        // }
    }

    public void UseBiome(ScriptableObjectTienda biomeIndex)
    {
        if (!_biomes.ContainsKey(biomeIndex)) return;
        
        _biomes[biomeIndex] -= 1;
        if (_biomes[biomeIndex] <= 0)
        {
            _biomes.Remove(biomeIndex);
        }
    }
    
    public void DeleteSeleccionables()
    {
        for(var i = seleccionableParent.childCount - 1; i >= 0; i--)
        {
            Destroy(seleccionableParent.GetChild(i).gameObject);
        }
    }
}
