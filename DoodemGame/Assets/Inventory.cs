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
    private List<List<TotemPiece>> _fullTotemPieces;
    private List<TotemPiece> _totemPieces;
    private Dictionary<ScriptableObjectTienda, int> _biomes;

    [FormerlySerializedAs("boton")] [SerializeField] private playerInfoStore playerStore;



    [Space(8)] [Header("Totem drag variables")]
    private readonly int _absoluteMaxTotems = 5;
    [SerializeField] private Transform totemParent;
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject wall;
    [SerializeField] private float distance;
    [FormerlySerializedAs("posToSpawn")] [SerializeField] private Transform totemsPosToSpawn;
    [SerializeField] private Totem totemToInstantiate;
    [SerializeField] private GameObject lockSprite;
    [SerializeField] private Transform spriteParent;

    [Space(5)] [Header("Totem pieces variables")]
    [SerializeField] private Transform piecesParent;
    [SerializeField] private Transform topCornerToSpawnPieces;

    [SerializeField] private Vector2Int totemPiecesDrawerSize;
    [SerializeField] private Vector2 totemPiecesDrawerSeparation;
    private int _totemPiecePage;
    private int _maxPages;

    [Space(8)][Header("Biome seleccionable variables")]
    [SerializeField] private Seleccionable seleccionableToSpawn;

    [SerializeField] private Transform seleccionableParent;
    [SerializeField] private Transform selecPosToSpawn;
    [SerializeField] private float selecDistance;
        
    [Space(8)][Header("Totem as seleccionables variables")]

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
        return _fullTotemPieces.Count(a => a.Count == 3);
    }
    
    public static Inventory Instance;
    // Start is called before the first frame update
    void Start()
    {
        _biomes = new Dictionary<ScriptableObjectTienda, int>();
        if (Instance) Destroy(gameObject);
        else Instance = this;
        
        _fullTotemPieces = new List<List<TotemPiece>>();
        _totemPieces = new List<TotemPiece>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePageUpDown(int direction)
    {
        //C#'s default math class clamp has an inclusive max, contrary to Unity's
        var page = Math.Clamp(_totemPiecePage + direction, 0, _maxPages - 1);
        Debug.LogWarning("Page: " + page + ", MaxPage: " + _maxPages);
        if(page == _totemPiecePage) return;
        _totemPiecePage = page;
        DespawnPieces();
        SpawnTotemPieces(page);
    }
    
    public bool Contains(TotemPiece piece)
    {
        foreach (var totem in _fullTotemPieces)
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
        _maxPages = Mathf.CeilToInt(_totemPieces.Count / (float)(totemPiecesDrawerSize.x * totemPiecesDrawerSize.y));
    }

    public void GetTotemsFromShop()
    {
        foreach (var obj in playerStore.boughtObjects.Select(objetoTienda => objetoTienda.GetComponent<objetoTienda>()).Where(objT => !objT.info.isBiome))
        {
            Debug.Log(obj.name);
            if (obj.info.objectsToSell.Count == 1)
            {
                _totemPieces.Add(obj.info.objectsToSell[0]);
            }
            else
            {
                _fullTotemPieces.Add(obj.info.objectsToSell);
            }
        }
        // Debug.Log("Now we have "+ _fullTotemPieces.Count + " totems");
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
        DespawnSprites();
        DespawnPieces();
        DespawnTotems();
    }

    public void DespawnSprites()
    {
        for(int i = spriteParent.childCount - 1; i >= 0; i--)
        {
            var child = spriteParent.GetChild(i);
            Destroy(child.gameObject);
        }
    }
    
    public void DespawnTotems()
    {
        var tempTotemPieces = new List<List<TotemPiece>>();
        for(int i = totemParent.childCount - 1; i >= 0; i--)
        {
            var child = totemParent.GetChild(i);
            var tempInfo = child.GetComponent<Totem>().GetTotem();
            if(tempInfo.Count == 3)
                tempTotemPieces.Add(tempInfo.Select(piece => piece.objectsToSell[0]).ToList());
            else if (tempInfo.Count > 0)
            {
                foreach (var objectTienda in tempInfo)
                {
                    _totemPieces.Add(objectTienda.objectsToSell[0]);
                }
            }
            Destroy(child.gameObject);
        }
        _fullTotemPieces.Clear();
        
        _fullTotemPieces = tempTotemPieces;
        foreach (var totemPiece in _fullTotemPieces)
        {
            var txt = "";
            foreach (var piece in totemPiece)
            {
                txt += "_ " + piece;
            }
            Debug.Log(txt);
        }
    }
    public void DespawnPieces()
    {
        // var tempTotemPieces = new List<TotemPiece>();
        if(piecesParent.childCount > 0)
        {
            for (int i = piecesParent.childCount - 1; i >= 0; i--)
            {
                var child = piecesParent.GetChild(i);
                var tempInfo = child.GetComponent<Totem>().GetTotem();
                if (tempInfo.Count == 1)
                    _totemPieces.Add(tempInfo[0].objectsToSell[0]);
                Destroy(child.gameObject);
            }
        }
        // _totemPieces.Clear();
        
        // _totemPieces = tempTotemPieces;
        foreach (var totemPiece in _totemPieces)
        {
            var txt = totemPiece.name;
            Debug.Log(txt);
        }
    }
    public void SpawnTotems()
    {
        SpawnTotemPieces();
        // var objectsToSpawn = _fullTotemPieces.Count;
        var separationDistance = distance / _absoluteMaxTotems;
        var pos = totemsPosToSpawn.position;
        var totemIndex = 0;
        foreach (var totemPiece in _fullTotemPieces)
        {
            totemIndex++;
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
        // Debug.Log($"Totems spawned: {totemIndex}, totems to spawn: {playerStore.currentLevel + 1}");
        //NON ZERO INDEXED ?????? CRIIINGE
        for (;totemIndex <= _absoluteMaxTotems; totemIndex++)
        {
            Debug.Log("Spawning number " + totemIndex);
            if (totemIndex < playerStore.currentLevel + 1)
            {
                var totem = Instantiate(totemToInstantiate, pos, Quaternion.identity, totemParent);
                // totem.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Debug.Log("Spawning empty Totem");
                totem.CreateEmptyTotem();
                // Debug.Log("Spawned new empty totem");
            }
            else
            {
                Instantiate(lockSprite, pos, Quaternion.identity, spriteParent);
            }
            pos += Vector3.right * separationDistance;
            totemIndex++;
        }
        
        SetDrag(true);
        // boton.DeleteShopItems();
    }

    public void SpawnTotemsAsSeleccionables()
    {
        DeleteSeleccionableTotems();
        var infoForSeleccionables = _fullTotemPieces.Where((list => list.Count == 3)).ToList();
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
            a.transform.localScale *= 2f;
            a.indexPrefab = 0;
            a.NumCards = 1;
            a.isTotem = true;
            a.objetoACrear = totemToInstantiate.gameObject;
            a.SetInfo(totemPiece[0].scriptableObjectTienda, totemPiece[1].scriptableObjectTienda, totemPiece[2].scriptableObjectTienda);
            // pos += Vector3.down * separationDistance;
        }

        Seleccionable.MaxTotems = playerStore.currentLevel + 1;
        SetDrag(false);
    }
    public void DeleteSeleccionableTotems()
    {
        for(var i = seleccionableTotemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(seleccionableTotemParent.GetChild(i).gameObject);
        }
    }

    public void SpawnTotemPieces(int page = 0)
    {
        if(_totemPieces.Count == 0) return;
        
        // SetDrag(false);
        var drawerSize = totemPiecesDrawerSize.x * totemPiecesDrawerSize.y;
        var objectsToSpawn = _totemPieces.Count / drawerSize;
        // var totemPiecesList = _fullTotemPieces.Where(a => a.Count == 1).ToList();
        // var doublePiecesList = _fullTotemPieces.Where(a => a.Count == 2);
        if(page >= (_maxPages))
        {
            Debug.LogError("WRONG PAGE + " + page + " max page " + _maxPages);
            page = 0;
        }
        
        // var separationDistance = distance / objectsToSpawn;
        // var pos = topCornerToSpawnPieces.position;
        var indexes = new List<int>();
        bool exit = false;
        for (int y = 0; y < totemPiecesDrawerSize.y; y++)
        {
            for (int x = 0; x < totemPiecesDrawerSize.x; x++)
            {
                var index = y * totemPiecesDrawerSize.x + x + drawerSize * page;
                // Debug.Log("Piece index: " + index + ", totem pieces " + _totemPieces.Count);
                if(_totemPieces.Count <= index)
                {
                    exit = true;
                    break;
                    // return;
                }
                indexes.Add(index);
                var pos = topCornerToSpawnPieces.position;
                pos.x += x * totemPiecesDrawerSeparation.x;
                pos.y -= y * totemPiecesDrawerSeparation.y;
                var totem = Instantiate(totemToInstantiate, pos, Quaternion.identity, piecesParent);
                // totem.transform.localRotation = Quaternion.Euler(0, 0, 0);
                var aux = new GameObject[] { null, null, null };
                var piece = _totemPieces[index];
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
                totem.TotemOffset = 0;
                totem.TotemPieceHover = 0f;
                totem.CreateTotem(aux[0], aux[1], aux[2]);
            }
            if(exit)
                break;
        }

        for (int i = indexes.Count - 1; i >= 0; i--)
        {
            _totemPieces.RemoveAt(indexes[i]);
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
            biome.NumCards = b.Value;
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

    private void OnDestroy()
    {
        Instance = null;
    }
}
