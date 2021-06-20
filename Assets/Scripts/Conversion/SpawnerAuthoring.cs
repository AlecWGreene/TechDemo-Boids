using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
	public GameObject GOPrefab;
	public int spawnCount;
	public uint seed;

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(GOPrefab);
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new BoidSpawningComponent
		{
			seed = seed,
			spawnCount = 100,
			prefab = conversionSystem.GetPrimaryEntity(GOPrefab),
			maxDistanceFromSpawner = 100
		});
	}
}
