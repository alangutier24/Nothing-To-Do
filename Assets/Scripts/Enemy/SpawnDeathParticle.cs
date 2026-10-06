using UnityEngine;


public class SpawnDeathParticle : MonoBehaviour
{
    [Header("Spawn Particle Reference")]
    public StoneTrollHealthScript scriptReference;
  
    public void SpawnDeathParticles()
    {
        scriptReference.SpawnDeathParticles();
    }

}
