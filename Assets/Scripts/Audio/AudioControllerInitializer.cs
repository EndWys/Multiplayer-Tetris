using UnityEngine;
using VContainer;
using AudioSettings = TetrisNetwork.AudioSettings;

namespace TetrisNetwork
{
    public class AudioControllerInitializer : MonoBehaviour
    {
        [SerializeField] AudioSettings _audioSettings;
        [SerializeField] GameObject _audioSourcesParent;

        [Inject]
        public void Construct()
        {
            var controller = new AudioController();
            controller.Initialize(_audioSettings, _audioSourcesParent);
        }
    }
}
