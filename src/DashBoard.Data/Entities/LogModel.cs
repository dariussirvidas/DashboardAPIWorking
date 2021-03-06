﻿using System;

namespace DashBoard.Data.Entities
{
    public class LogModel
    {
        public int Id { get; set; }
        public int Domain_Id { get; set; }
        public DateTime Log_Date { get; set; }
        public string Error_Text { get; set; }
        public Guid Team_Key { get; set; }
        public bool Notified { get; set; } = false;
        public string Service_Name { get; set; }
    }
}