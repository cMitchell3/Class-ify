using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.CS.Classify{
    public class LinearCongruentialGenerator : MonoBehaviour {
        public long seed;
        private const long a = 1664525; // Multiplier
        private const long c = 1013904223; // Increment variable for some offsett
        private const long m = 4294967296; // Modulus variable. Sets bound (2^32)

        public long Next() {
            if (seed == 0) {
                Debug.LogWarning("Seed not initialized in Unity inspector");
            }

            seed = (a * seed + c) % m;
            return seed;
        }
    }

    // Code for testing Room Code generation

    // class Program {
    //     static void Main(string[] args) {
    //         LinearCongruentialGenerator lcg = new LinearCongruentialGenerator(12345);

    //         for (int i = 0; i < 10; i++) {
    //             Console.WriteLine(lcg.Next());
    //         }
    //     }
    // }
}