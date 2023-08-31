using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Vector2Int noiseOffset = new Vector2Int(0, 0);
    public float noiseScale = 20;

    public static WorldGenerator Instance;
    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        GenerateTestChunk();
    }

    // public void GenerateChunk(int chunkX, int chunkY, int chunkZ)
    // {
    //     GenerateTestChunk();
    // }

    public Chunk GenerateChunk(Vector3Int chunkPosition)
    {
       // blocks = new Block[Chunk.chunkSize.x, Chunk.chunkSize.y, Chunk.chunkSize.z];
        Chunk chunk = new Chunk();
        chunk.position = chunkPosition;
        int surfaceY = 100;
        Vector3Int chunkOffset = ChunkToPosition(chunkPosition);
        


        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    int Value = Mathf.FloorToInt(surfaceY + OpenSimplex2.Noise2(World.Instance.seed, chunkOffset.x + x, chunkOffset.y + y) * noiseScale);

                    if(Value < surfaceY)
                    {
                        chunk[x, y, z].id = BlockID.STONE;
                    }
                    else
                    {
                        chunk[x, y, z].id = BlockID.AIR;
                    }
                }
            }
        }

        chunk.chunkState = ChunkState.LOADING;
        return chunk;
    }

    Chunk GenerateTestChunk()
    {
        Chunk chunk = new Chunk();
        //BLOCKS ON Z 0-8 ARE STRONE
        //BLOCKS ON Z 9-15 ARE DIRT
        //BLOCKS ON Z 16 ARE GRASS

        for (int x = 0; x < Chunk.chunkSize.x; x++)
        {
            for (int y = 0; y < Chunk.chunkSize.y; y++)
            {
                for (int z = 0; z < Chunk.chunkSize.z; z++)
                {
                    if (z < 8)
                    {
                        chunk[x, y, z].id = BlockID.STONE;
                    }
                    else if (z < 15)
                    {
                        chunk[x, y, z].id = BlockID.DIRT;
                    }
                    else if (z == 15)
                    {
                        if(y != 8)
                        {
                            chunk[x, y, z].id = BlockID.GRASS;
                        }
                    }
                }
            }
        }

        return chunk;
    }

    public static bool IsBlockAtOffset(Chunk chunk, int x, int y, int z, int xOffset, int yOffset, int zOffset)
    {
        if (x + xOffset < 0 || x + xOffset >= Chunk.chunkSize.x || y + yOffset < 0 || y + yOffset >= Chunk.chunkSize.y || z + zOffset < 0 || z + zOffset >= Chunk.chunkSize.z)
        {
            return false;
        }
        if (chunk[x + xOffset, y + yOffset, z + zOffset].id == BlockID.AIR)
        {
            return false;
        }
        return true;
    }

    public static Vector3Int PositionToChunk(float x, float y, float z)
    {
        return new Vector3Int(Mathf.FloorToInt(x / Chunk.chunkSize.x), Mathf.FloorToInt(y / Chunk.chunkSize.y), Mathf.FloorToInt(z / Chunk.chunkSize.z));
    }

    public static Vector3Int ChunkToPosition(Vector3Int chunkPosition)
    {
        return new Vector3Int
        {
            x = chunkPosition.x * Chunk.chunkSize.x,
            y = chunkPosition.y * Chunk.chunkSize.y,
            z = chunkPosition.z * Chunk.chunkSize.z,
        };
    }
    

    

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Chunk
{
    public static Vector3Int chunkSize = new Vector3Int(16, 16, 16);

    public Vector3Int position;
    private Block[,,] blocks = new Block[chunkSize.x,chunkSize.y, chunkSize.z];
    public ChunkState chunkState = ChunkState.GENERATING;

    public ref Block this[int x, int y, int z]
    {
        get
        {
            return ref blocks[x, y, z];
        }
    }
}

public enum ChunkState { CLEAN, GENERATING, LOADING, DIRTY, OUT_OF_VIEW }

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