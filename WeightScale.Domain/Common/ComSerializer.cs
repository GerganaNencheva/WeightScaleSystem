﻿//---------------------------------------------------------------------------------
// <copyright file="ComSerializer.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Common
{
    using System;
    using System.Linq;
    using System.Reflection;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using WeightScale.Utility.Helpers;

    /// <summary>
    /// Service for serialization and deserialization.
    /// Serializes IComSerializable objects to byte array
    /// </summary>
    public class ComSerializer
    {
        public byte[] Setialize<T>(T serObj)
        where T : class, IComSerializable
        {
            byte[] output = this.CreateByteArray<T>();

            var properties = typeof(T).GetProperties()
                                      .Where(x =>
                                          x.IsDefined(typeof(ComSerializablePropertyAttribute), true) &&
                                          !x.IsDefined(typeof(NonSerializedAttribute), true));

            foreach (var property in properties)
            {
                this.SerializeProperty(output, property, serObj);
            }

            return output;
        }

        public T Deserialize<T>(byte[] input)
            where T : IComSerializable, new()
        {
            return new T();
        }

      /// <summary>
      /// Creates the byte array.
      /// </summary>
      /// <typeparam name="T">Type of reflected object</typeparam>
      /// <returns>byte array with appropriated length</returns>
        private byte[] CreateByteArray<T>()
        {
            var attr = typeof(T).GetCustomAttributes(true)
                .Where(x => x is ComSerializableClassAttribute)
                .FirstOrDefault() as ComSerializableClassAttribute;

            if (attr == null)
            {
                throw new ArgumentException(
                    "serObject",
                    "Serialized Object must be decorated with ComSerializableClassAttribute");
            }

            return new byte[(int)attr.BlockLength];
        }

        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <typeparam name="T">type of input object</typeparam>
        /// <param name="output">The output.</param>
        /// <param name="property">The property.</param>
        /// <param name="serObj">The serialized object.</param>
        private void SerializeProperty<T>(byte[] output, PropertyInfo property, T serObj)
        {
            var propSerInfo = property.GetCustomAttributes<ComSerializablePropertyAttribute>().FirstOrDefault();
            this.ValidatePropertySerializationParameters(output.Length, propSerInfo, property.Name);

            byte[] serializedProp = this.ConvertPropertyValueToByteArray(property.GetValue(serObj, null), propSerInfo);
            Array.Copy(serializedProp, 0, output, propSerInfo.Offset, serializedProp.Length);
        }

        private byte[] ConvertPropertyValueToByteArray(object p, ComSerializablePropertyAttribute propSerInfo)
        {
            string str;
            if (propSerInfo.SerializeFormat == string.Empty)
            {
                str = string.Format("{0}", p);
            }
            else
            {
                str = string.Format("{0:" + propSerInfo.SerializeFormat + "}", p);
            }

            return str.ToByteArray(propSerInfo.Align, propSerInfo.Length);
        }

        /// <summary>
        /// Validates the property serialization parameters.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="propSerInfo">The property serialization information.</param>
        /// <param name="propName">The property name.</param>
        private void ValidatePropertySerializationParameters(int length, ComSerializablePropertyAttribute propSerInfo, string propName)
        {
            if (propSerInfo.Length > length)
            {
                throw new ArgumentOutOfRangeException(
                    "Property Length",
                    string.Format("The serialization length of property {0}: {1} is greater than length of resul byte array.", propName, propSerInfo.Length));
            }

            if (propSerInfo.Length + propSerInfo.Offset > length)
            {
                string message = @"The serialization offser of property {0} is outside boundary of array. 
The last char position must be less than {1}. 
Actual position of last char is {2}.";
                throw new ArgumentOutOfRangeException(
                    "Property Length",
                    string.Format(message, propName, length, propSerInfo.Offset + propSerInfo.Length));
            }
        }
    }
}