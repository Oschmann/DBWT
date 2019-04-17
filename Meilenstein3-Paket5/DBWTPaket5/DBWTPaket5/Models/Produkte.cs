using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;


namespace DBWTPaket5.Models
{
    public class Produkte 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Beschreibung { get; set; }
        public bool Verfuegbar { get; set; }
        public decimal Gastpreis { get; set; }
        public decimal Studentpreis { get; set; }
        public decimal MAPreis { get; set; }

        public Produkte() {
            ID = 0;
            Name = "";
            Beschreibung = "";
            Verfuegbar = false;
            Gastpreis = 0;
            Studentpreis = 0;
            MAPreis = 0;
        }

       

      
        }
       
    }
