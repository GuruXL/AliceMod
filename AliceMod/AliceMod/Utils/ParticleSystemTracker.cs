using System.Collections.Generic;
using GameManagement;
using ReplayEditor;
using UnityEngine;

namespace AliceMod
{
    class ParticleSystemTrackerStruct
    {
        public List<float> time = new List<float>();
        public List<Vector3> position = new List<Vector3>();
        public List<Quaternion> rotation = new List<Quaternion>();
        public List<bool> isPlaying = new List<bool>();

        public void PushState(float time, Vector3 position, Quaternion rotation, bool isPlaying)
        {
            this.time.Add(time);
            this.position.Add(position);
            this.rotation.Add(rotation);
            this.isPlaying.Add(isPlaying);
        }

        public void Shift()
        {
            this.time.RemoveAt(0);
            this.position.RemoveAt(0);
            this.rotation.RemoveAt(0);
            this.isPlaying.RemoveAt(0);
        }
    }
    class ParticleSystemTracker : MonoBehaviour
    {
        public ParticleSystemTrackerStruct tracker;
        public int BufferFrameCount;
        private ParticleSystem particleSystem;

        void Start()
        {
            tracker = new ParticleSystemTrackerStruct();
            BufferFrameCount = Mathf.RoundToInt(ReplaySettings.Instance.FPS * ReplaySettings.Instance.MaxRecordedTime);
            particleSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (!Main.settings.FX_Sparks_Enabled) return;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                int index = GetFrame();
                if (index >= 0 && tracker.rotation[index] != null && tracker.position[index] != null)
                {
                    transform.rotation = tracker.rotation[index];
                    transform.position = tracker.position[index];

                    if (tracker.isPlaying[index] && !particleSystem.isEmitting)
                    {
                        particleSystem.Play();
                    }
                    else if (!tracker.isPlaying[index] && particleSystem.isEmitting)
                    {
                        particleSystem.Stop();
                    }
                }
            }
            else if (GameStateMachine.Instance.CurrentState is PlayState)
            {
                tracker.PushState(PlayTime.time, transform.position, transform.rotation, particleSystem.isEmitting);

                if (tracker.time.Count >= BufferFrameCount)
                {
                    tracker.Shift();
                }
            }
        }
        int GetFrame()
        {
            float currentTime = ReplayEditorController.Instance.playbackController.CurrentTime;
            int low = 0;
            int high = tracker.time.Count - 1;
            int result = -1;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (tracker.time[mid] < currentTime)
                {
                    low = mid + 1;
                }
                else
                {
                    result = mid;
                    high = mid - 1;
                }
            }
            return result;
        }
    }
}
