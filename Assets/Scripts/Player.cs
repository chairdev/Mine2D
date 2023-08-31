using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player: MonoBehaviour
{
    public System.Action PositionChanged;
    public System.Action CellChanged;

    // public Vector3 Position3D
    // {
    //     get => _position3d;
    //     set
    //     {
    //         if (value != _position3d)
    //         {
    //             _position3d;
    //             transform.position = VolumetricPositionToSurfacePosition(value);
    //             PositionChanged?.Invoke();
    //         }
    //     }
    // }
    private Vector3 _position3d =  new Vector3(0.0f, 0.0f, 0.0f);

    //z is depth. 2d has no depth. this is. yoo ni tie two D. so if you use transform.position.z, it will be zero.
    //therefore, making it innacurate. so, all entities, get a fake z value. this is the z value of the cell they are in.
    //if you are in a different chunk, there is a possibility of colliding with a block that is not in the same chunk but are
    //on the same world position.

    
}