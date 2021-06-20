﻿using System;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public class Notification : BaseEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}