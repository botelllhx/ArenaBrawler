using Fusion;
using UnityEngine;

namespace Arena.Gameplay.Data
{
    public enum SuperType
    {
        SpecialShot,
        Dash,
        Shield
    }

    [CreateAssetMenu(fileName = "NewSuper", menuName = "Arena/Super Definition")]
    public class SuperDefinition : ScriptableObject
    {
        public SuperType Type;

        [Header("SpecialShot")]
        public NetworkObject ProjectilePrefab;
        public int Damage = 1500;
        public float ProjectileSpeed = 20f;
        public int ProjectileCount = 5;
        public float SpreadAngle = 60f;

        [Header("Dash")]
        public float DashDistance = 5f;

        [Header("Shield")]
        public float Duration = 3f;
    }
}
