namespace FunkyApp.Droid
{
    class BookPrice
    {
        private double amount;
        private string currencyCode;

        public BookPrice(double mAmount, string mCurrencyCode)
        {
            amount = mAmount;
            currencyCode = mCurrencyCode;
        }

        public override string ToString()
        {
            return amount + " " + currencyCode;
        }
    }
}