using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBWTPaket5.Models
{
    public class Bild
    {
     public string Alt_Text { get; set; }
        public string Titel { get; set; }
        public  string Binaerdaten { get; set; }

        public Bild()
        {
            Alt_Text = "";
            Titel = "";
            Binaerdaten = "";
            
        }

    }
}