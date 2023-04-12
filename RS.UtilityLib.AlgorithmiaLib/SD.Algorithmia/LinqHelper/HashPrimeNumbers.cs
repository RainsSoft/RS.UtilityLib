﻿
namespace SD.System_Linq
{
    using System;
    static class HashPrimeNumbers
    {
        static readonly int[] primeTbl = {
                        11,
                        19,
                        37,
                        73,
                        109,
                        163,
                        251,
                        367,
                        557,
                        823,
                        1237,
                        1861,
                        2777,
                        4177,
                        6247,
                        9371,
                        14057,
                        21089,
                        31627,
                        47431,
                        71143,
                        106721,
                        160073,
                        240101,
                        360163,
                        540217,
                        810343,
                        1215497,
                        1823231,
                        2734867,
                        4102283,
                        6153409,
                        9230113,
                        13845163
                };


        //
        // Private static methods
        //
        public static bool TestPrime(int x) {
            if ((x & 1) != 0) {
                int top = (int)Math.Sqrt(x);

                for (int n = 3; n < top; n += 2) {
                    if ((x % n) == 0)
                        return false;
                }
                return true;
            }
            // There is only one even prime - 2.
            return (x == 2);
        }

        public static int CalcPrime(int x) {
            for (int i = (x & (~1)) - 1; i < Int32.MaxValue; i += 2) {
                if (TestPrime(i)) return i;
            }
            return x;
        }

        public static int ToPrime(int x) {
            for (int i = 0; i < primeTbl.Length; i++) {
                if (x <= primeTbl[i])
                    return primeTbl[i];
            }
            return CalcPrime(x);
        }

    }
}

