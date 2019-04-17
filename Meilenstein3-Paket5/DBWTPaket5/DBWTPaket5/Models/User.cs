using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBWTPaket5.Models
{
    public class User : Controller
    {
      public int Nummer { get; set; }
        public string Nutzername { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Email { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public string Rolle { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }

        public User()
        {
            Nummer = 0;
            Nutzername = "";
            Vorname = "";
            Nachname = "";
            Email = "";
            Geburtsdatum = DateTime.MinValue;
            Rolle = "";
            Salt = "";
            Hash = "";
        }

    }
}