DROP TABLE IF EXISTS MahlzeitHatBilder;
DROP TABLE IF EXISTS BestellungEnthaeltMahlzeiten;
DROP TABLE IF EXISTS BefreundetMit;
DROP TABLE IF EXISTS FH_AngehoerigeGehoertZuFachbereiche;
DROP TABLE IF EXISTS MahlzeitenBrauchenDeklarationen;
DROP TABLE IF EXISTS MahlzeitEnthaeltZutat;
DROP TABLE IF EXISTS Zutaten;
DROP TABLE IF EXISTS Preise;
DROP TABLE IF EXISTS Kommentare;
DROP TABLE IF EXISTS Mahlzeiten;
DROP TABLE IF EXISTS Kategorien;
DROP TABLE IF EXISTS Bilder;
DROP TABLE IF EXISTS Deklarationen;
DROP TABLE IF EXISTS Bestellungen;
DROP TABLE IF EXISTS Fachbereiche;
DROP TABLE IF EXISTS Mitarbeiter;
DROP TABLE IF EXISTS Studenten;
DROP TABLE IF EXISTS FH_Angehoerige;
DROP TABLE IF EXISTS Gaeste;
DROP TABLE IF EXISTS Benutzer;



CREATE TABLE Benutzer(
	Nummer INT AUTO_INCREMENT not null,  -- Nummer soll Primärschlüssel sein
	CONSTRAINT NummerEindeutig UNIQUE(Nummer), -- Die USER-ID soll einzigartig sein
	Nutzername VARCHAR(25) not null, -- Name für den login
	CONSTRAINT Nutzername UNIQUE(Nutzername), -- Nutzername muss eindeutig sein
	Aktiv BOOL not null default 0, -- Aktiv flag
	Anlegedatum TIMESTAMP default CURRENT_TIMESTAMP not null, -- Datum an dem der Account angelegt wurde
	Vorname VARCHAR(28) not null, -- Vorname des Benutzers
	Nachname VARCHAR(40) not null, -- Nachname des Benutzers
	Geburtsdatum DATE, -- Geburtsdatum des Benutzers
	Alters INT AS (TIMESTAMPDIFF(YEAR,Geburtsdatum, NOW())) VIRTUAL, -- Diese Spalte wird automatisch berechnet bei Bedarf
	Email VARCHAR(254) not null, -- Email des Benutzers
	CONSTRAINT Email UNIQUE(Email), -- Email muss eindeutig sein
	Salt VARCHAR(32) not null,
	Hash VARCHAR(24) not null,
	LetzterLogin TIMESTAMP NULL, -- Zeitpunkt zu dem sich ein Benutzer zum letzten mal angemeldet hat
	PRIMARY KEY(Nummer) -- Nummer wird als Primary Key festgelegt
);

