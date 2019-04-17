using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWTPaket5.Models
{
    public class Kategorien
    {
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
        public int Oberkategorie { get; set; }


        public Kategorien() {
            ID = 0;
            Bezeichnung = "";
            Oberkategorie = 0;
        }

       


    }

}