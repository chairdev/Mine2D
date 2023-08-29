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
        GenerateChunkPerlin
          ();
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
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (blocks[x, y, z].id != BlockID.AIR)
                    {
                        //this is a 2d sprite
                        GameObject block = Instantiate(blockPrefab, new Vector3(x, y+(z*0.5f), 0), Quaternion.identity);
                        block.GetComponent<SpriteRenderer>().sprite = blockScriptables[(int)blocks[x, y, z].id].sprite;
                        block.GetComponent<SpriteRenderer>().sortingOrder = z;
                        blockGameObjects[x, y, z] = block;

                        //check if there's is not a block to the left
                        if (x == 0 || blocks[x - 1, y, z].id == BlockID.AIR)
                        {
                            GameObject leftSide = Instantiate(blockDepth[0], new Vector3(x, y+(z*0.5f), 0), Quaternion.identity, block.transform);
                            leftSide.GetComponent<SpriteRenderer>().color = blockScriptables[(int)blocks[x, y, z].id].lineColor;
                            leftSide.GetComponent<SpriteRenderer>().sortingOrder = z;
                        }
                        //check if there's is not a block to the right
                        if (x == chunkSize.x - 1 || blocks[x + 1, y, z].id == BlockID.AIR)
                        {
                            GameObject rightSide = Instantiate(blockDepth[1], new Vector3(x, y+(z*0.5f), 0), Quaternion.identity, block.transform);
                            rightSide.GetComponent<SpriteRenderer>().color = blockScriptables[(int)blocks[x, y, z].id].lineColor;
                            rightSide.GetComponent<SpriteRenderer>().sortingOrder = z;
                        }
                        //check if there's is not a block to the top
                        if (y == chunkSize.y - 1 || blocks[x, y + 1, z].id == BlockID.AIR)
                        {
                            GameObject topSide = Instantiate(blockDepth[2], new Vector3(x, y+(z*0.5f), 0), Quaternion.identity, block.transform);
                            topSide.GetComponent<SpriteRenderer>().color = blockScriptables[(int)blocks[x, y, z].id].lineColor;
                            topSide.GetComponent<SpriteRenderer>().sortingOrder = z;
                        }
                        // //check if there's is not a block to the bottom
                        // if (y == 0 || blocks[x, y - 1, z].id == BlockID.AIR)
                        // {
                        //     GameObject bottomSide = Instantiate(blockDepth[3], new Vector3(x, y+z, 0), Quaternion.identity, block.transform);
                        //     bottomSide.GetComponent<SpriteRenderer>().color = blockScriptables[(int)blocks[x, y, z].id].lineColor;
                        //     bottomSide.GetComponent<SpriteRenderer>().sortingOrder = z;
                        // }
                    }
                }
            }
        }

        Debug.Log("Number of Blocks:" + blockGameObjects.Length);
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
