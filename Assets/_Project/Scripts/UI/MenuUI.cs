using Arena.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arena.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _roomCodeInput;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private NetworkBootstrap _networkBootstrap;

        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        private async void OnHostClicked()
        {
            SetButtonsInteractable(false);
            try
            {
                await _networkBootstrap.StartAsHost(GetRoomCode());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Host failed: {e.Message}");
                SetButtonsInteractable(true);
            }
        }

        private async void OnJoinClicked()
        {
            SetButtonsInteractable(false);
            try
            {
                await _networkBootstrap.StartAsClient(GetRoomCode());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Join failed: {e.Message}");
                SetButtonsInteractable(true);
            }
        }

        private string GetRoomCode()
        {
            var code = _roomCodeInput.text.Trim();
            return string.IsNullOrEmpty(code) ? "DefaultRoom" : code;
        }

        private void SetButtonsInteractable(bool interactable)
        {
            _hostButton.interactable = interactable;
            _joinButton.interactable = interactable;
        }
    }
}
