using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using formulas;
using tienda;
using TMPro;
using Totems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class playerInfoStore : MonoBehaviour
{

    public List<GameObject> boughtObjects = new();

    public GameObject[] shopGameObjects;
    public GameObject[] initialSelectionGameObjects;
    public GameObject[] totemMixingGameObjects;
    
    [Space(20)]
    [SerializeField]
    private objetoTienda objTiendaPrefab;
    public List<ScriptableObjectTienda> objectsTiendas;
    public List<ScriptableObjectTienda> totemsTienda;

    [SerializeField] private Transform[] positionsToSpawn;
    [SerializeField] private Transform totemItems;
    [SerializeField] public GameObject botones;
    [FormerlySerializedAs("Ready")] [SerializeField] public GameObject ready;
    [Space(20)]
    [InspectorLabel("TEXTS", "Interface texts :)")]
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI selectedMoneyText;
    [SerializeField] private TextMeshProUGUI reRollCostText;
    [SerializeField] private TextMeshProUGUI experienceCostText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private GameObject fondoJunglaTienda;
    public Inventory inventory;
    public bool canOnlyChooseOne;
    private objetoTienda _selectedObject;
    private Vector3 _prevCameraPos;
    public Transform _cameraPos;
    public Quaternion cameraRot;
    public bool isFirstTime = true;
    public int playerMoney;
    public static readonly Color UnavailableColor = new Color(0.67f, 0.17f, 0.11f);
    public static readonly Color AvailableColor = new Color(0.25f, 0.53f, 0f);

    private Experience _experiencePrice;
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceCost;

    // private PieceTotem _pieceTotemFormula;
    private void UpdateExperienceTexts()
    {
        experienceCostText.SetText(experienceCost.ToString());
        levelText.SetText($"LEVEL {currentLevel.ToString()}");
        experienceText.SetText($"{currentExperience.ToString()}/3");
    }

    public int PlayerMoney
    {
        get => playerMoney;
        set
        {
            playerMoney = Math.Max(0, value);
            selectedMoneyText.SetText(_selectedItemsCost.ToString());
            playerMoneyText.SetText(playerMoney.ToString());
            Debug.Log(playerMoney);
            OnItemSelected.Invoke();
        }
    }
    private int _selectedItemsCost;

    public int SelectedItemsCost
    {
        get => _selectedItemsCost;
        set
        {
            _selectedItemsCost = Math.Max(0, value);
            // Debug.Log("Objeto tienda (de)seleccionado " + OnItemSelected.Method.Name + " new precio: " + _selectedItemsCost + " player " + playerMoney);
            selectedMoneyText.SetText(_selectedItemsCost.ToString());
            playerMoneyText.SetText(playerMoney.ToString());
            OnItemSelected.Invoke();
        }
    }

    public Action OnItemSelected;
    
    public bool CanBuyItem(int cost)
    {
        return SelectedItemsCost + cost <= playerMoney;
    }

    public objetoTienda SelectedObject
    {
        get => _selectedObject;
        set
        {
            if(_selectedObject)
                _selectedObject.selected = false;
            _selectedObject = value;
        }
    }

    public void TryBuyExperience()
    {
        if (CanBuyItem(experienceCost))
        {
            currentExperience++;
            if (currentExperience >= 3)
            {
                currentExperience = 0;
                currentLevel++;
                PlayerMoney -= experienceCost;
                experienceCost = _experiencePrice.GetExperience(currentLevel);
            }
            UpdateExperienceTexts();
        }
    }
    
    private void MoveCameraToShop()
    {
        fondoJunglaTienda.SetActive(true);
        _reRollsThisRound = 1;
        UpdateReRollCost();
        experienceCost = _experiencePrice.GetExperience(currentLevel);
        selectedMoneyText.SetText(_selectedItemsCost.ToString());
        playerMoneyText.SetText(playerMoney.ToString());
        UpdateExperienceTexts();
        var cam = Camera.main.transform;
        _prevCameraPos = cam.position;
        cameraRot = cam.rotation;
        // Debug.LogWarning("camara camara camaramsd asfddjasd " + _prevCameraPos);
        cam.SetPositionAndRotation(_cameraPos.position, _cameraPos.rotation);
        
    }
    
    private void Start()
    {
        // InitialSelection();
        OnItemSelected += SetButtonsTextColour;
        // SetVisibleAll(false);
    }

    public void CloseShopAfterTimer()
    {
        // Debug.LogWarning("PREVIOUS POSITION: " + _prevCameraPos);
        Camera.main!.transform.SetPositionAndRotation(_prevCameraPos, cameraRot);
        if (inventory.GetFullTotems() == 0)
        {
            boughtObjects.Add(totemItems.GetChild(0).gameObject);
            inventory.GetTotemsFromShop();
        }
        DeleteShopItems();
        if(inventory.IsDragActive())
            inventory.DespawnItems();
        
        SetVisibleAll(false);
        // playerMoneyText.gameObject.SetActive(false);
        // fondoJunglaTienda.SetActive(false);
    }

    private int _reRollsThisRound;
    private int _reRollCost;
    public void TryReRollShop()
    {
        if(!isFirstTime && CanBuyItem(_reRollCost))
        {
            _reRollsThisRound++;
            PlayerMoney -= _reRollCost;
            GenerateShop();
            UpdateReRollCost();
        }
    }

    private void SetButtonsTextColour()
    {
        reRollCostText.color = CanBuyItem(_reRollCost) ? AvailableColor : UnavailableColor;
        reRollCostText.SetText(_reRollCost.ToString());
    }
    
    private void UpdateReRollCost()
    {
        var a = 20f;
        const int D = 5;
        var d = 8;
        _reRollCost = Mathf.CeilToInt(a / D + Mathf.Pow(a / d, _reRollsThisRound - 1));
    }
    
    public void DeleteShopItems()
    {
        for(var i = totemItems.childCount - 1; i >= 0; i--)
        {
            Destroy(totemItems.GetChild(i).gameObject);
        }
        boughtObjects.Clear();
        // SetVisible(shopGameObjects, false);
        // ready.SetActive(false);
        // botones.SetActive(false);
    }
    
    // Start is called before the first frame update
    public void InitialSelection()
    {
        isFirstTime = true;
        _experiencePrice = new Experience(20, 1.3f, 5);
        MoveCameraToShop();
        canOnlyChooseOne = true;
        var index = 1;
        var prevTotems = new List<ScriptableObjectTienda>(totemsTienda);
        for (int i = 0; i < 2; i++)
        {
            var objT = Instantiate(objTiendaPrefab, positionsToSpawn[index].position, Quaternion.identity, totemItems);
            var totemI = Random.Range(0, prevTotems.Count);
            objT.CreateObject(prevTotems[totemI], true);
            prevTotems.RemoveAt(totemI);
            index++;
        }
        SetVisible(shopGameObjects, false);
        SetVisible(initialSelectionGameObjects, true);
    }

    private static void SetVisible(GameObject[] gameObjects, bool setActive)
    {
        foreach (var o in gameObjects)
        {
            // Debug.Log("Setting " + o.name + " to " + setActive);
            o.SetActive(setActive);
        }
    }

    private void SetVisibleAll(bool setVisible)
    {
        // Debug.Log(setVisible ? "Showing shop" : "Hiding shop");
        SetVisible(initialSelectionGameObjects, setVisible);
        SetVisible(shopGameObjects, setVisible);
        SetVisible(totemMixingGameObjects, setVisible);
    }

    public void ToggleShopOrMixing(bool visibleShop)
    {
        // Debug.Log("Toggling shop");
        if (isFirstTime)
        {
            inventory.GetItemsFromShop();
        }
        if (!visibleShop)
        {
            inventory.SpawnTotems();
            SetVisible(shopGameObjects, false);
            SetVisible(initialSelectionGameObjects, false);
            SetVisible(totemMixingGameObjects, true);
        }
        else
        {
            SetVisible(totemMixingGameObjects, false);
            SetVisible(shopGameObjects, true);
            inventory.DespawnItems();
            inventory.SetDrag(false);
        }

        if (isFirstTime)
        {
            transform.Find("ShopToggleButtons/SeeShop").gameObject.SetActive(false);
        }
    }
    
    public void GenerateShop()
    {
        SetButtonsTextColour();
        _selectedObject = null;
        SelectedItemsCost = 0;
        
        DeleteShopItems();
        var index = 0;
        //List of objects that can appear in the shop. Totem pieces on the inventory are discarded
        // var spawnableObjects = objectsTiendas.Where(aux => (aux.isBiome || !inventory.Contains(aux.objectsToSell[0]))).ToList();
        var spawnableObjects = new List<ScriptableObjectTienda>(objectsTiendas);
        // Debug.Log(objectsTiendas.Count);
        var spawnedBiomes = 0;
        var spawnedTotems = 0;
        int numOfSpawnables = 4;
        for (int i = 0; i < numOfSpawnables; i++)
        {
            
            if(spawnableObjects.Count == 0) break;
            
            var objT = Instantiate(objTiendaPrefab, positionsToSpawn[index].position, Quaternion.identity, totemItems);
            //Last object to spawn checks if it has spawned at least a biome and at least a totem
            if (i == numOfSpawnables - 1)
            {
                if (spawnedBiomes == 0 || spawnedTotems == 0)
                {
                    //If it hasn't spawned at least one of each, gets which hasn't been spawned and creates a list with only that type of spawnables
                    var hasToSpawnBiome = spawnedBiomes == 0;
                    var tempList = spawnableObjects.Where(aux => aux.isBiome == hasToSpawnBiome);
                    //If it can spawn an item of that type (lenght > 0), it swaps the spawnable items list so that one is selected at random below
                    if (spawnableObjects.Count > 0)
                    {
                        spawnableObjects = tempList.ToList();
                    }
                }
            }
            var objToSpawn = Random.Range(0, spawnableObjects.Count);
            if (spawnableObjects[objToSpawn].isBiome)
            {
                spawnedBiomes++;
            }
            else
            {
                spawnedTotems++;
            }
            
            objT.CreateObject(spawnableObjects[objToSpawn]);
            spawnableObjects.RemoveAt(objToSpawn);
            index++;
            
        }
        SetVisible(shopGameObjects, !GameManager.Instance.startMatchAfterTimer);
    }
    
    public void SetUpShop(int moneyGained)
    {
        isFirstTime = false;
        PlayerMoney += moneyGained;

        
        MoveCameraToShop();
        canOnlyChooseOne = false;
        GenerateShop();
    }

    public void DestroyBoughtObjects()
    {
        for (var i = boughtObjects.Count - 1; i >= 0; i--)
        {
            Destroy(boughtObjects[i]);
        }
        boughtObjects.Clear();
    }
}
