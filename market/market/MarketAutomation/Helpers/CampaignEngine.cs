using MarketAutomation.Data;
using MarketAutomation.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MarketAutomation.Helpers
{
    public static class CampaignEngine
    {
        public static decimal CalculateDiscount(List<POSItem> cartItems, out string discountReason)
        {
            decimal totalDiscount = 0;
            discountReason = "";

            using (var db = new MarketDbContext())
            {
                var activeCampaigns = db.Campaigns.Where(c => c.IsActive && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now).ToList();
                
                foreach(var campaign in activeCampaigns)
                {
                    if (campaign.CampaignType == "3_AL_2_ODE" && campaign.ProductId.HasValue)
                    {
                        var matchingItem = cartItems.FirstOrDefault(i => i.ProductId == campaign.ProductId.Value);
                        if (matchingItem != null && matchingItem.Quantity >= 3)
                        {
                            // Her 3 üründe 1 ürün bedava
                            int freeItems = (int)(matchingItem.Quantity / 3);
                            decimal discount = freeItems * matchingItem.UnitPrice;
                            totalDiscount += discount;
                            discountReason += $"{campaign.CampaignName} (-{discount:C2})\n";
                        }
                    }
                    else if (campaign.CampaignType == "YUZDE_INDIRIM" && campaign.DiscountValue.HasValue)
                    {
                        if (campaign.ProductId.HasValue)
                        {
                            var matchingItem = cartItems.FirstOrDefault(i => i.ProductId == campaign.ProductId.Value);
                            if (matchingItem != null)
                            {
                                decimal discount = (matchingItem.SubTotal * campaign.DiscountValue.Value) / 100;
                                totalDiscount += discount;
                                discountReason += $"{campaign.CampaignName} (-{discount:C2})\n";
                            }
                        }
                    }
                }
            }

            return totalDiscount;
        }
    }

    public class POSItem : System.ComponentModel.INotifyPropertyChanged
    {
        private decimal _quantity;
        private decimal _unitPrice;

        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        
        public decimal Quantity 
        { 
            get => _quantity; 
            set { if (_quantity != value) { _quantity = value; OnPropertyChanged(nameof(Quantity)); OnPropertyChanged(nameof(SubTotal)); } } 
        }

        public decimal UnitPrice 
        { 
            get => _unitPrice; 
            set { if (_unitPrice != value) { _unitPrice = value; OnPropertyChanged(nameof(UnitPrice)); OnPropertyChanged(nameof(SubTotal)); } } 
        }

        public decimal SubTotal => Quantity * UnitPrice;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}
