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
        public void Construct(AudioController controller)
        {
            controller.Initialize(_audioSettings, _audioSourcesParent);
        }
    }
}
