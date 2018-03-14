namespace AI
{
    public class Intelligence : IAIAgent
    {
        public Decision[] decisions;

        public Decision GetHighestDecision(IAIAgent[] agents)
        {
            float minScore = float.MinValue;
            Decision highestDecision = null;

            for (int i = 0; i < decisions.Length; i++)
            {
                Decision decision = decisions[i];
                float currentDecisionScore = decision.EvaluateScore(this, agents);

                if(currentDecisionScore == 0.0f)
                {
                    continue;
                }

                if(currentDecisionScore > minScore)
                {
                    highestDecision = decision;
                    minScore = currentDecisionScore;
                }
            }

            return highestDecision;
        }
    }
}
