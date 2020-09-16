using System;
using System.Globalization;
using System.Linq;
using Heyworks.PocketShooter.Utils;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// This class contains all necessary information (received form e-market) about in-app purchase.
    /// </summary>
    public sealed class PurchaseRepresentationData
    {
        /// <summary>
        /// Gets the unique Id of purchase, this object represents.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the title of purchase, this object represents.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of purchase, this object represents.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the price of purchase (represented by the object) combined with currency symbol.
        /// </summary>
        public string FormatedPrice { get; private set; }

        /// <summary>
        /// Gets the currency symbol, corresponding to the location of the e-market.
        /// </summary>
        public string CurrencySymbol { get; private set; }

        public decimal LocalPrice { get; private set; }

        public string TransactionID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseRepresentationData"/> class.
        /// </summary>
        /// <param name="id"> Id of the purchase. </param>
        /// <param name="title"> Purchase title. </param>
        /// <param name="description"> Purchase description. </param>
        /// <param name="currencySymbol"> Symbol identifying the currency the price for purchase is represented in. </param>
        /// <param name="localPrice"> Price of the purchase. </param>
        /// <param name="transactionID"> Transaction ID. </param>
        /// <exception cref="ArgumentNullException"> Thrown if some of the specified objects is null. </exception>
        public PurchaseRepresentationData(string id, string title, string description, string currencySymbol, decimal localPrice, string transactionID)
        {
            AssertUtils.NotNull(id, "Purchase ID");
            AssertUtils.NotNull(title, "Purchase title");
            AssertUtils.NotNull(description, "Purchase description");
            AssertUtils.NotNull(currencySymbol, "Purchase currency symbol");

            Id = id;
#if UNITY_ANDROID
            Title = title.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
#else
        Title = title;
#endif
            Description = description;
            CurrencySymbol = ChangeCurrencySymbol(currencySymbol);
            FormatedPrice = ChangeCurrencySymbol($"{localPrice.ToString()} {currencySymbol}");
            LocalPrice = localPrice;
            TransactionID = transactionID;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Id: {0}; Title: {1}; Price: {2}; Currency Symbol: {3}; Description: {4}", Id, Title, FormatedPrice, CurrencySymbol, Description);
        }

        private string ChangeCurrencySymbol(string price)
        {
            var resultString = string.Empty;
            for (var i = 0; i < price.Length; i++)
            {
                var s = price[i];
                resultString += ParseSymbol(s);
            }

            return resultString;
        }

        private string ParseSymbol(char c)
        {
            int intSymbol = c;

            switch (intSymbol)
            {
                case 8381:
                    return "RUB";

                case 8376:
                    return "KZT";

                case 8372:
                    return "UAH";

                default:
                    return c.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
