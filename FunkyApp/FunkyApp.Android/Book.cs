﻿﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace FunkyApp.Droid
{
    class Book
    {
        // Google Books Reference
        public string Id { get; set; }
        public string GUrl { get; set; }

        // Volume Info
        public VolumeInfo VolumeInfo { get; set; }
        
        // Sale Info
        public SaleInfo SaleInfo { get; set; }

        public override string ToString()
        {
            return
                "ID: " + Id + "\n" +
                "Link: " + GUrl + "\n" +
                "\n" + "Volume Info" + "\n" + VolumeInfo + "\n" +
                "\n" + "Sale Info" + "\n" + SaleInfo;
        }
    }
}