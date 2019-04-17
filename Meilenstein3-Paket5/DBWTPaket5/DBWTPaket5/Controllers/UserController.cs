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
    public class UserController : Controller
    {
        
        public ActionResult Login()
        {


            if (string.IsNullOrEmpty(Session["user"] as string))
            {
                string meinUserName = Request["user"];
                User meinUser = Functions.GetUserByNutzerName(meinUserName);
                meinUser.Rolle = Functions.GetRoleByNutzerName(meinUserName);
                if (meinUser.Nummer != 0)
                {
                    if (Functions.IsActive(meinUser.Nummer))
                    {
                        string hash = "sha1:64000:18:" + meinUser.Salt + ":" + meinUser.Hash;
                        if (hash != "" && PasswordSecurity.PasswordStorage.VerifyPassword(Request["password"], hash))
                        {
                            Session["user"] = meinUser.Nutzername;
                            Session["role"] = meinUser.Rolle;

                            return RedirectToAction("Logout");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else { return View(); }
                }
                else
                {
                    return View();
                } }
                
                else
                {
                    return RedirectToAction("Logout");
                }
            }
           

        

        public ActionResult Logout()
        {
            if (!string.IsNullOrEmpty(Session["user"] as string))
            {
                if (Request["logout"] == "true")
                {
                    Session.Clear();
                    Session.Abandon();
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Registrieren()
        {
            if (!string.IsNullOrEmpty(Session["user"] as string))
            {
                return RedirectToAction("Logout");
            }
            else if (Request["role"] != null)
            {
                string selectedRole = Request["role"].ToString();
                if (selectedRole == "Student") {
                    return RedirectToAction("RegistrierenStudent");
                }
                else if (selectedRole == "Mitarbeiter") {
                    return RedirectToAction("RegistrierenMitarbeiter");
                }
                else {
                    return RedirectToAction("RegistrierenGast");
                }
                
            }
            else
            {
                return View();
            }
        }
        public ActionResult RegistrierenStudent() {
            
            if (Request["pwd"] as string != null && Request["pwdrepeat"] as string != null)
            {
               string pwd1 = Request["pwd"].ToString();
               string pwd2 = Request["pwdrepeat"].ToString();

                if (pwd1 != pwd2)
                {
                    return View();
                }
                else
                {
                    User meinUser = new User();
                    meinUser.Nutzername = Request["nutzername"];
                    meinUser.Vorname = Request["vorname"];
                    meinUser.Nachname = Request["nachname"];
                    meinUser.Email = Request["email"];
                    meinUser.Geburtsdatum = Convert.ToDateTime(Request["birthdate"]);
                    string hashedPasword = PasswordSecurity.PasswordStorage.CreateHash(Convert.ToString(Request["pwd"]));
                    string[] hashedPasswordSplitted = hashedPasword.Split(':');
                    meinUser.Salt = hashedPasswordSplitted[3];
                    meinUser.Hash = hashedPasswordSplitted[4];
                    int matrikelnummer = Convert.ToInt32(Request["matrikelnummer"]);
                    string studiengang = Request["studiengang"];

                    Boolean worked = Functions.InsertUserIntoDatabase(meinUser);
                    
                    


                    if (worked) {

                        
                      
                        Boolean workedSecondStep = Functions.InsertStudentIntoDatabase(meinUser, matrikelnummer, studiengang);

                        if (workedSecondStep)
                        {
                            return RedirectToAction("RegistrierenSuccess");
                        }
                        else {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult RegistrierenMitarbeiter()
        {

            if (Request["pwd"] as string != null && Request["pwdrepeat"] as string != null)
            {
                string pwd1 = Request["pwd"].ToString();
                string pwd2 = Request["pwdrepeat"].ToString();

                if (pwd1 != pwd2)
                {
                    return View();
                }
                else
                {
                    User meinUser = new User();
                    meinUser.Nutzername = Request["nutzername"];
                    meinUser.Vorname = Request["vorname"];
                    meinUser.Nachname = Request["nachname"];
                    meinUser.Email = Request["email"];
                    meinUser.Geburtsdatum = Convert.ToDateTime(Request["birthdate"]);
                    string hashedPasword = PasswordSecurity.PasswordStorage.CreateHash(Convert.ToString(Request["pwd"]));
                    string[] hashedPasswordSplitted = hashedPasword.Split(':');
                    meinUser.Salt = hashedPasswordSplitted[3];
                    meinUser.Hash = hashedPasswordSplitted[4];
                    string buero = Request["buero"];
                    int telefon = Convert.ToInt32(Request["telefon"]);

                    Boolean worked = Functions.InsertUserIntoDatabase(meinUser);




                    if (worked)
                    {



                        Boolean workedSecondStep = Functions.InsertMitarbeiterIntoDatabase(meinUser, telefon, buero);

                        if (workedSecondStep)
                        {
                            return RedirectToAction("RegistrierenSuccess");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult RegistrierenGast()
        {
            if (Request["pwd"] as string != null && Request["pwdrepeat"] as string != null)
            {
                string pwd1 = Request["pwd"].ToString();
                string pwd2 = Request["pwdrepeat"].ToString();

                if (pwd1 != pwd2)
                {
                    return View();
                }
                else
                {
                    User meinUser = new User();
                    meinUser.Nutzername = Request["nutzername"];
                    meinUser.Vorname = Request["vorname"];
                    meinUser.Nachname = Request["nachname"];
                    meinUser.Email = Request["email"];
                    
                    meinUser.Geburtsdatum = Convert.ToDateTime(Request["birthdate"]); 
                    string hashedPasword = PasswordSecurity.PasswordStorage.CreateHash(Convert.ToString(Request["pwd"]));
                    string[] hashedPasswordSplitted = hashedPasword.Split(':');
                    meinUser.Salt = hashedPasswordSplitted[3];
                    meinUser.Hash = hashedPasswordSplitted[4];
                    string grund = Request["grund"];
                    

                    Boolean worked = Functions.InsertUserIntoDatabase(meinUser);




                    if (worked)
                    {



                        Boolean workedSecondStep = Functions.InsertGastIntoDatabase(meinUser, grund);

                        if (workedSecondStep)
                        {
                            return RedirectToAction("RegistrierenSuccess");
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult RegistrierenSuccess()
        {
            return View();
        }
    }


}