using System.Collections;
using UnityEngine;

namespace CMP.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;   
        [SerializeField] private AudioSource loopSource;  
        [SerializeField] private AudioSource wakaSource;  

        [Header("Clips")]
        [SerializeField] private AudioClip startClip;
        [SerializeField] private AudioClip dieClip;
        [SerializeField] private AudioClip sirenClip;
        [SerializeField] private AudioClip siren2Clip;
        [SerializeField] private AudioClip wakaClip;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (wakaSource != null && wakaClip != null)
            {
                wakaSource.clip = wakaClip;
                wakaSource.loop = true;
                wakaSource.playOnAwake = false;
            }
        }

        public void PlayStartSequence()
        {
            StartCoroutine(StartSequenceRoutine());
        }

        private IEnumerator StartSequenceRoutine()
        {
            loopSource.Stop();
            StopWaka();

            if (startClip != null)
            {
                sfxSource.PlayOneShot(startClip);
                yield return new WaitForSeconds(startClip.length);
            }

            PlaySiren(false);
        }

        public void PlayDieSound()
        {
            StopAllCoroutines();
            loopSource.Stop();
            StopWaka();

            if (dieClip != null)
            {
                sfxSource.PlayOneShot(dieClip);
            }
        }

        public void PlaySiren(bool isChase = false)
        {
            AudioClip targetClip = isChase ? siren2Clip : sirenClip;
            if (targetClip == null) return;

            if (loopSource.clip == targetClip && loopSource.isPlaying) return;

            loopSource.clip = targetClip;
            loopSource.loop = true;
            loopSource.Play();
        }

        public void StopLoop()
        {
            loopSource.Stop();
        }
        
        public void StartWaka()
        {
            if (wakaSource != null && !wakaSource.isPlaying)
            {
                wakaSource.Play();
            }
        }

        public void StopWaka()
        {
            if (wakaSource != null && wakaSource.isPlaying)
            {
                wakaSource.Stop(); 
            }
        }
    }
}