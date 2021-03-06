﻿using System;
using System.ComponentModel.DataAnnotations;
using DashBoard.Data.Enums;

namespace DashBoard.Data.Entities
{

    public class DomainModel
    {

        //Pamokele: Value types(such as decimal, int, float, DateTime) are inherently required and don't need the [Required] attribute.
        //[BindNever] //padeda nuo over-posting attacks apsisaugot. Neleidzia ideti i DB. Sitas neveikia, tik formose kaip supratau. Ir taip neleidzia nes error: Cannot insert explicit value for identity column in table 'Domains' when IDENTITY_INSERT is set to OFF. 
        public int Id { get; set; }
        [Required]
        public string Service_Name { get; set; }
        [Required]
        [Url]
        public string Url { get; set; }
        [Range(0, 1)]
        public ServiceType Service_Type { get; set; } //EF in SQL enums saves as int.
        [Range(0, 1)]
        public RequestMethod Method { get; set; }
        public bool Basic_Auth { get; set; }
        public string Auth_User { get; set; }
        public string Auth_Password { get; set; } //pagalvoti kaip saugoti PW
        public string Parameters { get; set; } //kaip cia parametrus, jei JSON arba XML ?
        [Required]
        [EmailAddress]
        public string Notification_Email { get; set; }

        [Range(3000, int.MaxValue, ErrorMessage = "Interval ms value must be more than 3000")]
        public int Interval_Ms { get; set; } = 600000; //default, jei nieko neiveda is front-end
        public int Latency_Threshold_Ms { get; set; } = 60000;
        public bool Active { get; set; } = true; //Default reiksme
        public bool Deleted { get; set; }
        public Guid Team_Key { get; set; }
        public int Created_By { get; set; } //Useris negali keisti
        public int Modified_By { get; set; } //Useris negali keisti
        public DateTime Date_Created { get; set; } = DateTime.Now; //Useris negali keisti
        public DateTime Date_Modified { get; set; } = DateTime.Now; //Useris negali keisti
        public DateTime Last_Fail { get; set; } //Useris negali keisti
        public DateTime Last_Notified { get; set; }
    }
}
