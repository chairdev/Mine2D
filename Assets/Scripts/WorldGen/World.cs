using System.IO;
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

    public string worldName = "DEBUG_WORLD";
    public long seed;
    public int renderDistance = 2;

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

        for (int x = -renderDistance; x < renderDistance; x++)
        {
            for (int y = -renderDistance; y < renderDistance; y++)
            {
                for (int z = -renderDistance; z < renderDistance; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(playerChunk.x + x, playerChunk.y + y, 0);
                    Debug.Log(chunkPos);
                    if (!IsChunkLoaded(chunkPos))
                    {
                        //if not, load it
                        LoadChunk(chunkPos);
                    }
                }
            }
        }

        //unload the ones that are too far away
        for (int i = 0; i < loadedChunks.Count; i++)
        {
            Vector3Int max = new Vector3Int(playerChunk.x + renderDistance, playerChunk.y + renderDistance, 0);
            Vector3Int min = new Vector3Int(playerChunk.x - renderDistance, playerChunk.y - renderDistance, 0);
            if(loadedChunks[i].position.x > max.x || loadedChunks[i].position.y > max.y || loadedChunks[i].position.x < min.x || loadedChunks[i].position.y < min.y)
            {
                loadedChunks[i].chunkState = ChunkState.OUT_OF_VIEW;
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

    void LoadChunk(Vector3Int chunkPos)
    {
        Chunk chunk = new Chunk();
        //look for file
        //if file exists, load it
        if (File.Exists(Application.persistentDataPath + "/" + worldName + "/chunk/" + GetChunkFileName(chunkPos) + ".chunk"))
        {
            
        }
        else
        {
            chunk = WorldGenerator.Instance.GenerateChunk(chunkPos);
        }
        //if file does not exist, generate it
        loadedChunks.Add(chunk);

    }

    string GetChunkFileName(Vector3Int chunkPos)
    {
        return chunkPos.x + "_" + chunkPos.y + "_" + chunkPos.z + ".chunk";
    }
}