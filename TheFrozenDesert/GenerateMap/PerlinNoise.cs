using System;

namespace TheFrozenDesert.GenerateMap
{
    internal sealed class PerlinNoise
    {
        #region Feilds

        private static readonly Random sRandom = new Random();

        #endregion

        private static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }
        //3 step plan for Perlin noise

        //1. Generating White noise


        private static float[][] GenerateWhiteNoise(int width, int height)
        {
            var noise = GetEmptyArray<float>(width, height);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    noise[i][j] = (float) sRandom.NextDouble() % 1;
                }
            }

            return noise;
        }

        private static T[][] GetEmptyArray<T>(int width, int height)
        {
            var image = new T[width][];

            for (var i = 0; i < width; i++)
            {
                image[i] = new T[height];
            }

            return image;
        }

        //2. Generating Smooth noise
        private static float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
        {
            var width = baseNoise.Length;
            var height = baseNoise[0].Length;

            var smoothNoise = GetEmptyArray<float>(width, height);

            var samplePeriod = 1 << octave; // calculates 2 ^ k
            var sampleFrequency = 1.0f / samplePeriod;

            for (var i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                var sampleI0 = i / samplePeriod * samplePeriod;
                var sampleI1 = (sampleI0 + samplePeriod) % width; //wrap around
                var horizontalBlend = (i - sampleI0) * sampleFrequency;

                for (var j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    var sampleJ0 = j / samplePeriod * samplePeriod;
                    var sampleJ1 = (sampleJ0 + samplePeriod) % height; //wrap around
                    var verticalBlend = (j - sampleJ0) * sampleFrequency;

                    //blend the top two corners
                    var top = Interpolate(baseNoise[sampleI0][sampleJ0],
                        baseNoise[sampleI1][sampleJ0],
                        horizontalBlend);

                    //blend the bottom two corners
                    var bottom = Interpolate(baseNoise[sampleI0][sampleJ1],
                        baseNoise[sampleI1][sampleJ1],
                        horizontalBlend);

                    //final blend
                    smoothNoise[i][j] = Interpolate(top, bottom, verticalBlend);
                }
            }

            return smoothNoise;
        }

        //3. Merging all probability fields 

        private static float[][] GeneratePerlinNoise(float[][] baseNoise, int octaveCount)
        {
            var width = baseNoise.Length;
            var height = baseNoise[0].Length;

            var smoothNoise = new float[octaveCount][][]; //an array of 2D arrays containing

            var persistance = 0.7f;

            //generate smooth noise
            for (var i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            var perlinNoise = GetEmptyArray<float>(width, height); //an array of floats initialised to 0

            var amplitude = 1.0f;
            var totalAmplitude = 0.0f;

            //blend noise together
            for (var octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (var i = 0; i < width; i++)
                {
                    for (var j = 0; j < height; j++)
                    {
                        perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
                    }
                }
            }


            //normalisation
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    perlinNoise[i][j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }


        public float[][] GeneratePerlinNoise(int width, int height, int octaveCount)
        {
            var baseNoise = GenerateWhiteNoise(width, height);

            return GeneratePerlinNoise(baseNoise, octaveCount);
        }
    }
}