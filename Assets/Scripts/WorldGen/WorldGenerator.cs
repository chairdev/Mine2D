using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    public GameObject blockPrefab;
    public BlockScriptable[] blockScriptables;
    public GameObject[] blockDepth;
    public Block[,,] blocks;
    public GameObject[,,] blockGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        GenerateChunkPerlin();
        RenderChunk();
    }

    void GenerateChunkPerlin()
    {
        blocks = new Block[chunkSize.x, chunkSize.y, chunkSize.z];
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    float noise = Mathf.PerlinNoise((x + transform.position.x) / 10f, (y + transform.position.y) / 10f);
                    if (noise > 0.5f)
                    {
                        blocks[x, y, z].id = BlockID.STONE;
                    }
                    else
                    {
                        blocks[x, y, z].id = BlockID.AIR;
                    }
                }
            }
        }
    }

    void GenerateTestChunk()
    {
        blocks = new Block[chunkSize.x, chunkSize.y, chunkSize.z];
        //BLOCKS ON Z 0-8 ARE STRONE
        //BLOCKS ON Z 9-15 ARE DIRT
        //BLOCKS ON Z 16 ARE GRASS

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (z < 8)
                    {
                        blocks[x, y, z].id = BlockID.STONE;
                    }
                    else if (z < 15)
                    {
                        if(y != 8)
                        {
                            blocks[x, y, z].id = BlockID.DIRT;
                        }
                        else
                        {
                            blocks[x, y, z].id = BlockID.AIR;
                        }
                    }
                    else if (z == 15)
                    {
                        if(y != 8 || x != 8)
                        {
                            blocks[x, y, z].id = BlockID.GRASS;
                        }
                        else
                        {
                            blocks[x, y, z].id = BlockID.AIR;
                        }
                    }
                    else
                    {
                        blocks[x, y, z].id = BlockID.AIR;
                    }
                }
            }
        }
    }

    void RenderChunk()
    {
        blockGameObjects = new GameObject[chunkSize.x, chunkSize.y, chunkSize.z];
        int drawnBlocks = 0;
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (blocks[x, y, z].id != BlockID.AIR && !IsBlockAtOffset(x, y, z, 0, -1, 1))
                    {
                        Vector3 blockPosition = new Vector3(x, y + (z * 0.5f), 0);
                        drawnBlocks++;
                        //this is a 2d sprite
                        GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.identity);
                        SpriteRenderer blockRenderer = block.GetComponent<SpriteRenderer>();
                        BlockScriptable blockScriptable = blockScriptables[(int)blocks[x, y, z].id];

                        blockRenderer.sprite = blockScriptable.sprite;
                        blockRenderer.sortingOrder = z;

                        blockGameObjects[x, y, z] = block;

                        SpriteRenderer[] sideRenderers = new SpriteRenderer[3];
                        GameObject[] sidePrefabs = new GameObject[] { blockDepth[0], blockDepth[1], blockDepth[2] };

                        for (int i = 0; i < sideRenderers.Length; i++)
                        {
                            if ((i == 0 && x == 0) || (i == 1 && x == chunkSize.x - 1) || (i == 2 && y == chunkSize.y - 1))
                            {
                                continue;
                            }

                            if ((i == 0 && blocks[x - 1, y, z].id == BlockID.AIR) ||
                                (i == 1 && blocks[x + 1, y, z].id == BlockID.AIR) ||
                                (i == 2 && blocks[x, y + 1, z].id == BlockID.AIR))
                            {
                                GameObject side = Instantiate(sidePrefabs[i], blockPosition, Quaternion.identity, block.transform);
                                SpriteRenderer sideRenderer = side.GetComponent<SpriteRenderer>();
                                sideRenderer.color = blockScriptable.lineColor;
                                sideRenderer.sortingOrder = z;
                                sideRenderers[i] = sideRenderer;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("Number of Blocks:" + drawnBlocks);
    }

    void DestroyChunk()
    {
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (blockGameObjects[x, y, z] != null)
                    {
                        Destroy(blockGameObjects[x, y, z]);
                    }
                }
            }
        }
    }

    bool IsBlockAtOffset(int x, int y, int z, int xOffset, int yOffset, int zOffset)
    {
        if (x + xOffset < 0 || x + xOffset >= chunkSize.x || y + yOffset < 0 || y + yOffset >= chunkSize.y || z + zOffset < 0 || z + zOffset >= chunkSize.z)
        {
            return false;
        }
        if (blocks[x + xOffset, y + yOffset, z + zOffset].id == BlockID.AIR)
        {
            return false;
        }
        return true;
    }
    

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public struct Block
    {
        public BlockID id;
    }


    public enum BlockID
    {
        AIR = 0,
        STONE = 1,
        DIRT = 2,
        GRASS = 3,
    }
}
