using UnityEngine;
using System.Collections;

public class BoardGen : MonoBehaviour
{

    [Header("Puzzle Settings")]
    [Tooltip("How many pieces wide should the puzzle be?")]
    public int puzzleColumns = 4;

    private JigsawManager jigsawManager;
    public string imageFileName;
    Sprite mBaseSpriteOpaque;
    Sprite mBaseSpriteTransparent;

    GameObject mGameObjectOpaque;
    GameObject mGameObjectTransparent;

    public float ghostTransparency = 0.1f;

    // Jigsaw tiles creation

    public int numTileX { get; private set; }
    public int numTileY { get; private set; }

    Tile[,] mTiles = null;
    GameObject[,] mTileGameObjects = null;

    public Transform parentForTiles = null;

    Sprite LoadBaseTexture()
    {
        Texture2D originalTex = SpriteUtils.LoadTexture(imageFileName);
        if (!originalTex.isReadable)
        {
            Debug.LogError("Texture is not readable: " + imageFileName);
            return null;
        }
        int targetWidth = puzzleColumns * Tile.tileSize;

        // 2. Calculate height to maintain aspect ratio, then snap it to the nearest 100
        float aspectRatio = (float)originalTex.height / originalTex.width;
        int targetHeight = Mathf.RoundToInt((targetWidth * aspectRatio) / Tile.tileSize) * Tile.tileSize;

        // 3. Create the correctly sized texture
        Texture2D tex = ResizeTexture(originalTex, targetWidth, targetHeight);
        // Add padding on all sides

        Texture2D newTex = new Texture2D(
            tex.width + Tile.padding * 2,
            tex.height + Tile.padding * 2,
            TextureFormat.ARGB32,
            false);

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                newTex.SetPixel(i, j, Color.white);
            }
        }

        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                Color color = tex.GetPixel(x, y);
                color.a = 1.0f;
                newTex.SetPixel(x + Tile.padding, y + Tile.padding, color);
            }
        }
        newTex.Apply();
        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            newTex,
            0,
            0,
            newTex.width,
            newTex.height);

        return sprite;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        jigsawManager = GetComponent<JigsawManager>();

        if (jigsawManager == null)
        {
            Debug.LogError("[BoardGen] Missing JigsawManager component on the same GameObject!", this);
            return;
        }

        mBaseSpriteOpaque = LoadBaseTexture();
        if (mBaseSpriteOpaque == null) return;

        mGameObjectOpaque = new GameObject();
        mGameObjectOpaque.name = imageFileName + "_Opaque";
        mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
        mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

        mBaseSpriteTransparent = CreateTransparentView(mBaseSpriteOpaque.texture);
        mGameObjectTransparent = new GameObject();
        mGameObjectTransparent.name = imageFileName + "_Transparent";
        mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
        mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

        mGameObjectOpaque.gameObject.SetActive(false);

        SetCameraPosition();

        StartCoroutine(Coroutine_CreateJigsawTiles());
    }

    Sprite CreateTransparentView(Texture2D tex)
    {
        Texture2D newTex = new Texture2D(
            tex.width,
            tex.height,
            TextureFormat.ARGB32,
            false);

        for (int x = 0; x < newTex.width; x++)
        {
            for (int y = 0; y < newTex.height; y++)
            {
                Color c = tex.GetPixel(x, y);
                if (x > Tile.padding &&
                   x < (newTex.width - Tile.padding) &&
                   y > Tile.padding &&
                   y < (newTex.height - Tile.padding))
                {
                    c.a = ghostTransparency;
                }
                newTex.SetPixel(x, y, c);
            }
        }
        newTex.Apply();

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
          newTex,
          0,
          0,
          newTex.width,
          newTex.height);
        return sprite;
    }

    void SetCameraPosition()
    {
        Camera.main.transform.position = new Vector3(
          mBaseSpriteOpaque.texture.width / 2,
          mBaseSpriteOpaque.texture.height / 2,
          -10.0f);
        Camera.main.orthographicSize =
          mBaseSpriteOpaque.texture.width / 2;
    }

    public GameObject CreateGameObjectFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "TileGameObj_" +
          tile.xIndex.ToString() +
          "_" +
          tile.yIndex.ToString();

        obj.transform.position = new Vector3(
          tile.xIndex * Tile.tileSize,
          tile.yIndex * Tile.tileSize,
          0.0f);

        SpriteRenderer spriteRenderer =
          obj.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite =
          SpriteUtils.CreateSpriteFromTexture2D(
            tile.finalCut,
            0,
            0,
            Tile.padding * 2 + Tile.tileSize,
            Tile.padding * 2 + Tile.tileSize);

        BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

        TileMovement tileMovement = obj.AddComponent<TileMovement>();
        tileMovement.tile = tile;

        // Added this to have piece report to manager when it snaps in place
        tileMovement.manager = this.jigsawManager;

        if (jigsawManager != null)
        {
            jigsawManager.localSorting.Add(spriteRenderer);
        }

        return obj;
    }

    //void CreateJigsawTiles()
    //{
    //    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    //    numTileX = baseTexture.width / Tile.tileSize;
    //    numTileY = baseTexture.height / Tile.tileSize;

    //    mTiles = new Tile[numTileX, numTileY];
    //    mTileGameObjects = new GameObject[numTileX, numTileY];

    //    for (int i = 0; i < numTileX; i++)
    //    {
    //        for (int j = 0; j < numTileY; j++)
    //        {
    //            mTiles[i, j] = CreateTile(i, j, baseTexture);
    //            mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
    //            if (parentForTiles != null)
    //            {
    //                mTileGameObjects[i, j].transform.SetParent(parentForTiles);
    //            }
    //        }
    //    }
    //}

    IEnumerator Coroutine_CreateJigsawTiles()
    {
        Texture2D baseTexture = mBaseSpriteOpaque.texture;
        numTileX = baseTexture.width / Tile.tileSize;
        numTileY = baseTexture.height / Tile.tileSize;

        mTiles = new Tile[numTileX, numTileY];
        mTileGameObjects = new GameObject[numTileX, numTileY];

        for (int i = 0; i < numTileX; i++)
        {
            for (int j = 0; j < numTileY; j++)
            {
                mTiles[i, j] = CreateTile(i, j, baseTexture);
                mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
                if (parentForTiles != null)
                {
                    mTileGameObjects[i, j].transform.SetParent(parentForTiles);
                }
                yield return null;
            }
        }

        if(jigsawManager != null)
        {
            jigsawManager.StartGame(mTileGameObjects, numTileX, numTileY);
        }
    }

    Tile CreateTile(int i, int j, Texture2D baseTexture)
    {
        Tile tile = new Tile(baseTexture);
        tile.xIndex = i;
        tile.yIndex = j;

        // Left side tiles
        if (i == 0)
        {
            tile.SetCurveType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
        }
        else
        {
            // check the tile on the left, represented by column i – 1,
            // for its RIGHT curve type, and apply opposite
            Tile leftTile = mTiles[i - 1, j];
            Tile.PosNegType rightOp = leftTile.GetCurveType(Tile.Direction.RIGHT);
            tile.SetCurveType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ?
                Tile.PosNegType.POS : Tile.PosNegType.NEG);
        }

        // Bottom tiles
        if(j==0)
        {
            tile.SetCurveType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
        }
        else
        {
            Tile downTile = mTiles[i, j - 1];
            Tile.PosNegType upOp = downTile.GetCurveType(Tile.Direction.UP);
            tile.SetCurveType(Tile.Direction.DOWN, upOp == Tile.PosNegType.NEG ?
                Tile.PosNegType.POS : Tile.PosNegType.NEG);
        }

        // Right side tiles
        if(i == numTileX - 1)
        {
            tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
        }
        else
        {
            float toss = Random.Range(0.0f, 1.0f);
            if(toss < 0.5f)
            {
                tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
            }
            else
            {
                tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
            }
        }

        // Up side
        if(j == numTileY - 1)
        {
            tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NONE);
        }
        else
        {
            float toss = Random.Range(0.0f, 1.0f);
            if (toss < 0.5f)
            {
                tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.POS);
            }
            else
            {
                tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NEG);
            }
        }

        tile.Apply();
        return tile;

    }

    // To resize the image and scale it

    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        // Use a RenderTexture to smoothly scale the image
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        Texture2D nTex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
