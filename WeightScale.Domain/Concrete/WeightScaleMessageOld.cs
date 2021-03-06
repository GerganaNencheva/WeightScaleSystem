﻿//---------------------------------------------------------------------------------
// <copyright file="OldWeightScaleMessage.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Domain.Concrete
{
    using System;
    using System.Linq;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Class for transportation of messages from and to Weight Scales communicating over old protocol
    /// </summary>
    [ComSerializableClass(BlockLen.OldProtocol)]
    public class WeightScaleMessageOld : WeightScaleMessageBase
    {
        private const int GROSS_TOTAL_MIN_VAL = 0;
        private const int GROSS_TOTAL_MAX_VAL = 999999999;
        private const int NET_TOTAL_MIN_VAL = 0;
        private const int NET_TOTAL_MAX_VAL = 999999999;

        private string productName;
        private int? totalOfGrossWeight;
        private int? totalOfNetWeight;

        [ComSerializableProperty(length: 12, offset: 54, originalType: typeof(string), serializeFormat: "")]
        public string ProductName
        {
            get { return this.productName; }
            set { this.productName = value; }
        }

        [ComSerializableProperty(length: 9, offset: 84, originalType: typeof(int?), serializeFormat: "")]
        public int? TotalOfGrossWeight
        {
            get { return this.totalOfGrossWeight; }
            set { this.totalOfGrossWeight = value; }
        }

        [ComSerializableProperty(length: 9, offset: 93, originalType: typeof(int?), serializeFormat: "")]
        public int? TotoalOfNetWeight
        {
            get { return this.totalOfNetWeight; }
            set { this.totalOfNetWeight = value; }
        }

        public override ValidationMessageCollection Validate()
        {
            var validationResult = new ValidationMessageCollection();
            validationResult.AddRange(base.Validate());

            // Validate ProductName
            if (string.IsNullOrEmpty(this.productName) || string.IsNullOrWhiteSpace(this.productName))
            {
                validationResult.AddError("ProductName", "The ProductName cannot be empty.");
            }

            // Validate TotalOfGrossWeight
            if (GROSS_TOTAL_MIN_VAL > this.totalOfGrossWeight || this.totalOfGrossWeight > GROSS_TOTAL_MAX_VAL)
            {
                string message = "The value of TotalOfGrossWeight must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalOfGrossWeight",
                    string.Format(message, GROSS_TOTAL_MIN_VAL, GROSS_TOTAL_MAX_VAL, this.totalOfGrossWeight));
            }

            // Validate NetTotalizer
            if (NET_TOTAL_MIN_VAL > this.totalOfNetWeight || this.totalOfNetWeight > NET_TOTAL_MAX_VAL)
            {
                string message = "The value of TotalOfNetWeight must be between {0} and {1}. The actual value is {2}.";
                validationResult.AddError(
                    "TotalOfNetWeight",
                string.Format(message, NET_TOTAL_MIN_VAL, NET_TOTAL_MAX_VAL, this.totalOfNetWeight));
            }

            return validationResult;
        }

        /// <summary>
        /// Provides data block to be sent as Weight scale message block element.
        /// </summary>
        /// <returns>byte[] - Array of bytes</returns>
        public override byte[] ToBlock()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
