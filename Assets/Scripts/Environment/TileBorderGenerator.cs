using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileBorderGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TileBase borderTile;

    [Tooltip("The layer to assign the border collider to")]
    [SerializeField] private int borderLayerIndex = 0; 

    [Header("Automation")]
    [SerializeField] private bool generateOnAwake = true;

    private Tilemap mainTilemap;
    private GameObject borderMapObject;
    private Tilemap borderTilemap;

    private void Awake()
    {
        mainTilemap = GetComponent<Tilemap>();

        if (generateOnAwake)
        {
            GeneratePerimeter();
        }
    }

    [ContextMenu("Generate Perimeter Border")]
    public void GeneratePerimeter()
    {
        if (borderTile == null)
        {
            Debug.LogError("Please assign a 'Border Tile' in the inspector to use as the collider shape.");
            return;
        }
        mainTilemap = GetComponent<Tilemap>();
        //1.Setup the hidden Border Tilemap
        SetupBorderTilemap();

        //2.Clear old border
        borderTilemap.ClearAllTiles();

        //3.Scan the main map and place ghost tiles around the edges
        //CompressBounds ensures we don't scan empty infinity space
        mainTilemap.CompressBounds();
        BoundsInt bounds = mainTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                //If there is a tile in the main map at this position
                if (mainTilemap.HasTile(pos))
                {
                    CheckAndPlaceNeighbor(pos, Vector3Int.up);
                    CheckAndPlaceNeighbor(pos, Vector3Int.down);
                    CheckAndPlaceNeighbor(pos, Vector3Int.left);
                    CheckAndPlaceNeighbor(pos, Vector3Int.right);
                }
            }
        }

        //4.Configure the Collider on the Border Map
        ConfigureCollider();
    }

    private void CheckAndPlaceNeighbor(Vector3Int origin, Vector3Int direction)
    {
        Vector3Int neighborPos = origin + direction;

        //if the neighbor is EMPTY in the main map, place a border tile there
        if (!mainTilemap.HasTile(neighborPos))
        {
            borderTilemap.SetTile(neighborPos, borderTile);
        }
    }

    private void SetupBorderTilemap()
    {
        //find or create the child object for the border
        string borderName = "BorderColliders";
        Transform borderTransform = transform.Find(borderName);

        if (borderTransform == null)
        {
            borderMapObject = new GameObject(borderName);
            borderMapObject.transform.parent = transform;
            borderMapObject.transform.localPosition = Vector3.zero;
            borderMapObject.layer = borderLayerIndex;
        }
        else
        {
            borderMapObject = borderTransform.gameObject;
        }

        //ensure it has the required components
        borderTilemap = borderMapObject.GetComponent<Tilemap>();
        if (borderTilemap == null) borderTilemap = borderMapObject.AddComponent<Tilemap>();

        TilemapRenderer renderer = borderMapObject.GetComponent<TilemapRenderer>();
        if (renderer != null) renderer.enabled = false; // Make the border invisible
    }

    private void ConfigureCollider()
    {
        //1.TilemapCollider
        TilemapCollider2D tmCol = borderMapObject.GetComponent<TilemapCollider2D>();
        if (tmCol == null) tmCol = borderMapObject.AddComponent<TilemapCollider2D>();


        //Instead of deprecated usedByComposite, use compositeOperation
        tmCol.compositeOperation = Collider2D.CompositeOperation.Merge;
        // ----------------------

        //2. Rigidbody (Required for Composite)
        Rigidbody2D rb = borderMapObject.GetComponent<Rigidbody2D>();
        if (rb == null) rb = borderMapObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Static wall

        //3. CompositeCollider (Generates the mesh outline)
        CompositeCollider2D compCol = borderMapObject.GetComponent<CompositeCollider2D>();
        if (compCol == null) compCol = borderMapObject.AddComponent<CompositeCollider2D>();

        compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compCol.generationType = CompositeCollider2D.GenerationType.Synchronous;
    }
}