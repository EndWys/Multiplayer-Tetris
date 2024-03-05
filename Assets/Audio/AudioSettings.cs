using UnityEngine;

namespace TetrisNetwork
{
    public class AudioSettings : ScriptableObject
    {
        [SerializeField] Music _music;
        public Music Music => _music;

        [SerializeField] Sounds _sounds;
        public Sounds Sounds => _sounds;
    }
}
