using UnityEngine;

namespace Arena.Gameplay.Data
{
    [CreateAssetMenu(fileName = "NewBrawler", menuName = "Arena/Brawler Definition")]
    public class BrawlerDefinition : ScriptableObject
    {
        [Header("Identidade")]
        public string BrawlerName;

        [Header("Stats")]
        public int MaxHealth = 3800;
        public float MoveSpeed = 6f;
        public int MaxAmmo = 3;
        public float RechargeTimePerCharge = 1.5f;

        [Header("Combate")]
        public AttackDefinition Attack;
        public SuperDefinition Super;
        public float SuperChargeRequired = 3000f;
    }
}
