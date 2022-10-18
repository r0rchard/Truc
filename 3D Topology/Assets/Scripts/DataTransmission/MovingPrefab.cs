using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataTransmission
{
    [RequireComponent(typeof(AudioSource))]
    public class MovingPrefab : MonoBehaviour
    {
        AudioSource _source;
        private float _originalPitch;

        public enum StateEnum
        {
            NONE,
            INFLATING,
            DEFLATING,
            MOVING,
            ASCENDING,
            DESCENDING
        }
        private MeshFilter _filter;

        private StateEnum _state = StateEnum.NONE;
        public StateEnum State
        {
            get => _state;
            set
            {
                if (value != _state)
                {
                    _state = value;
                    if(_shouldPlaySound && (value == StateEnum.DEFLATING || value == StateEnum.INFLATING))
                    {
                        if (value == StateEnum.DEFLATING)
                        {
                            _source.clip = _reversedClip;
                        }
                        else
                        {
                            _source.clip = _forwardClip;
                        }
                        StartCoroutine(WaitForAudioToEnd());
                    }
                }
            }
        }

        bool _shouldPlaySound;
        public bool ShouldPlaySound
        {
            get => _shouldPlaySound;
            set
            {
                _shouldPlaySound = value;
            }
        }

        [SerializeField] Mesh _baseMesh;
        [SerializeField] Mesh _changedMesh;
        [SerializeField] AudioClip _forwardClip;
        [SerializeField] AudioClip _reversedClip;

        // Start is called before the first frame update
        void Start()
        {
            if (_source == null)
            {
                _source = GetComponent<AudioSource>();
                _originalPitch = _source.pitch;
            }
            _filter = GetComponent<MeshFilter>();
            _source.enabled = _shouldPlaySound;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetAudioLength(float length)
        {
            if(_source == null)
            {
                _source = GetComponent<AudioSource>();
                _originalPitch = _source.pitch;
            }
            _source.pitch = _originalPitch * _source.clip.length / length;
        }

        public void ReturnToBaseMesh()
        {
            _filter.mesh = _baseMesh;
        }

        public void ModifyMesh()
        {
            _filter.mesh = _changedMesh;
        }

        IEnumerator WaitForAudioToEnd()
        {
            yield return new WaitUntil(() => !_source.isPlaying);
            _source.Play();
        }
    }
}