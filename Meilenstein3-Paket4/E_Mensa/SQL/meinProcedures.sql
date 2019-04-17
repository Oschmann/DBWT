-- --------------------------------------------------------
-- Host:                         149.201.48.84
-- Server Version:               10.3.10-MariaDB-1:10.3.10+maria~bionic-log - mariadb.org binary distribution
-- Server Betriebssystem:        debian-linux-gnu
-- HeidiSQL Version:             9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Exportiere Datenbank Struktur für db3155568
CREATE DATABASE IF NOT EXISTS `db3155568` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;
USE `db3155568`;

-- Exportiere Struktur von Prozedur db3155568.getMyPrice
DELIMITER //
CREATE DEFINER=`oliver.oschmann`@`149.201.%` PROCEDURE `getMyPrice`(
	IN `InputBenutzer` INT,
	IN `InputMahlzeit` INT


,
	OUT `Price` INT






)
    READS SQL DATA
BEGIN

SELECT Nutzername INTO @userName FROM Benutzer WHERE Nummer=InputBenutzer;
CALL user_role(@userName, @result);
CASE @result
	WHEN "Student" THEN SELECT Studentpreis FROM Preise as p WHERE p.Mahlzeit = InputMahlzeit;
	WHEN "Mitarbeiter" THEN SELECT MAPreis FROM Preise as p WHERE p.Mahlzeit = InputMahlzeit;
	WHEN "Gast" THEN SELECT Gastpreis FROM Preise as p WHERE p.Mahlzeit = InputMahlzeit;
	ELSE BEGIN END;
END CASE;


END//
DELIMITER ;

-- Exportiere Struktur von Prozedur db3155568.user_role
DELIMITER //
CREATE DEFINER=`oliver.oschmann`@`149.201.%` PROCEDURE `user_role`(
	IN `username` VARCHAR(25)


,
	OUT `role` VARCHAR(15)

)
    READS SQL DATA
    DETERMINISTIC
BEGIN




IF (SELECT COUNT(*) FROM Studenten AS s JOIN FH_Angehoerige AS a ON s.Nummer = a.Nummer JOIN Benutzer AS b ON b.Nummer = a.Nummer WHERE b.Nutzername= username) = 1 THEN SELECT 'Student' into role;
ELSEIF (SELECT COUNT(*) FROM Mitarbeiter AS m JOIN FH_Angehoerige AS a ON m.Nummer = a.Nummer JOIN Benutzer AS b ON b.Nummer = a.Nummer WHERE b.Nutzername= username) = 1 THEN SELECT 'Mitarbeiter' into role;
ELSE SELECT 'Gast' INTO role;
END IF;





END//
DELIMITER ;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
