using UnityEngine;
using UnityEngine.InputSystem;

public class TilesGen : MonoBehaviour
{
    public string imageFileName;
    private Texture2D mTextureOriginal;

    private Tile mTile = null;
    private Sprite mSprite = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateBaseTexture();

    }


    void CreateBaseTexture()
    {
        mTextureOriginal = SpriteUtils.LoadTexture(imageFileName);
        if(!mTextureOriginal.isReadable)
        {
            Debug.LogError("Texture is not readable.");
            return;
        }

        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        mSprite = SpriteUtils.CreateSpriteFromTexture2D(
            mTextureOriginal,
            0,
            0,
            mTextureOriginal.width,
            mTextureOriginal.height);
        spriteRenderer.sprite = mSprite;
    }

    private(Tile.PosNegType, Color) GetRandomType()
    {
        Tile.PosNegType type = Tile.PosNegType.POS;
        Color color = Color.blue;
        float rand = Random.Range(0f, 1f);

        if(rand < 0.5f)
        {
            type = Tile.PosNegType.POS;
            color = Color.blue;
        }
        else
        {
            type = Tile.PosNegType.NEG;
            color = Color.red;
        }
        return (type, color);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TestRandomCurves();
        }
        else if(Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            TestTileFloodFill();
        }
    }

    void TestRandomCurves()
    {
        if(mTile != null)
        {
            mTile.DestroyAllCurves();
            mTile = null;
        }

        Tile tile = new Tile(mTextureOriginal);
        mTile = tile;

        var type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.UP, type_color.Item1, type_color.Item2);
        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.RIGHT, type_color.Item1, type_color.Item2);
        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.DOWN, type_color.Item1, type_color.Item2);
        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.LEFT, type_color.Item1, type_color.Item2);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = mSprite;
    }

    void TestTileFloodFill()
    {
        if (mTile != null)
        {
            mTile.DestroyAllCurves();
            mTile = null;
        }

        Tile tile = new Tile(mTextureOriginal);
        mTile = tile;

        var type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.UP, type_color.Item1, type_color.Item2);
        mTile.SetCurveType(Tile.Direction.UP, type_color.Item1);

        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.RIGHT, type_color.Item1, type_color.Item2);
        mTile.SetCurveType(Tile.Direction.RIGHT, type_color.Item1);

        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.DOWN, type_color.Item1, type_color.Item2);
        mTile.SetCurveType(Tile.Direction.DOWN, type_color.Item1);
        
        type_color = GetRandomType();
        mTile.DrawCurve(Tile.Direction.LEFT, type_color.Item1, type_color.Item2);
        mTile.SetCurveType(Tile.Direction.LEFT, type_color.Item1);

        mTile.Apply(); // To execute floodfill

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            mTile.finalCut,
            0,
            0,
            mTile.finalCut.width,
            mTile.finalCut.height);

        spriteRenderer.sprite = sprite;
    }
}
