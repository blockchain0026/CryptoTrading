using CryptoTrading.Services.ExchangeAccess.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets
{
    public class Order : ValueObject
    {
        public int ExchangeId { get; private set; }
        public string OrderId { get; private set; }
        public OrderType OrderType { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalQuantity { get; private set; }
        public decimal ExecutedQuantity { get; private set; }
        public decimal CommisionPaid { get; private set; }
        public DateTime DateCreated { get; private set; }

        public Order(int exchangeId, string orderId, OrderType orderType, OrderStatus orderStatus,
            string baseCurrency, string quoteCurrency, decimal price, decimal originalQuantity, decimal executedQuantity, decimal commisionPaid,
            DateTime dateCreated)
        {
            ExchangeId = exchangeId;
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OrderType = orderType ?? throw new ArgumentNullException(nameof(orderType));
            OrderStatus = orderStatus ?? throw new ArgumentNullException(nameof(orderStatus));
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            Price = price;
            OriginalQuantity = originalQuantity;
            ExecutedQuantity = executedQuantity;
            CommisionPaid = commisionPaid;
            DateCreated = dateCreated;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ExchangeId;
            yield return this.OrderId;
            yield return this.OrderType;
            yield return this.OrderStatus;
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
            yield return this.Price;
            yield return this.OriginalQuantity;
            yield return this.ExecutedQuantity;
            yield return this.CommisionPaid;
            yield return this.DateCreated;
        }
    }
}
