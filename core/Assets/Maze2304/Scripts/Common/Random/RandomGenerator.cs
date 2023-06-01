using UnityEngine;
using Rand = UnityEngine.Random;

namespace Common.Random
{
    public class RandomGenerator
    {
        private Rand.State state;

        public RandomGenerator(int seed)
        {
            var prevState = Rand.state;
            Rand.InitState(xxHash(seed, 12345U));
            this.state = Rand.state;
            Rand.state = prevState;
        }

        public int Range(int min, int max)
        {
            var prevState = Rand.state;
            Rand.state = this.state;
            
            var result = Rand.Range(min, max);

            this.state = Rand.state;
            Rand.state = prevState;

            return result;
        }

        public float Range(float min, float max)
        {
            var prevState = Rand.state;
            Rand.state = this.state;
            
            var result = Rand.Range(min, max);

            this.state = Rand.state;
            Rand.state = prevState;

            return result;
        }

        int xxHash(int data, uint seed)
        {
            const uint PRIME32_2 = 2246822519U;
            const uint PRIME32_3 = 3266489917U;
            const uint PRIME32_4 = 668265263U;
            const uint PRIME32_5 = 374761393U;
            uint h32 = seed + PRIME32_5 + 4U;
            h32 += (uint)data * PRIME32_3;
            h32 = (h32 << 17) | (h32 >> 15);
            h32 *= PRIME32_4;
            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            return (int)h32;
        }
    }
}
