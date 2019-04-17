using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBWTPaket5.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWTPaket5.Controllers
{
    public class ProdukteController : Controller
    {


  
    

    
        public ActionResult Index()
        {
            int selectedCategory;
            Boolean available = false;
            Boolean vegetarian = false;
            Boolean vegan = false;

            if (Request.QueryString["available"] != null) {
                available = (int.Parse(Request.QueryString["available"].ToString()) == 1);
            }
            if (Request.QueryString["onlyVegetarian"] != null) {
                vegetarian = (int.Parse(Request.QueryString["onlyVegetarian"].ToString()) == 1);
            }
            if (Request.QueryString["onlyVegan"] != null) {
                vegan = (int.Parse(Request.QueryString["onlyVegan"].ToString()) == 1);
            }
            ViewBag.available = available;
            ViewBag.vegetarisch = vegetarian;
            ViewBag.vegan = vegan;

           try
            {
                selectedCategory = int.Parse(Request.Params["category"].ToString());
            }
            catch
            {
                selectedCategory = 0;
            }
            ViewBag.selectedCategory = selectedCategory;



            List<Produkte> meineProdukteListe = Functions.GetProdukte(available,vegetarian,vegan,selectedCategory);
  
           




            return View(meineProdukteListe);
        }

        public ActionResult Detail(int id = 0) {
            Produkte meinProdukt = Functions.GetProdukteByIDForDetail(id);
            if (meinProdukt.ID == 0)
            {
                return Redirect("~/Home/Index/");
            }
            return View(meinProdukt);
        }

        public ActionResult Zutaten()
        {
            List<Zutat> meineZutatenListe = Functions.GetZutatenListe();
            return View(meineZutatenListe);
        }
    }
}