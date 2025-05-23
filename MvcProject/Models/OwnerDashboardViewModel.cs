﻿using System.Domain.Models;

namespace MvcProject.Models
{
    public class OwnerDashboardViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public List<MenuCategory> Categories { get; set; }
        public List<Order> Orders { get; set; }
        public List<AssistanceRequest> AssistanceRequests { get; set; }
    }
}