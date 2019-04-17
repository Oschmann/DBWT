using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWTPaket5.Models
{
    public class Zutat : Controller
    {
     
        public string Name { get; set; }
        public Boolean Bio { get; set; }
        public Boolean Vegan { get; set; }
        public Boolean Vegetarisch { get; set; }
        public Boolean Glutenfrei { get; set; }

        public Zutat()
        {
            Name = "";
            Bio = false;
            Vegan = false;
            Vegetarisch = false;
            Glutenfrei = false; 
        }
    }
}