using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField]
        Disjunction[] and;


        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return and.All((dis) => dis.Check(evaluators));
        }
        [System.Serializable]
        class Disjunction
        {
            [SerializeField]
            Predicate[] or;

            public bool Check(IEnumerable <IPredicateEvaluator> evaluators)
            {
                return or.Any((predicate) => predicate.Check(evaluators));
            }
        }


        [System.Serializable]
        class Predicate
        {
            
            //
            [SerializeField]
            string predicate;
            [SerializeField]
            string[] parametrs;
            [SerializeField]
            bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parametrs);
                    if (result == null) continue;

                    if (result == negate) return false;
                }
                return true;
            }
        }
        
    }
}