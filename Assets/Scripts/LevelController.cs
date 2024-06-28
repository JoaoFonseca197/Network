using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Transform[] _spawners;

    public Vector3 GetSpawnPoint(ulong ID)
    {
        return _spawners[ID-1].position;
    }

}
