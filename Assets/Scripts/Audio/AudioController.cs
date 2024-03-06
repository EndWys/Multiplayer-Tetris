using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VContainer;

namespace TetrisNetwork
{
    public class AudioController
    {
        private const int AUDIO_SOURCES_AMOUNT = 4;

        private static AudioController _instance;
        public static AudioController Instance => _instance;

        private FieldInfo[] _fields;

        private GameObject _targetGameObject;

        private List<AudioSource> _audioSources = new List<AudioSource>();

        private List<AudioSource> _activeMusic = new List<AudioSource>();
        private List<AudioSource> _activeSounds = new List<AudioSource>();

        private static AudioClip[] _musicAudioClips;
        public static AudioClip[] MusicAudioClips => _musicAudioClips;

        private static Sounds _sounds;
        public static Sounds Sounds => _sounds;

        private static Music _music;
        public static Music Music => _music;

        private static AudioListener _audioListener;
        public static AudioListener AudioListener => _audioListener;

        [Inject] private AudioEventReciver _eventReciver;

        public void Initialize(AudioSettings settings, GameObject targetGameObject)
        {
            _instance = this;
            _targetGameObject = targetGameObject;

            _fields = typeof(Music).GetFields();
            _musicAudioClips = new AudioClip[_fields.Length];

            for (int i = 0; i < _fields.Length; i++)
            {
                _musicAudioClips[i] = _fields[i].GetValue(settings.Music) as AudioClip;
            }

            _music = settings.Music;
            _sounds = settings.Sounds;

            _audioSources.Clear();

            for (int i = 0; i < AUDIO_SOURCES_AMOUNT; i++)
            {
                _audioSources.Add(CreateAudioSourceObject(false));
            }
        }

        public static void CreateAudioListener()
        {
            if (_audioListener != null)
                return;

            GameObject listenerObject = new GameObject("[AUDIO LISTENER]");
            listenerObject.transform.position = Vector3.zero;

            GameObject.DontDestroyOnLoad(listenerObject);

            _audioListener = listenerObject.AddComponent<AudioListener>();
        }

        private static void AddMusic(AudioSource source)
        {
            if (!Instance._activeMusic.Contains(source))
            {
                Instance._activeMusic.Add(source);
            }
        }

        private static void AddSound(AudioSource source)
        {
            if (!Instance._activeSounds.Contains(source))
            {
                Instance._activeSounds.Add(source);
            }
        }

        public static void PlayMusic(AudioClip clip, float volumePercentage = 1)
        {
            if (clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = Instance.GetAudioSource();

            SetSourceDefaultSettings(source, ClipType.Music);

            source.clip = clip;
            source.volume *= volumePercentage;
            source.Play();

            AddMusic(source);
        }


        public static void PlaySound(AudioClip clip, float volumePercentage = 1.0f, float pitch = 1.0f)
        {
            if (clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = Instance.GetAudioSource();

            SetSourceDefaultSettings(source, ClipType.Sound);

            source.clip = clip;
            source.volume *= volumePercentage;
            source.pitch = pitch;
            source.Play();

            AddSound(source);
        }

        private AudioSource GetAudioSource()
        {
            int sourcesAmount = _audioSources.Count;
            for (int i = 0; i < sourcesAmount; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    return _audioSources[i];
                }
            }

            AudioSource createdSource = CreateAudioSourceObject(false);
            _audioSources.Add(createdSource);

            return createdSource;
        }
        private AudioSource CreateAudioSourceObject(bool isCustom)
        {
            AudioSource audioSource = _targetGameObject.AddComponent<AudioSource>();
            SetSourceDefaultSettings(audioSource);

            return audioSource;
        }

        public static void SetSourceDefaultSettings(AudioSource source, ClipType type = ClipType.Sound)
        {
            if (type == ClipType.Sound)
            {
                source.loop = false;
            }
            else if (type == ClipType.Music)
            {
                source.loop = true;
            }

            source.clip = null;

            source.volume = 1.0f;
            source.pitch = 1.0f;
            source.spatialBlend = 0;
            source.mute = false;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = null;
        }
    }
}
