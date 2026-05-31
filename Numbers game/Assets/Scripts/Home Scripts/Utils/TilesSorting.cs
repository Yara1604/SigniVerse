using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesSorting
{
    private List<SpriteRenderer> mSortIndices = new List<SpriteRenderer>();

    public TilesSorting()
    {

    }

    public void Clear()
    {
        mSortIndices.Clear();
    }

    public void Add(SpriteRenderer spriteRenderer)
    {
        mSortIndices.Add(spriteRenderer);
        SetRenderOrder(spriteRenderer, mSortIndices.Count());
    }

    public void Remove(SpriteRenderer spriteRenderer)
    {
        mSortIndices.Remove(spriteRenderer);
        for (int i = 0; i < mSortIndices.Count(); i++)
        {
            SetRenderOrder(mSortIndices[i], i + 1);
        }
    }

    public void BringToTop(SpriteRenderer spriteRenderer)
    {
        Remove(spriteRenderer);
        Add(spriteRenderer);
    }

    public void SetRenderOrder(SpriteRenderer spriteRenderer, int index)
    {
        spriteRenderer.sortingOrder = index;
        Vector3 pos = spriteRenderer.transform.position;
        pos.z = -index / 10.0f;
        spriteRenderer.transform.position = pos;
    }
}
