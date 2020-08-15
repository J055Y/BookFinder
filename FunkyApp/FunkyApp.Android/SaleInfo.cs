﻿namespace FunkyApp.Droid
{
    class SaleInfo
    {
        private char[] charCountryCode;
        public string Country
        {
            get => new string(charCountryCode);
            set => charCountryCode = value.ToCharArray().Length == 2 ? value.ToCharArray() : "??".ToCharArray();
        }

        public BookPrice ListPrice { get; set; }
        public BookPrice RetailPrice { get; set; }
        public string PurchaseLink { get; set; }

        public override string ToString()
        {
            return
                "Country: " + Country + "\n" +
                "List Price: " + ListPrice + "\n" +
                "Retail Price: " + RetailPrice + "\n" +
                "Purchase Link: " + PurchaseLink;
        }
    }
}