CREATE TABLE Gaeste (
	Nummer INT not null, -- Foreign Key aus Benutzer Tabelle
	Constraint NummerEindeutig UNIQUE(Nummer), -- Nummer soll eindeutig sein
	Grund VARCHAR(255), -- Speichert die Begründung für den Gaststatus
	Ablaufdatum DATE default DATE_ADD(NOW(), INTERVAL 7 DAY), -- Ablaufdatum des Gaststatus, Standard = 1 Woche
	CONSTRAINT GastForeignKey FOREIGN KEY(Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE --  Als Foreign Key wird der Primärschlüssel von Benutzer genutzt
	
);

CREATE TABLE FH_Angehoerige (
	Nummer INT not null, -- Foreign Key aus Benutzer Tabelle
	Constraint NummerEindeutig UNIQUE(Nummer), -- Nummer soll eindeutig sein
	CONSTRAINT FH_AngehoerigeForeignKey FOREIGN KEY(Nummer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE --  Als Foreign Key wird der Primärschlüssel von Benutzer genutzt
);

CREATE TABLE Studenten (
	Nummer INT not null, -- Foreign Key aus Benutzer Tabelle
	Constraint NummerEindeutig UNIQUE(Nummer), -- Nummer soll eindeutig sein
	Matrikelnummer INT(9) not null, -- Ganzzahlige Matrikelnummer
	CONSTRAINT UNIQUE(Matrikelnummer), -- Matrikelnummer soll einzigartig sein
	CONSTRAINT Matrikelnummer CHECK(Matrikelnummer > 9999999), CHECK(Matrikelnummer < 100000000),
	Studiengang ENUM ('ET', 'INF', 'ISE', 'MCD', 'WI'), -- Name des Studiengangs des Studierenden
	CONSTRAINT StudentenForeignKey FOREIGN KEY(Nummer) REFERENCES FH_Angehoerige(Nummer) ON DELETE CASCADE --  Als Foreign Key wird der Primärschlüssel von FH_Angehörige genutzt
);

CREATE TABLE Mitarbeiter (
	Nummer INT not null, -- Foreign Key aus Benutzer Tabelle
	Constraint NummerEindeutig UNIQUE(Nummer), -- Nummer soll eindeutig sein
	Buero VARCHAR(4), -- Büro is optional, Typischer Name sieht wie folgt aus: E113
	Telefon INT(15), -- Telefonnummer ist optional und maximal 15 Ziffern lang
	CONSTRAINT Telefon CHECK(Telefon < 1000000000000000),
	CONSTRAINT MitarbeiterForeignKey FOREIGN KEY(Nummer) REFERENCES FH_Angehoerige(Nummer) ON DELETE CASCADE --  Als Foreign Key wird der Primärschlüssel von FH_Angehörige genutzt
);

CREATE TABLE Fachbereiche(
	ID INT AUTO_INCREMENT not null,  -- ID soll Primärschlüssel sein
	Website VARCHAR(253) not null, -- Website ist maximal 253 Character lang (lt. Wikipedia:DNS), ist nicht optional
	Name VARCHAR(40) not null, -- Name des Fachbereichs ist maximal 28 Zeichen lang (Information- und Elektrotechnik)  
	Primary Key (ID) -- ID ist Primary Key
);


CREATE TABLE Bestellungen (
	Nummer INT AUTO_INCREMENT not null,  -- Nummer soll Primärschlüssel sein
	CONSTRAINT NummerEindeutig UNIQUE(Nummer), -- Die Bestellungs-ID soll einzigartig sein
	Bestellzeitpunkt TIMESTAMP DEFAULT CURRENT_TIMESTAMP not null, -- Bestellzeitpunkt ist nicht optional und wird per default auf den aktuellen Zeitpunkt gesetzt
	Abholzeitpunkt TIMESTAMP, -- Abholzeitpunkt ist optional
	CONSTRAINT AbholzeitpunktSpaeterAlsBestellzeitpunkt CHECK(TIMESTAMPDIFF(YEAR,Abholzeitpunkt, Bestellzeitpunkt)>0), CHECK(TIMESTAMPDIFF(Month,Abholzeitpunkt, Bestellzeitpunkt)>0), CHECK(TIMESTAMPDIFF(DAY,Abholzeitpunkt, Bestellzeitpunkt)>0), CHECK(TIMESTAMPDIFF(HOUR,Abholzeitpunkt, Bestellzeitpunkt)>0), CHECK(TIMESTAMPDIFF(Minute,Abholzeitpunkt, Bestellzeitpunkt)>0),CHECK(TIMESTAMPDIFF(SECOND,Abholzeitpunkt, Bestellzeitpunkt)>0),
	Endpreis DOUBLE, -- Endpreis wird aus den in der Bestellung enhaltenen Produkten berechnet. Das ist bei einer N:M Relation in MariaDB nicht möglich
	GetaetigtVon INT DEFAULT NULL,
	PRIMARY KEY (Nummer), -- Nummer wird als Primary Key festgelegt
	CONSTRAINT GetaetigtForeignKey FOREIGN KEY(GetaetigtVon) REFERENCES Benutzer(Nummer) ON DELETE SET NULL --  Als Foreign Key wird der Primärschlüssel von Benutzer genutzt, um eine 1:N Relation abzubilden. Wenn der benutzer gelöscht wird sollen auch seine Bestellungen gelöscht werden
);


CREATE TABLE Deklarationen (
	Zeichen VARCHAR(2) not null, -- Zeichen soll als Primärschlüssel dienen
	Beschriftung VARCHAR(32) not null, --  Die Beschriftung darf maximal 32 Zeichen lang sein und ist nicht optional
	Primary KEY (Zeichen) -- Zeichen ist Primary Key
);


CREATE TABLE Bilder(
	ID INT AUTO_INCREMENT, -- ID soll Primary Key sein
	CONSTRAINT IDEindeutig UNIQUE(ID),
	Alt_Text VARCHAR(100)  not null, -- ist nicht optional und maximal 100 Zeichen lang
	Titel Varchar(100), -- ist optional und maximal 100 Zeichen lang
	Binaerdaten BLOB not null, -- hier wird das Bild gespeichert (JPG, PNG)
	PRIMARY KEY (ID) -- ID ist Primary Key 
);

CREATE TABLE Kategorien(
	ID INT AUTO_INCREMENT not null, -- ID soll Primary Key sein
	CONSTRAINT IDEindeutig UNIQUE(ID),
	Bezeichnung VARCHAR(40) not null, -- Bezeichnung soll ein Text von maximal 40 Zeichen sein. Nicht optional.
	Oberkategorie INT DEFAULT NULL, -- Selbstreferenzieller Foreign Key, default ist 0
	CONSTRAINT hatOberkategorie FOREIGN KEY (Oberkategorie) REFERENCES Kategorien(ID) ON DELETE SET NULL,
	Bild INT DEFAULT NULL,
	CONSTRAINT hatBild FOREIGN KEY(Bild) REFERENCES Bilder(ID) ON DELETE SET NULL, -- Wenn das Bild gelöscht wird soll die Kategorie erhalten bleiben
	PRIMARY KEY (ID) -- ID ist Prymary Key
	
);

CREATE TABLE Mahlzeiten (
	ID INT AUTO_INCREMENT not null,  -- ID soll Primärschlüssel sein
	CONSTRAINT IDEindeutig UNIQUE(ID), -- Die Mahlzeiten-ID soll einzigartig sein
	Beschreibung TEXT not null, 
	Vorrat INT not null,
	Verfügbar BOOL as (Vorrat != 0),
	Kategorie INT DEFAULT NULL,
	CONSTRAINT inKategorie FOREIGN KEY(Kategorie) REFERENCES Kategorien(ID) ON DELETE SET NULL,
	PRIMARY KEY (ID) -- ID wird als Primary Key festgelegt
);

CREATE TABLE Kommentare (
	ID INT AUTO_INCREMENT not null,  -- ID soll Primärschlüssel sein
	CONSTRAINT IDEindeutig UNIQUE(ID), -- Die Kommentar-ID soll einzigartig sein
	Bemerkung TEXT, -- Bemerkung ist optional
	Bewertung INT(1) not null, -- Bewrtung ist nicht optional und ist eine einstellige Zahl
	CONSTRAINT Bewertung CHECK(Bewertung < 6), CHECK(Bewertung > 0), -- Bewertung ist eine Zahl zwischen 1 und 5 (Meilenstein 1)
	Geschrieben_von INT not null, -- ID des Autors als foreign Key 1:N
	zuMahlzeit INT DEFAULT NULL, -- ID der Mahlzeit 1:N
	PRIMARY KEY (ID), -- ID wird als Primary Key festgelegt
	CONSTRAINT GeschriebenForeignKey FOREIGN KEY(Geschrieben_von) REFERENCES Studenten(Nummer)  ON DELETE CASCADE, --  Als Foreign Key wird der Primärschlüssel von Studenten genutzt, um eine 1:N Relation abzubilden. Wenn der Student gelöscht wird sollen auch seine Kommentare gelöscht werden
	CONSTRAINT zuMahlzeitenForeignKey FOREIGN KEY(zuMahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE SET NULL
);

CREATE TABLE Preise(
	Mahlzeit INT not null, -- Soll Foreign Key sein
	Jahr YEAR not null default YEAR(NOW()), -- Das Jahr in dem der Preis gültig sein soll
	CONSTRAINT Jahr CHECK(Jahr > 999), CHECK(Jahr < 10000), -- Jahr muss einen Wert zwischen 1000 und 9999 haben
	Gastpreis DECIMAL(4,2) not null,  -- Ist eine Kommazahl mit Zwei Stellen vor und zwei Stellen hinter dem Komma. Ist nicht optional
	Studentpreis DECIMAL(4,2), -- ist eine Kommazahl wie Gastpreis, optional
	MAPreis DECIMAL(4,2), --  Ist eine Kommazahl wie Gastpreis, optional
	CONSTRAINT StudentenPreisGuenstigerAlsMA_Preis CHECK(Studentpreis < MAPreis),
	CONSTRAINT MahlzeitKostetPreis FOREIGN KEY(Mahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE CASCADE ON UPDATE CASCADE -- Da Preis eine schwache Entität ist verfügt sie nur über einen Fremdschlüssel und wird gelöscht, sobald dieser gelöscht wird
		
);

CREATE TABLE Zutaten(
	ID INT(5) not null, -- ID soll PrimärSchlüssel sein
	CONSTRAINT IDEindeutig UNIQUE(ID), -- ID ist eindeutig
	CONSTRAINT IDFuenfstellig CHECK(ID > 9999), CHECK(ID <100000), -- ID ist eine fuenfstellige Zahl
	Name VARCHAR(100), -- Zutaten werden ein Text von maximal 100 Zeichen sein
	Bio BOOL not null, -- Flag um die Zutat als BIO zu kennzeichnen
	Vegetarisch BOOL not null, -- Flag um die Zutat als Vegetarisch zu kennzeichnen
	Vegan BOOL not null, -- Flag um die Zutat als Vegan zu kennzeichnen
	Glutenfrei BOOL not null, -- Flag um die Zutat als Glutenfrei zu kennzeichnen
	PRIMARY KEY (ID) -- ID ist PrimaryKey
);


CREATE TABLE MahlzeitEnthaeltZutat(
	Mahlzeit INT not null, 
	Zutat INT not null, 
	CONSTRAINT FOREIGN KEY(Mahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE CASCADE, 
	CONSTRAINT FOREIGN KEY(Zutat) REFERENCES Zutaten(ID) ON DELETE CASCADE
);


CREATE TABLE MahlzeitenBrauchenDeklarationen (
	Mahlzeit INT not null, 
	Deklaration VARCHAR(2) not null, 
	CONSTRAINT MahlzeitBrauchtDeklaration FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE CASCADE,
	CONSTRAINT DeklarationBrauchtMahlzeit FOREIGN KEY (Deklaration) REFERENCES Deklarationen(Zeichen) ON DELETE CASCADE
);

CREATE TABLE FH_AngehoerigeGehoertZuFachbereiche(
	FH_Angehoeriger INT not null, 
	Fachbereich INT not null,
	CONSTRAINT FH_AngehoerigerGehoertZuFachbereich FOREIGN KEY (FH_Angehoeriger) REFERENCES Mahlzeiten(ID),
	CONSTRAINT FachbereichGehoertZuFH_Angehoerigem FOREIGN KEY (Fachbereich) REFERENCES FH_Angehoerige(Nummer)
);

CREATE TABLE BefreundetMit(
	Benutzer INT not null, 
	Freund INT not null, 
	CONSTRAINT Benutzer FOREIGN KEY(Benutzer) REFERENCES Benutzer(Nummer) ON DELETE CASCADE,
	CONSTRAINT Freund FOREIGN KEY(Freund) REFERENCES Benutzer(Nummer) ON DELETE CASCADE -- Beide Seiten der Relation müssen ausgefüllt sein, es gibt keine unique Beschränkung
);

CREATE TABLE BestellungEnthaeltMahlzeiten (
	Bestellung INT not null, 
	Mahlzeit INT DEFAULT NULL, 
	Anzahl INT not null,
	CONSTRAINT Bestellung FOREIGN KEY (Bestellung) REFERENCES Bestellungen(Nummer) ON DELETE CASCADE, -- wenn eine Bestellung gelöscht wird soll auch die N:M Relation gelöscht werden
	CONSTRAINT Mahlzeit FOREIGN KEY (Mahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE SET NULL -- wenn eine Mahlzeit gelöscht wird bleibt die Relation erhalten sie verweist dann lediglich auf NULL
	
);

CREATE TABLE MahlzeitHatBilder (
	Mahlzeit INT not null, 
	Bild INT not null, 
	CONSTRAINT FOREIGN KEY(Mahlzeit) REFERENCES Mahlzeiten(ID) ON DELETE CASCADE,
	CONSTRAINT FOREIGN KEY (Bild) REFERENCES Bilder(ID) ON DELETE CASCADE
);




INSERT INTO Benutzer (Nutzername, Vorname, Nachname, Email, Geburtsdatum, Salt, Hash) VALUES
    ("BJOE", "Banana", "Joe", "Banana@joe.net", "1950-01-30", "3", "4"),
    ("HP", "Harry", "Potter", "Harry@potter.net", "1983-10-15", "3", "4"),
    ("Poet_in_Exile","Oscar",  "Wilde", "Oscar@Wilde.net", "1995-04-21", "3", "4"),
    ("Test_User","Test", "Testinson", "Test@user.net", "1997-10-30", "3", "4"),
    ("Timelord","Doctor", "Who", "Doctor@who.net", "1991-01-30", "3", "4"),
    ("Detectiv","Dick", "Gently","Dick@gently.net", "1970-10-07", "3", "4");
    
   
INSERT INTO Gaeste (Nummer, Grund) VALUES (1, "Pilot zu Besuch.");

INSERT INTO FH_Angehoerige (Nummer) VALUES (2), (3), (4);  


INSERT INTO Studenten (Nummer, Matrikelnummer) VALUES (2, 31556789), (3, 31556790);

INSERT INTO Mitarbeiter (Nummer, Telefon) VALUES (4, 23);

INSERT INTO Mahlzeiten (ID, Beschreibung, Vorrat) VALUES (1, "Suppe", 100);

INSERT INTO Fachbereiche (ID, Name, Website) VALUES (1, "Elektrotechnik- und Informationstechnik", "fh-aachen.de");

INSERT INTO Kommentare (Geschrieben_von, ID, Bewertung, zuMahlzeit) VALUES
	(2, 1, 5, 1),
	(2, 2, 3, 1);
	
INSERT INTO Deklarationen (Zeichen, Beschriftung) VALUES ("E2", "Enthält Dioxin"), ("E3", "Scharf");
INSERT INTO MahlzeitenBrauchenDeklarationen (Mahlzeit, Deklaration) VALUES (1, "E2"), (1, "E3");

INSERT INTO Bestellungen(GetaetigtVon) VALUES (2);
INSERT INTO BestellungEnthaeltMahlzeiten(Bestellung, Mahlzeit, Anzahl) VALUES (1,1,5);

INSERT INTO Preise(Mahlzeit, Gastpreis) VALUES (1,99.99);


 -- DELETE FROM Mahlzeiten WHERE ID=1;	

 DELETE FROM Benutzer WHERE Nummer=2;

-- INSERT INTO Benutzer (Nutzername, Vorname, Nachname, Email, Geburtsdatum, Salt, Hash) VALUES
--    ("BJOE", "Banana", "Joe", "Banana@joe.net", "1950-01-30", "3", "4");    
    
SELECT Nummer, Nutzername,Vorname, Nachname, Aktiv, Anlegedatum, Geburtsdatum, Alters, LetzterLogin FROM Benutzer

