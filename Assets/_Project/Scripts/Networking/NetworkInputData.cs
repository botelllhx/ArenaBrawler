using Fusion;
using UnityEngine;

namespace Arena.Networking
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 Move;
        public Vector2 Aim;
        public NetworkButtons Buttons;

        public const int ButtonAttack = 0;
        public const int ButtonSuper = 1;
    }
}
