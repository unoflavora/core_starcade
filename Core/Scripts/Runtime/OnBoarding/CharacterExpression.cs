using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agate.Starcade
{
    [CreateAssetMenu(fileName = "ExpressionData", menuName = "FTUE/ExpressionData", order = 1)]
    public class CharacterExpression : ScriptableObject
    {
        public List<Expression> ListCharacterExpression;
    }

    [Serializable]
    public class Expression
    {
        public int Id;
        public Sprite Sprite;
    }
}
