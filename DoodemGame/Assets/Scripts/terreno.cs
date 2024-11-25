using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;


public class terreno : NetworkBehaviour
{
    public Vector2Int grid;

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject tilePrefab;
    private static readonly int ScaleX = Shader.PropertyToID("_ScaleX");
    private static readonly int ScaleY = Shader.PropertyToID("_ScaleY");
    [SerializeField] private bool generate;
    public List<GameObject> casillas = new();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void GenerateTileMap()
    {
        var parent = transform.Find("Tiles");
        if (!parent)
        {
            parent = new GameObject("Tiles").transform;
            parent.SetParent(transform);
        }
        else
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i));
            }
            //TODO: DESTROY EVERYTHING
        }

        for (int x = 0; x < grid.x; x++)
        {
            for (int y = 0; y < grid.y; y++)
            {
                var tile = Instantiate(tilePrefab, GridToPosition(new Vector2Int(x, y)), Quaternion.Euler(90,0,0), parent);
                casillas.Add(tile);
                tile.name = $"[{x}, {y}]";
                var cellSize = new Vector2(transform.lossyScale.x, transform.lossyScale.z) / grid;
                tile.transform.localScale = new Vector3(cellSize.x-0.1f, cellSize.y-0.1f, 1);
            }
        }
    }
    
    public Vector2Int GetGrid()
    {
        return grid;
    }
  
    private void OnValidate()
    {
        _meshRenderer.sharedMaterial.SetFloat(ScaleX, grid.x);
        _meshRenderer.sharedMaterial.SetFloat(ScaleY, grid.y);
        if(generate)
        {
            casillas = new();
            generate = false;
            GenerateTileMap();
        }
    }

    public bool IsInside(Vector3 position)
    {
        
        return _meshRenderer.bounds.Contains(new Vector3(position.x,transform.position.y,position.z)) ;
    }

    public Vector2Int PositionToGrid(Vector3 position)
    {
        var corner = (transform.position - transform.lossyScale / 2F );
        var posInTerreain = position - corner;
        var cellSize = new Vector2(transform.lossyScale.x, transform.lossyScale.z) / grid;
        Vector2Int pos = new Vector2Int((int)(posInTerreain.x / cellSize.x), (int)(posInTerreain.z/cellSize.y) );
        return pos;
    }

    public Vector3 GridToPosition(Vector2Int tilePos)
    {
        var cellSize = new Vector2(transform.lossyScale.x, transform.lossyScale.z) / grid;
        var pos = transform.position - transform.lossyScale / 2F;
       
        pos += new Vector3(tilePos.x * cellSize.x, transform.GetComponent<MeshRenderer>().bounds.max.y + 0.4f, tilePos.y * cellSize.y)
               + new Vector3(cellSize.x/2F,0,cellSize.y/2F);
        return pos;
    }
}
