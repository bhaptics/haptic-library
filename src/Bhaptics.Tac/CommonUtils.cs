using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Bhaptics.Tac
{
    public class CommonUtils
    {
        /// <summary>
        /// Determines whether [is device name valid] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is device name valid] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDeviceNameValid(string candidate)
        {
            byte[] name = Encoding.ASCII.GetBytes(candidate);

            if (name.Length > 20)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the current millis.
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentMillis()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            return milliseconds;
        }

        public static T Clamp<T>(T aValue, T aMin, T aMax) where T : IComparable<T>
        {
            var result = aValue;
            if (aValue.CompareTo(aMax) > 0)
                result = aMax;
            else if (aValue.CompareTo(aMin) < 0)
                result = aMin;
            return result;
        }

        /// <summary>
        /// Determines whether [is array equal] [the specified a1].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <returns>
        ///   <c>true</c> if [is array equal] [the specified a1]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArrayEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static bool Parse(string json, out Dictionary<int, HapticFeedbackData> hapticFeedback, out int endTime)
        {
            endTime = -1;
            hapticFeedback = new Dictionary<int, HapticFeedbackData>();
            try
            {
                List<HapticFeedbackData> items = SimpleJson.DeserializeObject<List<HapticFeedbackData>>(json);
                foreach (HapticFeedbackData data in items)
                {
                    hapticFeedback.Add(data.Time, data);
                    endTime = data.Time;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Subs the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Converts the byte array to string.
        /// </summary>
        /// <param name="ba">The ba.</param>
        /// <returns></returns>
        public static string ConvertByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static HapticFeedbackFile ConvertToTactosyFile(string path)
        {
            string texts = FileUtils.ReadString(path);
            return ConvertJsonStringToTactosyFile(texts);
        }

        /// <summary>
        /// Converts the json string to tactosy file.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static HapticFeedbackFile ConvertJsonStringToTactosyFile(string json)
        {
            return Parse(json);
        }

        private static HapticFeedbackFile Parse(string json)
        {
            var obj = SimpleJson.DeserializeObject<HapticFeedbackFileBridge>(json);

            return HapticFeedbackFileBridge.AsTactosyFile(obj);
        }

        /// <summary>
        /// Converts the file to tactosy file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static HapticFeedbackFile ConvertFileToTactosyFile(string path)
        {
            try
            {
                string json = FileUtils.ReadString(path);

                return Parse(json);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("ConvertFileToTactosyFile() : " + exception.Message);
                return null;
            }
        }

        private static readonly DateTime Jan1St1970 = new 
            DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        }

        public static float Max(float v1, float v2)
        {
            return v1 > v2 ? v1 : v2;
        }

        public static float Min(float v1, float v2)
        {
            return v1 > v2 ? v2 : v1;
        }
    }
}
