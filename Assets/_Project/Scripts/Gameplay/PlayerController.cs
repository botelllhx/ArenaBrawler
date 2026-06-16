using Arena.Gameplay.Data;
using Fusion;
using UnityEngine;

namespace Arena.Gameplay
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private BrawlerDefinition _definition;

        public BrawlerDefinition Definition => _definition;

        public void SetDefinition(BrawlerDefinition definition)
        {
            _definition = definition;
        }
    }
}
