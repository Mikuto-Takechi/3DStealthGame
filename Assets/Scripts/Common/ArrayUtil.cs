using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonstersDomain.Common
{
    public class ArrayUtil : MonoBehaviour
    {
        public static bool CheckIndexOutOfRange<T>(IEnumerable<T> enumerable, int index)
        {
            return enumerable.Count() > index && 0 <= index;
        }
        public static int CircularBuffer(int num, int length)
        {
            if (num < 0)
            {
                return length - 1;
            }

            return num % length;
        }
    }
}
