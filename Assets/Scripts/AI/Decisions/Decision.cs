using System;
using UnityEngine;

namespace AI
{
    [Serializable]
    public struct DecisionData
    {
        [Range(0, 5)]
        public float weight;
        public ConsiderationHandler considerationHandler;
    }

    [Serializable]
    public abstract class Decision : ScriptableObject
    {
        [SerializeField]
        protected DecisionData decisionData;
        private IAIAgent targetAgent;

        public virtual IAIAgent GetTarget { get{ return targetAgent; } }
        public abstract Type GetDecisionType();
        
        public virtual float EvaluateScore(Intelligence intelligence, IAIAgent[] possibleTargets)
        {
            Consideration[] considerations = decisionData.considerationHandler.considerations;
            float finalScore = decisionData.weight;

            for (int i = 0; i < considerations.Length; i++)
            {
                Consideration currentConsideration = considerations[i];
                float considerationScore = currentConsideration.Score(intelligence, ref possibleTargets);
                float evaluatedScore = currentConsideration.Evaluate(considerationScore);

                if(evaluatedScore == 0.0f)
                {
                    return 0.0f;
                }

                finalScore *= Mathf.Clamp01(evaluatedScore);

            }

            if(possibleTargets.Length <= 0)
            {
                return 0.0f;
            }

            targetAgent = possibleTargets[0];

            return finalScore;
        }

    }
}
