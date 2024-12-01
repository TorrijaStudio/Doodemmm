using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class casilla : NetworkBehaviour
{
    private GameObject biome;
    private GameObject previousBiome;
    private Material previousMaterial;
    private NavMeshModifier _navMeshModifier;
    private int previousIndexArea;
    private Material originalMaterial;
    private Material material;
    private Color _localMaterialSeleccionable; 
    private MeshRenderer _meshRenderer;
    public string side;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _navMeshModifier = GetComponent<NavMeshModifier>();
        originalMaterial = _meshRenderer.material;
        material = originalMaterial;
        var terreno = transform.root.GetComponent<terreno>();
        var postitionGrid = terreno.PositionToGrid(transform.position);
        side = postitionGrid.y < terreno.grid.y/2 ? "HostSide" : "ClientSide";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Getters and Setters

    public void SetPreviousBiome(GameObject o)
    {
        previousBiome = o;
    }
    public GameObject GetPreviousBiome()
    {
        return previousBiome;
    }
    
    public void SetBiome(GameObject o)
    {
        biome = o;
    }
    public GameObject GetBiome()
    {
        return biome;
    }
   
    public void SetPreviousMat(Material m)
    {
        previousMaterial = m;
    }
    public Material GetPrevousMat()
    {
        return previousMaterial;
    }
    public void SetMat(Material m)
    {
        material = m;
        
    }
    public Material GetMat()
    {
        return material;
    }
    public void SetAreaNav(int index)
    {
        _navMeshModifier.area = index;
    }

    public int GetAreaNav()
    {
        return _navMeshModifier.area;
    }

    public void SetPreviousIndexArea(int index)
    {
        previousIndexArea = index;
    }

    public int GetPreviousIndexArea()
    {
        return previousIndexArea;
    }
    #endregion

    public void ResetCasilla()
    {
        previousBiome = null;
        biome = null;
        previousIndexArea = 0;
        _navMeshModifier.area = 0;
        previousMaterial = originalMaterial;
        GetComponent<MeshRenderer>().material = originalMaterial;
    }

    public void SetColorSeleccionable(bool canDropEnemySide)
    {
        _localMaterialSeleccionable = _meshRenderer.material.color;
        if (canDropEnemySide)
            _meshRenderer.material.color = Color.green;
        else
        {
            if (IsHost)
                _meshRenderer.material.color = side == "HostSide" ? Color.red : Color.green;
            else
                _meshRenderer.material.color = side == "ClientSide" ? Color.red : Color.green;
        }
    }

    public void SetPreviousColorSeleccionable()
    {
        _meshRenderer.material.color = Color.white;
    }
    

}
