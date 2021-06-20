using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public struct BoidSpawningComponent : IComponentData
{
	public uint seed;
	public int spawnCount;
	public float maxDistanceFromSpawner;
	public Entity prefab;
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class BoidSpawningSystem : SystemBase
{
	private BeginInitializationEntityCommandBufferSystem commandBufferSystem;

	// Runs when system is created
	protected override void OnCreate()
	{
		commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
	}

	// Runs whenever entities exist with BoidSpawningComponent
	protected override void OnUpdate()
	{
		// Instantiate command buffer for spawning commands
		EntityCommandBuffer.ParallelWriter commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

		// Spawn boids
		Entities
			.WithAll<BoidSpawningComponent>()
			.ForEach((Entity entity, int entityInQueryIndex, in BoidSpawningComponent spawningComponent, in LocalToWorld localToWorld) =>
		{
			Unity.Mathematics.Random rng = new Unity.Mathematics.Random(spawningComponent.seed);

			// Instatiate prefabs
			for (int index = 0; index < spawningComponent.spawnCount; ++index)
			{
				Entity instance = commandBuffer.Instantiate(entityInQueryIndex, spawningComponent.prefab);

				commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = rng.NextFloat3() * spawningComponent.maxDistanceFromSpawner });
			}

			// Destroy spawner
			commandBuffer.DestroyEntity(entityInQueryIndex, entity);
		}).ScheduleParallel();

		// Inherit this system's dependencies
		commandBufferSystem.AddJobHandleForProducer(Dependency);
	}
}
