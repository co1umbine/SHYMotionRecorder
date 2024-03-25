using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Co1umbine.SHYMotionRecorder
{
    public static class ObjectExtension
    {
        /// <summary>
        /// ディープコピーの複製を作る拡張メソッド
        /// SerializableでないものやUnityのクラス、構造体は不可
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter
                  = new System.Runtime.Serialization
                        .Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}