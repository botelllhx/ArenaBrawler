using Fusion;
using UnityEngine;

namespace Arena.Gameplay.Data
{
    [CreateAssetMenu(fileName = "NewAttack", menuName = "Arena/Attack Definition")]
    public class AttackDefinition : ScriptableObject
    {
        [Header("Projétil")]
        public NetworkObject ProjectilePrefab;
        public float ProjectileSpeed = 20f;

        [Header("Dano e Alcance")]
        public int Damage = 1000;
        public float Range = 10f;

        [Header("Dispersão")]
        public int ProjectileCount = 1;
        public float SpreadAngle;
    }
}
