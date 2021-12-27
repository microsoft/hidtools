// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace HidEngine.ReportDescriptorItems
{
    using System;
    using HidEngine.Properties;
    using HidSpecification;

    /// <summary>
    /// Identifies the start of a relationship betwen two or more items.
    /// Must ALWAYS be terminated by an <see cref="EndCollectionItem"/>.
    /// </summary>
    [MainItem(HidConstants.MainItemKind.Collection)]
    public class CollectionItem : ShortItem
    {
        private byte? vendorCollectionKind = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItem"/> class.
        /// </summary>
        /// <param name="kind">Kind of collection.</param>
        public CollectionItem(HidConstants.MainItemCollectionKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItem"/> class.
        /// </summary>
        /// <param name="vendorCollectionKind">Kind of vendor collection.</param>
        public CollectionItem(byte vendorCollectionKind)
        {
            this.VendorCollectionKind = vendorCollectionKind;
        }

        /// <summary>
        /// Gets the kind of collection.
        /// </summary>
        public HidConstants.MainItemCollectionKind Kind { get; private set; }

        /// <summary>
        /// Gets or sets the kind of vendor collection.
        /// </summary>
        public byte? VendorCollectionKind
        {
            get
            {
                return this.vendorCollectionKind;
            }

            set
            {
                this.Kind = HidConstants.MainItemCollectionKind.VendorDefined;

                if (value < HidConstants.VendorCollectionKindStart || value > HidConstants.VendorCollectionKindEnd)
                {
                    throw new ArgumentOutOfRangeException(string.Format(Resources.ExceptionItemInvalidVendorDefinedCollection, HidConstants.VendorCollectionKindStart, HidConstants.VendorCollectionKindEnd, this.VendorCollectionKind));
                }

                this.vendorCollectionKind = value;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Collection({this.Kind})";
        }

        /// <inheritdoc/>
        protected override byte[] GenerateItemData()
        {
            byte[] itemBytes = new byte[1];

            if (this.Kind == HidConstants.MainItemCollectionKind.VendorDefined)
            {
                itemBytes[0] = this.VendorCollectionKind.Value;
            }
            else
            {
                itemBytes[0] = (byte)this.Kind;
            }

            return itemBytes;
        }
    }
}
