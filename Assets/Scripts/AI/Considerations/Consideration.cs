using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public struct ConsiderationData
    {
        public string name;
        public AnimationCurve curve;
    }

    public abstract class Consideration : ScriptableObject
    {
        [SerializeField]
        protected ConsiderationData considerationData;
        protected List<IAIAgent> filteredAgents;

        protected abstract void FilterAgents(Intelligence intelligence, IAIAgent currentAgent);

        private void OnEnable()
        {
            if(null == filteredAgents)
            {
                filteredAgents = new List<IAIAgent>();
            }
        }

        public virtual float Score(Intelligence intelligence, ref IAIAgent[] agents)
        {
            filteredAgents.Clear();

            for (int i = 0; i < agents.Length; i++)
            {
                IAIAgent currentAgent = agents[i];
                FilterAgents(intelligence, currentAgent);
            }

            agents = filteredAgents.ToArray();
            return filteredAgents.Count > 0 ? 1f : 0f;
        }

        public virtual void Reset()
        {
            considerationData.name = GetType().Name;
            considerationData.curve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1f, 1f));
        }

        public float Evaluate(float score)
        {
            return considerationData.curve.Evaluate(score);
        }
    }
}
