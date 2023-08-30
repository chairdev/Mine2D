using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    public List<GameObject> chunkObjects = new List<GameObject>();

    public GameObject blockPrefab;
    public BlockScriptable[] blockScriptables;
    public GameObject[] blockDepth;

    void Update()
    {
        for(int i = 0; i < World.Instance.loadedChunks.Count; i++)
        {
            Chunk chunk = World.Instance.loadedChunks[i];
            switch(chunk.state)
            {
                case ChunkState.LOADING:
                    CreateChunkOLD(chunk);
                break;
                case ChunkState.DIRTY:
                    //RENDER CHUNK
                break;
                case ChunkState.OUT_OF_VIEW:
                    //DESTROY CHUNK
                break;
            }

        }
        
    }   

    void CreateChunkMesh(Chunk chunk)
    {
        string chunkName = GetChunkName(chunk.position);
        GameObject chunkObject = new GameObject(chunkName);
        chunkObject.transform.position = new Vector3(chunkX * Chunk.chunkSize.x, chunkY * Chunk.chunkSize.y, chunkZ * Chunk.chunkSize.z);

        int totalBlocks = Chunk.chunkSize.x * Chunk.chunkSize.y * Chunk.chunkSize.z;

        chunkObject.AddComponent<MeshFilter>();
        chunkObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        var vertices = new Vector3[];
        List<Vector2> uvs = new List<Vector2>();

        int drawnBlocks = 0;
    }

    void CreateChunkOLD(Chunk chunk)
    {
        string chunkName = GetChunkName(chunk.position);
        GameObject chunkObject = new GameObject(chunkName);
        chunkObject.transform.position = new Vector3(chunkX * Chunk.chunkSize.x, chunkY * Chunk.chunkSize.y, chunkZ * Chunk.chunkSize.z);

        int drawnBlocks = 0;
        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    if (chunk[x, y, z].id != BlockID.AIR && !IsBlockAtOffset(x, y, z, 0, -1, 1))
                    {
                        Vector3 blockPosition = new Vector3(x, y + (z * 0.5f), 0);
                        drawnBlocks++;
                        //this is a 2d sprite
                        GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.identity, chunkObject.transform);
                        SpriteRenderer blockRenderer = block.GetComponent<SpriteRenderer>();
                        BlockScriptable blockScriptable = blockScriptables[(int)chunk[x, y, z].id];

                        blockRenderer.sprite = blockScriptable.sprite;
                        blockRenderer.sortingOrder = z;

                        SpriteRenderer[] sideRenderers = new SpriteRenderer[3];
                        GameObject[] sidePrefabs = new GameObject[] { blockDepth[0], blockDepth[1], blockDepth[2] };

                        for (int i = 0; i < sideRenderers.Length; i++)
                        {
                            if ((i == 0 && x == 0) || (i == 1 && x == Chunk.chunkSize.x - 1) || (i == 2 && y == Chunk.chunkSize.y - 1))
                            {
                                continue;
                            }

                            if ((i == 0 && chunk[x - 1, y, z].id == BlockID.AIR) ||
                                (i == 1 && chunk[x + 1, y, z].id == BlockID.AIR) ||
                                (i == 2 && chunk[x, y + 1, z].id == BlockID.AIR))
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

    string GetChunkName(Vector3Int chunkPosition)
    {
        return string chunkName = "Chunk (" + chunkPosition.x + ", " + chunkPosition.y + ", " + chunkPosition.z + ")";
    }  
}