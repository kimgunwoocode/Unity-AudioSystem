using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSystemConfig audioConfig;
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private int poolCount = 3;

        private Dictionary<string, AudioData> audioMap = new();

        private AudioSource bgmSource;
        private Queue<AudioSource> sfxPool = new();

        private Dictionary<string, (AudioSource source, string key, uint version)> activeSFX = new();

        private Queue<AudioHandle> handlePool = new();
        private uint _nextHandleId = 0;
        private uint _globalVersion = 0;



        #region public API

        // Volume
        /// <summary>
        /// 마스터 볼륨 조절 (0 ~ 1)
        /// </summary>
        /// <param name="volume"></param>
        public static void SetMasterVolume(float volume) => I?.SetMasterVolume_Internal(volume);
        /// <summary>
        /// BGM 볼륨 조절 (0 ~ 1)
        /// </summary>
        /// <param name="volume"></param>
        public static void SetBGMVolume(float volume) => I?.SetBGMVolume_Internal(volume);
        /// <summary>
        /// SFX 볼륨 조절 (0 ~ 1)
        /// </summary>
        /// <param name="volume"></param>
        public static void SetSFXVolume(float volume) => I?.SetSFXVolume_Internal(volume);

        // SFX
        /// <summary>
        /// SFX 재생 (2D전용)
        /// <br></br>
        /// AudioHandle 반환 (재생중인 SFX 제어할 때 사용함)
        /// </summary>
        /// <param name="key">오디오 키</param>
        /// <param name="loop">반복 여부</param>
        /// <returns></returns>
        public static AudioHandle PlaySFX(string key, bool loop = false)
            => I?.PlaySFX_2D(key, loop);

        /// <summary>
        /// SFX 재생 (3D 전용)
        /// <br></br>
        /// AudioHandle 반환 (재생중인 SFX 제어할 때 사용함)
        /// </summary>
        /// <param name="key">오디오 키</param>
        /// <param name="position">재생 위치</param>
        /// <param name="loop">반복 여부</param>
        /// <returns></returns>
        public static AudioHandle PlaySFX(string key, Vector3 position, bool loop = false)
            => I?.PlaySFX_3D(key, position, loop);

        /// <summary>
        /// SFX 중지 (handle)
        /// </summary>
        /// <param name="handle">PlaySFX의 반환값. 고유 ID</param>
        public static void StopSFX(AudioHandle handle) => I?.StopSFX_Internal(handle);

        /// <summary>
        /// SFX 중지 (오디오 키)
        /// <br></br>
        /// 해당하는 모든 SFX 중지
        /// </summary>
        /// <param name="key">오디오 키</param>
        public static void StopSFX(string key) => I?.StopSFX_Internal(key);

        /// <summary>
        /// 모든 효과음 재생을 멈춘다
        /// (주의!!! 현재 모든 Handle은 유효성을 잃게 됩니다)
        /// </summary>
        public static void StopAllSFX() => I?.StopAllSFX_Internal();

        // BGM

        public static void PlayBGM(string key) => I?.PlayBGM_Internal(key);

        public static void StopBGM(float duration = 0f) => I?.StopBGM_Internal(duration);


        public static int GetSFXpoolCount()
        {
            return I.sfxPool.Count;
        }

        public static int GetActiveSFXCount()
        {
            return I.activeSFX.Count;
        }

        #endregion




        #region Singleton

        private static AudioManager Instance;
        private static bool isQuitting;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private static AudioManager I
        {
            get
            {
                if (isQuitting) return null;
                return Instance;
            }
        }

        #endregion


        #region Initialize

        private void Initialize()
        {
            SetupAudioSources();
            BuildAudioMap();
        }
        private void SetupAudioSources()
        {
            // BGM
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];

            // SFX
            for (int i = 0; i < poolCount; i++)
            {
                var source = CreateNewSFXSource();
                sfxPool.Enqueue(source);
            }
        }

        private void BuildAudioMap()
        {
            audioMap.Clear();

            if (audioConfig == null)
            {
                Debug.LogError("[AudioManager] AudioSystemConfig is null");
                return;
            }

            foreach (var db in audioConfig.audioDatabases)
            {
                if (db == null) continue;

                AddToAudioMap(db.bgmList);
                AddToAudioMap(db.sfxList);
            }
        }
        private void AddToAudioMap(IEnumerable<AudioData> list)
        {
            foreach (var s in list)
            {
                if (s == null || string.IsNullOrEmpty(s.key))
                {
                    Debug.LogWarning("[AudioManager] Invalid AudioData (null or empty key)");
                    continue;
                }

                if (audioMap.ContainsKey(s.key))
                {
                    Debug.LogError($"[AudioManager] Duplicate key detected: {s.key} (Skipped)");
                    continue;
                }

                audioMap.Add(s.key, s);
            }
        }

        #endregion






        //------------------------------------------------------------ Internal ------------------------------------------------------------//



        #region volume controler

        // volume : 0~1 범위로 입력하기
        public void SetMasterVolume_Internal(float volume) => SetMixerVolume("MasterVol", volume);
        public void SetBGMVolume_Internal(float volume) => SetMixerVolume("BGMVol", volume);
        public void SetSFXVolume_Internal(float volume) => SetMixerVolume("SFXVol", volume);
        private void SetMixerVolume(string parameterName, float volume)
        {
            volume = Mathf.Clamp01(volume);

            float dB;

            if (volume <= 0.0001f)
            {
                dB = -80f;
            }
            else
            {
                dB = Mathf.Log10(volume) * 20f;
            }

            mainMixer.SetFloat(parameterName, dB);
        }

        #endregion




        #region SFX

        private void ApplyAudioSource(AudioSource source, AudioData audio, bool loop)
        {
            source.enabled = true;
            source.clip = audio.clip;
            source.volume = audio.volume;
            source.pitch = audio.pitch;
            source.loop = loop;
        }


        // 2D 사운드
        public AudioHandle PlaySFX_2D(string key, bool loop = false)
        {
            return PlaySFXInternal(key, null, loop);
        }

        // 위치 기반 (3D)
        public AudioHandle PlaySFX_3D(string key, Vector3 position, bool loop = false)
        {
            return PlaySFXInternal(key, position, loop);
        }

        private AudioHandle PlaySFXInternal(string key, Vector3? position, bool loop)
        {
            if (!audioMap.TryGetValue(key, out var audio)) return null;

            AudioSource source = GetAvailableAudioSource();
            if (source == null) return null;

            // 위치 적용 (3D)
            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = 1f;
            }
            else
            {
                source.spatialBlend = 0f;
            }

            ApplyAudioSource(source, audio, loop);

            if (audio.clip == null) return null;
            source.Play();


            AudioHandle handle = GetHandle(key);
            activeSFX.Add(handle.handleId, (source, key, handle.version));


            if (!loop)
            {
                float duration = audio.clip.length / Mathf.Max(0.01f, audio.pitch);
                StartCoroutine(DisableAfterPlay(handle, duration));
            }

            return handle;
        }


        // 핸들 기반
        public void StopSFX_Internal(AudioHandle handle)
        {
            if (handle == null || string.IsNullOrEmpty(handle.handleId)) return;

            // 유효성 검사
            if (activeSFX.TryGetValue(handle.handleId, out var ver))
                if (ver.version != handle.version)
                    return;


            if (activeSFX.TryGetValue(handle.handleId, out var data))
            {
                ReleaseAudioSource(data.source);
                activeSFX.Remove(handle.handleId);

                ReleaseHandle(handle);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] handleID not found : {handle.handleId}");
            }
        }

        // 레거시 기능 재구현 (key 기반)
        public void StopSFX_Internal(string key)
        {
            if (string.IsNullOrEmpty(key)) return;


            List<string> idsToStop = new List<string>();

            foreach (var pair in activeSFX)
            {
                if (pair.Value.key == key)
                {
                    idsToStop.Add(pair.Key);
                }
            }

            foreach (var id in idsToStop)
            {
                if (activeSFX.TryGetValue(id, out var data))
                {
                    ReleaseAudioSource(data.source);
                    activeSFX.Remove(id);
                }
            }
        }

        public void StopAllSFX_Internal()
        {
            foreach (var pair in activeSFX)
            {
                if (pair.Value.source != null)
                {
                    ReleaseAudioSource(pair.Value.source);
                }
            }
            activeSFX.Clear();
        }


        private IEnumerator DisableAfterPlay(AudioHandle handle, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (activeSFX.ContainsKey(handle.handleId))
            {
                StopSFX_Internal(handle);
            }
            else
            {
                ReleaseHandle(handle);
            }
        }

        #endregion



        #region Pool

        private AudioHandle GetHandle(string key)
        {
            string id = (++_nextHandleId).ToString();
            uint ver = ++_globalVersion;

            AudioHandle handle = (handlePool.Count > 0) ? handlePool.Dequeue() : new AudioHandle();
            handle.Setup(id, key, ver);

            return handle;
        }

        private void ReleaseHandle(AudioHandle handle)
        {
            handle.Reset();
            handlePool.Enqueue(handle);
        }

        private AudioSource GetAvailableAudioSource()
        {
            if (sfxPool.Count > 0)
            {
                return sfxPool.Dequeue();
            }

            // 부족하면 생성
            return CreateNewSFXSource();
        }

        private void ReleaseAudioSource(AudioSource source)
        {
            source.Stop();
            source.enabled = false;
            source.clip = null;

            source.spatialBlend = 0f;

            sfxPool.Enqueue(source);
        }

        private AudioSource CreateNewSFXSource()
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            source.enabled = false;
            return source;
        }

        #endregion



        #region BGM

        public void PlayBGM_Internal(string key)
        {
            if (audioMap.TryGetValue(key, out var audio))
            {
                bgmSource.clip = audio.clip;
                bgmSource.volume = audio.volume;
                bgmSource.pitch = audio.pitch;
                bgmSource.loop = true;

                bgmSource.Play();
            }
        }

        public void StopBGM_Internal(float duration = 0f)
        {
            if (duration <= 0f)
            {
                bgmSource.Stop();
            }
            else
            {
                StartCoroutine(FadeOutBGM(duration));
            }
        }

        private IEnumerator FadeOutBGM(float duration)
        {
            float startVolume = bgmSource.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }

            bgmSource.Stop();
            bgmSource.volume = startVolume;
        }

        #endregion

    }

    public class AudioHandle
    {
        public string handleId; // 고유 ID
        public string key;      // 오디오 키
        public uint version; // 세대(토큰) 번호

        public void Setup(string id, string key, uint ver)
        {
            this.handleId = id;
            this.key = key; 
            this.version = ver;
        }

        public void Reset()
        {
            handleId = null;
            key = null;
        }
    }

}