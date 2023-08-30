using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public List<Chunk> loadedChunks = new List<Chunk>();
    public static World Instance;
    void Awake()
    {
        Instance = this;
    }

    public long seed;

    public static Vector3 VolumetricPositionToSurfacePosition(Vector3 volumetricPosition)
    {
        return new Vector3
        {
            x = volumetricPosition.x,
            y = volumetricPosition.y + (volumetricPosition.z * 0.5f),
            z = 0.0f,
        };
    } 



    void Update()
    {
        Vector3Int playerChunk = WorldGenerator.PositionToChunk(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        //check if chunks +- 2 are loaded
        // Z Z Z Z Z
        // Z Z X Z Z
        // Z Z Z Z Z
        // Z Z Z Z Z

        for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                for (int z = -2; z < 2; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(playerChunk.x + x, playerChunk.y + y, playerChunk.z + z);
                    if (!IsChunkLoaded(chunkPos))
                    {
                        //if not, load it
                        WorldGenerator.Instance.GenerateChunk(chunkPos.x, chunkPos.y, chunkPos.z);
                    }
                }
            }
        }
    }

    bool IsChunkLoaded(Vector3Int chunkPos)
    {
        for (int i = 0; i < loadedChunks.Count; i++)
        {
            var chunk = loadedChunks[i];
            if (chunk.position == chunkPos)
            {
                return true;
            }  
        }
        return false;
    }
}