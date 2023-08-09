using System;

namespace RandCracker
{
    public class RandCracker
    {
        private Int32[] state = new Int32[56];
        private Int32 offset;
        private Int32 index1;
        private Int32 index2;

        public RandCracker(Int32[] series)
        {
            Feed(series);
            offset = 21;
            index1 = 1;
            index2 = index1 + offset;
        }

        public void Feed(Int32[] series)
        {
            if (series.Length < 55)
            {
                throw new ArgumentException("I need at least 55 numbers");
            }
            for (Int32 i = 0; i < 55; i++)
            {
                state[i + 1] = series[i + series.Length - 55];
            }
        }

        public int PredictNext()
        {
            return PredictInternalSample();
        }

        public int PredictNext(int minValue, int maxValue)
        {
            if(minValue > maxValue)
            {
                throw new ArgumentException("min has to be smaller then max");
            }
            long range = (long)maxValue - minValue;
            if(range <= (long)Int32.MaxValue)
            {
                return ((int)(PredictSample() * range) + minValue);
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }

        public int PredictNext(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue has to be positive");
            }
            return (int)(PredictSample() * maxValue);
        }

        public double PredictNextDouble()
        {
            return PredictSample();
        }

        public void PredictNextBytes(byte[] buffer)
        {
            if(buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(PredictInternalSample() % (Byte.MaxValue + 1));
            }
        }

        private int PredictInternalSample()
        {
            index1 = WrapIndex(index1);
            index2 = WrapIndex(index2);

            Int32 prediction = Tick(state[index1], state[index2]);

            state[index1] = prediction;
            index1++;
            index2++;
            return prediction;
        }

        private double PredictSample()
        {
            return PredictInternalSample()*(1.0/Int32.MaxValue);
        }

        private double GetSampleForLargeRange()
        {
            int result = PredictInternalSample();
            bool negative = (PredictInternalSample() % 2 == 0) ? true : false;
            if(negative)
            {
                result = -result;
            }
            double d = result;
            d += (Int32.MaxValue - 1);
            d /= 2*(uint)Int32.MaxValue -1;
            return d;
        }

        private Int32 Tick(Int32 i1, Int32 i2)
        {
            Int32 diff = i1 - i2;
            if (diff == Int32.MaxValue)
            {
                diff -= 1;
            }
            if (diff < 0)
            {
                diff += Int32.MaxValue;
            }
            return diff;
        }

        private Int32 WrapIndex(Int32 index)
        {
            if (index >= 56)
            {
                index = 1;
            }
            return index;
        }
    }
}
