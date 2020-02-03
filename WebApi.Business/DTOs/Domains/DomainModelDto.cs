﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebApi.Business.DTOs.Domains
{
    public class DomainModelDto
    {
        public int Id { get; set; }
        [Required]
        public string Service_Name { get; set; }
        [Required]
        //[Url] uzkomentuoju, nes neaisku, dar kokius linkus pinginsim
        public string Url { get; set; }
        [Required]
        public string Service_Type { get; set; } //cia gal enuma reiktu, jei viena is triju renkames
        public string Method { get; set; } //cia jei webApp, kokia reiksme?
        public bool Basic_Auth { get; set; }
        public string Auth_User { get; set; }
        public string Auth_Password { get; set; } //pagalvoti kaip saugoti PW
        public string Parameters { get; set; } //kaip cia parametrus, jei JSON arba XML ?
        [Required]
        [EmailAddress]
        public string Notification_Email { get; set; }
        public int Interval_Ms { get; set; } = 600000; //default, jei nieko neiveda is front-end
        public bool Active { get; set; } = true; //Default reiksme
        public bool Deleted { get; set; }
        public int Created_By { get; set; } //Useris negali keisti
        public int Modified_By { get; set; } //Useris negali keisti
        public DateTime Date_Created { get; set; } = DateTime.Now; //Useris negali keisti
        public DateTime Date_Modified { get; set; } = DateTime.Now; //Useris negali keisti
        public DateTime Last_Fail { get; set; } //Useris negali keisti
    }
}
