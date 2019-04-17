using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DBWTPaket5.Models
{
    public class Functions
    {
        private static readonly MySqlConnection con;

        static Functions()
        {
            con = new MySqlConnection(ConfigurationManager.ConnectionStrings["dbConStr"].ConnectionString);
            con.Open();
        }


        public static List<Produkte> GetProdukte (bool verfuegbar, bool vegetarisch, bool vegan, int katID)
        {
            List<Produkte> meineProdukte = new List<Produkte>();
            MySqlCommand cmd = con.CreateCommand();

            try
            {
                cmd.CommandText = @"SELECT m.ID, m.Name, Beschreibung, Verfügbar FROM Mahlzeiten m JOIN Kategorien mk ON mk.ID=m.Kategorie JOIN MahlzeitEnthaeltZutat mz on mz.Mahlzeit = m.ID JOIN Zutaten mzz on mzz.ID = mz.Zutat ";

                if(verfuegbar || vegetarisch || vegan || katID > 0)
                {
                    cmd.CommandText += "WHERE ";
                }
                if (verfuegbar)
                {
                    cmd.CommandText += "Verfügbar=1 ";
                }
                if (vegetarisch)
                {
                    if (verfuegbar)
                    {
                        cmd.CommandText += "AND ";
                    }
                    cmd.CommandText += "Vegetarisch=1 ";
                }
                if (vegan)
                {
                    if (verfuegbar || vegetarisch)
                    {
                        cmd.CommandText += "AND ";
                    }
                    cmd.CommandText += "Vegan=1 ";
                }
                if (katID>0)
                {
                    if (verfuegbar || vegetarisch || vegan)
                    {
                        cmd.CommandText += "AND ";
                    }
                    cmd.CommandText += "m.Kategorie=" + katID;
                }
              


                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Produkte meinProdukt = new Produkte();
                        meinProdukt.ID = Convert.ToInt32(r["ID"]);
                        meinProdukt.Name = r["Name"].ToString();
                        meinProdukt.Beschreibung = r["Beschreibung"].ToString();
                        meinProdukt.Verfuegbar = Convert.ToBoolean(r["Verfügbar"]);
                        
                        meineProdukte.Add(meinProdukt);

                    }
                }
            }
            catch
            {

            }
            
            return meineProdukte;
        }

        public static Produkte GetProdukteByIDForDetail(int id)
        {
            Produkte meinProdukt = new Produkte();
            MySqlCommand cmd = con.CreateCommand();

            try
            {
                cmd.CommandText = @"SELECT m.ID, Name, Beschreibung, Gastpreis, Studentpreis, MAPreis FROM Mahlzeiten AS m JOIN Preise AS p ON p.Mahlzeit = m.ID LEFT JOIN MahlzeitHatBilder AS mb ON mb.Mahlzeit = m.ID LEFT JOIN Bilder AS b ON b.ID = mb.Bild WHERE m.ID=@searchedID LIMIT 1";
                cmd.Parameters.AddWithValue("@searchedID", id);




                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        
                        meinProdukt.ID = Convert.ToInt32(r["ID"]);
                        meinProdukt.Name = r["Name"].ToString();
                        meinProdukt.Beschreibung = r["Beschreibung"].ToString();
                        meinProdukt.Gastpreis = Convert.ToDecimal(r["Gastpreis"]);
                        meinProdukt.Studentpreis = Convert.ToDecimal(r["Studentpreis"]);
                        meinProdukt.MAPreis = Convert.ToDecimal(r["MAPreis"]);



                    }
                }
            }
            catch
            {

            }

            return meinProdukt;
        }

        public static List<Kategorien> GetKategorien()
        {
            List<Kategorien> meineKategorien = new List<Kategorien>();
            MySqlCommand cmd = con.CreateCommand();

            try
            {
                cmd.CommandText = @"SELECT ID, Bezeichnung, Oberkategorie FROM Kategorien";

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Kategorien neueKategorie = new Kategorien();
                        neueKategorie.ID = Convert.ToInt32(r["ID"]);
                        neueKategorie.Bezeichnung = r["Bezeichnung"].ToString();
                        if (r["Oberkategorie"] != DBNull.Value)
                        {
                            neueKategorie.Oberkategorie = Convert.ToInt32(r["Oberkategorie"]);
                        }
                        meineKategorien.Add(neueKategorie);
                    }
                }
            }
            catch { }
            return meineKategorien;
        }

        public static Kategorien GetKategorieByID(int id)
        {
            Kategorien meineKategorie = new Kategorien();
            MySqlCommand cmd = con.CreateCommand();
            if (id == 0) {
                meineKategorie.ID = 0;
                meineKategorie.Bezeichnung = "Bestseller";
            }
            else
            {
                try
                {
                    cmd.CommandText = @"SELECT ID, Bezeichnung FROM Kategorien WHERE ID=@searchedID";
                    cmd.Parameters.AddWithValue("@searchedID", id);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            meineKategorie.ID = Convert.ToInt32(r["ID"]);
                            meineKategorie.Bezeichnung = r["Bezeichnung"].ToString();
                        }
                    }
                }
                catch { }
            }
            return meineKategorie;
        }

        public static Bild GetBild(int id)
        {
            Bild meinBild = new Bild();
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"SELECT Alt_Text, Titel, Binaerdaten FROM Mahlzeiten AS m LEFT JOIN MahlzeitHatBilder AS mb ON mb.Mahlzeit = m.ID LEFT JOIN Bilder AS b ON b.ID = mb.Bild WHERE m.ID=@searchedID LIMIT 1";
                cmd.Parameters.AddWithValue("@searchedID", id);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        meinBild.Alt_Text = r["Alt_Text"].ToString();
                        meinBild.Titel = r["Titel"].ToString();
                        meinBild.Binaerdaten = Convert.ToBase64String((byte[])r["Binaerdaten"]);

                    }
                }
            }
            catch { }
            return meinBild;
        }

        public static List<Zutat> GetZutatenListe() {
            List<Zutat> meineZutatenListe = new List<Zutat>();
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"SELECT Name, Bio, Vegan, Vegetarisch, Glutenfrei FROM Zutaten ORDER BY Bio Desc, Name ASC";
                

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Zutat meineZutat = new Zutat();
                        meineZutat.Name = Convert.ToString(r["Name"]);
                        meineZutat.Bio = Convert.ToBoolean(r["Bio"]);
                        meineZutat.Vegan = Convert.ToBoolean(r["Vegan"]);
                        meineZutat.Vegetarisch = Convert.ToBoolean(r["Vegetarisch"]);
                        meineZutat.Glutenfrei = Convert.ToBoolean(r["Glutenfrei"]);
                        meineZutatenListe.Add(meineZutat);

                    }
                }
            }
            catch { }
            return meineZutatenListe;
        }

        public static User GetUserByNutzerName(string searchedNutzername)
        {
            User meinUser = new User();
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"SELECT Nummer,Salt, Hash FROM Benutzer WHERE Nutzername=@searchedNutzername";
                cmd.Parameters.AddWithValue("@searchedNutzername", searchedNutzername);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        meinUser.Nutzername = searchedNutzername;
                        meinUser.Nummer = Convert.ToInt32(r["Nummer"]);
                        meinUser.Salt = r["Salt"].ToString();
                        meinUser.Hash = r["Hash"].ToString();


                    }
                }
            }
            catch { }
           
            return meinUser;
        }

        public static string GetRoleByNutzerName(string searchedNutzername)
        {
            
            MySqlCommand cmd = con.CreateCommand();
            string foundRole = "";
            try
            {
                
                cmd.CommandText = @"user_role";
                cmd.Parameters.AddWithValue("@searchedNutzername", searchedNutzername);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", searchedNutzername);
                cmd.Parameters.Add("@role", MySqlDbType.VarChar, 15);
                cmd.Parameters["@username"].Direction = System.Data.ParameterDirection.Input;
                cmd.Parameters["@role"].Direction = System.Data.ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                foundRole = (string)cmd.Parameters["@role"].Value;
                
            }
            catch { }
            return foundRole;
        }

        public static Boolean InsertUserIntoDatabase(User meinUser)
        {
            MySqlTransaction tr = con.BeginTransaction();
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"INSERT INTO Benutzer (Nutzername, Vorname, Nachname, Email, Geburtsdatum, Salt, Hash, Aktiv) VALUES (@username, @firstName, @lastName, @userMail, @birthDate, @newSalt, @newHash, 0)";
                cmd.Parameters.AddWithValue("@username", meinUser.Nutzername);
                cmd.Parameters.AddWithValue("@firstName", meinUser.Vorname);
                cmd.Parameters.AddWithValue("@lastName", meinUser.Nachname);
                cmd.Parameters.AddWithValue("@userMail", meinUser.Email);
                cmd.Parameters.AddWithValue("@birthDate", meinUser.Geburtsdatum);
                cmd.Parameters.AddWithValue("@newSalt", meinUser.Salt);
                cmd.Parameters.AddWithValue("@newHash", meinUser.Hash);
                int worked = cmd.ExecuteNonQuery();
                if (worked > 0)
                {
                    tr.Commit();
                    return true;
                }
                else
                {
                    tr.Rollback();
                    return false; 
                }
            }
            catch {
                tr.Rollback();
                return false;  }

        }
        public static Boolean InsertStudentIntoDatabase(User meinUser, int matrikelnummer, string studiengang)
        {
            User meinNeuerUser = GetUserByNutzerName(meinUser.Nutzername);
            Boolean insertIntoAngehoerige = InsertAngehoerigeIntoDatabase(meinNeuerUser);
            if (insertIntoAngehoerige)
            {
                MySqlTransaction tr = con.BeginTransaction();
                MySqlCommand cmd = con.CreateCommand();

                try
                {

                    cmd.CommandText = @"INSERT INTO Studenten (Nummer, Matrikelnummer, Studiengang) VALUES (@userID, @matrikelNummer, @courseStudy)";
                    cmd.Parameters.AddWithValue("@userID", meinNeuerUser.Nummer);
                    cmd.Parameters.AddWithValue("@matrikelNummer", matrikelnummer);
                    cmd.Parameters.AddWithValue("@courseStudy", studiengang);

                    int worked = cmd.ExecuteNonQuery();
                    if (worked > 0)
                    {
                        tr.Commit();
                        return true;
                    }
                    else
                    {
                        tr.Rollback();
                        return false;
                    }
                }
                catch
                {
                    tr.Rollback();
                    return false;
                }
            }
            else
            {
                return false; 
            }

        }

        public static Boolean InsertMitarbeiterIntoDatabase(User meinUser, int telefon, string buero)
        {
            User meinNeuerUser = GetUserByNutzerName(meinUser.Nutzername);
            Boolean insertIntoAngehoerige = InsertAngehoerigeIntoDatabase(meinNeuerUser);
            if (insertIntoAngehoerige)
            {
                MySqlTransaction tr = con.BeginTransaction();
                MySqlCommand cmd = con.CreateCommand();

                try
                {

                    cmd.CommandText = @"INSERT INTO Mitarbeiter (Nummer, Buero, Telefon) VALUES (@userID, @office, @phone)";
                    cmd.Parameters.AddWithValue("@userID", meinNeuerUser.Nummer);
                    cmd.Parameters.AddWithValue("@office", buero);
                    cmd.Parameters.AddWithValue("@phone", telefon);

                    int worked = cmd.ExecuteNonQuery();
                    if (worked > 0)
                    {
                        tr.Commit();
                        return true;
                    }
                    else
                    {
                        tr.Rollback();
                        return false;
                    }
                }
                catch
                {
                    tr.Rollback();
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public static Boolean InsertGastIntoDatabase(User meinUser, string grund)
        {
            User meinNeuerUser = GetUserByNutzerName(meinUser.Nutzername);
            DateTime ablaufDatum = DateTime.Now.AddYears(1);
          
                MySqlTransaction tr = con.BeginTransaction();
                MySqlCommand cmd = con.CreateCommand();

                try
                {

                    cmd.CommandText = @"INSERT INTO Gaeste (Nummer, Grund, Ablaufdatum) VALUES (@userID, @reason, @due)";
                    cmd.Parameters.AddWithValue("@userID", meinNeuerUser.Nummer);
                    cmd.Parameters.AddWithValue("@reason", grund);
                cmd.Parameters.AddWithValue("@due", ablaufDatum);


                int worked = cmd.ExecuteNonQuery();
                    if (worked > 0)
                    {
                        tr.Commit();
                        return true;
                    }
                    else
                    {
                        tr.Rollback();
                        return false;
                    }
                }
                catch
                {
                    tr.Rollback();
                    return false;
                }
           

        }

        public static Boolean InsertAngehoerigeIntoDatabase(User meinUser)
        {
            

            MySqlTransaction tr = con.BeginTransaction();
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"INSERT INTO FH_Angehoerige (Nummer) VALUES (@userID)";
                cmd.Parameters.AddWithValue("@userID", meinUser.Nummer);
                

                int worked = cmd.ExecuteNonQuery();
                if (worked > 0)
                {
                    tr.Commit();
                    return true;
                }
                else
                {
                    tr.Rollback();
                    return false;
                }
            }
            catch
            {
                tr.Rollback();
                return false;
            }

        }

        public static Boolean IsActive(int searchedNutzer)
        {
            Boolean active = false;
            MySqlCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = @"SELECT Aktiv FROM Benutzer WHERE Nummer=@searchedNutzer";
                cmd.Parameters.AddWithValue("@searchedNutzer", searchedNutzer);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        active = Convert.ToBoolean(r["Aktiv"]);


                    }
                }
            }
            catch { }

            return active;
        }
    }
}