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
            Debug.Log(chunk.chunkState);
            switch(chunk.chunkState)
            {
                case ChunkState.LOADING:
                    World.Instance.loadedChunks[i].chunkState = ChunkState.CLEAN;
                    CreateChunkOLD(chunk);
                    
                break;
                case ChunkState.DIRTY:
                    //RENDER CHUNK
                break;
                case ChunkState.OUT_OF_VIEW:
                    Destroy(chunkObjects[i]);
                    World.Instance.loadedChunks.RemoveAt(i);
                break;
            }

        }
        
    }   

    void CreateChunkMesh(Chunk chunk)
    {
        string chunkName = GetChunkName(chunk.position);
        GameObject chunkObject = new GameObject(chunkName);

        chunkObject.AddComponent<MeshFilter>();
        chunkObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        int drawnBlocks = 0;


        //draw 16x24 quads for each block
        Vector3[] vertices = new Vector3[Chunk.chunkSize.x * Chunk.chunkSize.y * Chunk.chunkSize.z * 4];
        int[] triangles = new int[Chunk.chunkSize.x * Chunk.chunkSize.y * Chunk.chunkSize.z * 6];
        Vector2[] uv = new Vector2[vertices.Length];
        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    if (chunk[x, y, z].id != BlockID.AIR && !WorldGenerator.IsBlockAtOffset(chunk, x, y, z, 0, -1, 1))
                    {
                        Vector3 blockPosition = new Vector3(x, y + (z * 0.5f), 0);
                        //vertices
                        vertices[vertexIndex++] = blockPosition;
                        vertices[vertexIndex++] = blockPosition + new Vector3(1, 0, 0);
                        vertices[vertexIndex++] = blockPosition + new Vector3(1, 1, 0);
                        vertices[vertexIndex++] = blockPosition + new Vector3(0, 1, 0);

                        //triangles
                        triangles[triangleIndex++] = vertexIndex - 4;
                        triangles[triangleIndex++] = vertexIndex - 3;
                        triangles[triangleIndex++] = vertexIndex - 2;
                        triangles[triangleIndex++] = vertexIndex - 4;
                        triangles[triangleIndex++] = vertexIndex - 2;
                        triangles[triangleIndex++] = vertexIndex - 1;

                        //uv
                        uv[vertexIndex - 4] = new Vector2(0, 0);
                        uv[vertexIndex - 3] = new Vector2(1, 0);
                        uv[vertexIndex - 2] = new Vector2(1, 1);
                        uv[vertexIndex - 1] = new Vector2(0, 1);

                        drawnBlocks++;
                    }
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        chunkObject.GetComponent<MeshFilter>().mesh = mesh;
        chunkObject.GetComponent<MeshRenderer>().material = blockScriptables[0].sprite;


        
        chunkObject.transform.position = new Vector3(chunk.position.x * Chunk.chunkSize.x, (chunk.position.y * Chunk.chunkSize.y) + ((chunk.position.z * Chunk.chunkSize.z) * 0.5f), 0);
        chunkObjects.Add(chunkObject);
        Debug.Log("Number of Blocks:" + drawnBlocks);
    }

    void CreateChunkOLD(Chunk chunk)
    {
        string chunkName = GetChunkName(chunk.position);
        GameObject chunkObject = new GameObject(chunkName);
        Debug.Log(chunk.position.x);

        int drawnBlocks = 0;
        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    if (chunk[x, y, z].id != BlockID.AIR && !WorldGenerator.IsBlockAtOffset(chunk, x, y, z, 0, -1, 1))
                    {
                        Vector3 blockPosition = new Vector3(x, y + (z * 0.5f), 0);
                        drawnBlocks++;
                        //this is a 2d sprite
                        GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.identity, chunkObject.transform);
                        SpriteRenderer blockRenderer = block.GetComponent<SpriteRenderer>();
                        BlockScriptable blockScriptable = blockScriptables[(int)chunk[x, y, z].id];

                        //blockRenderer.sprite = blockScriptable.sprite;
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
                                sideRenderer.sortingOrder = z+1;
                                Debug.Log("Z:" + z);
                                sideRenderers[i] = sideRenderer;
                            }
                        }
                    }
                }
            }
        }

        chunkObject.transform.position = new Vector3(chunk.position.x * Chunk.chunkSize.x, (chunk.position.y * Chunk.chunkSize.y) + ((chunk.position.z * Chunk.chunkSize.z) * 0.5f), 0);
        chunkObjects.Add(chunkObject);
        Debug.Log("Number of Blocks:" + drawnBlocks);
    } 

    string GetChunkName(Vector3Int chunkPosition)
    {
        return "Chunk (" + chunkPosition.x + ", " + chunkPosition.y + ", " + chunkPosition.z + ")";
    }  
}