-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: May 07, 2022 at 11:41 AM
-- Server version: 10.4.10-MariaDB
-- PHP Version: 7.4.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `mediatek86`
--


-- --------------------------------------------------------

--
-- Table structure for table `abonnement`
--

DROP TABLE IF EXISTS `abonnement`;
CREATE TABLE IF NOT EXISTS `abonnement` (
  `id` varchar(5) NOT NULL,
  `dateFinAbonnement` date DEFAULT NULL,
  `idRevue` varchar(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idRevue` (`idRevue`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `abonnement`
--

INSERT INTO `abonnement` (`id`, `dateFinAbonnement`, `idRevue`) VALUES
('00014', '2022-04-07', '10001'),
('00015', '2022-02-15', '10001'),
('00016', '2022-04-07', '10005'),
('00017', '2022-04-29', '10004'),
('00018', '2022-04-19', '10007');

--
-- Triggers `abonnement`
--
DROP TRIGGER IF EXISTS `insabonnement`;
DELIMITER $$
CREATE TRIGGER `insabonnement` BEFORE INSERT ON `abonnement` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM commande where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "une commande doit exister pour la création de l'abonnement";
    END IF ; 
    IF EXISTS(SELECT * FROM commandedocument where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "une commandedocument existe déjà, insertion impossible de l'abonnement";
    END IF ;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
CREATE TABLE IF NOT EXISTS `commande` (
  `id` varchar(5) NOT NULL,
  `dateCommande` date DEFAULT NULL,
  `montant` double DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `commande`
--

INSERT INTO `commande` (`id`, `dateCommande`, `montant`) VALUES
('00001', '2022-02-12', 50),
('00003', '2022-02-05', 3),
('00004', '2022-02-05', 130),
('00005', '2022-02-07', 674.44),
('00007', '2022-02-07', 49),
('00008', '2022-02-14', 5),
('00009', '2022-02-14', 20),
('00010', '2022-02-14', 20),
('00013', '2022-02-14', 74.99),
('00014', '2022-02-18', 20),
('00015', '2022-02-10', 10),
('00016', '2022-04-01', 1),
('00017', '2022-04-01', 3),
('00018', '2022-04-01', 5);

-- --------------------------------------------------------

--
-- Table structure for table `commandedocument`
--

DROP TABLE IF EXISTS `commandedocument`;
CREATE TABLE IF NOT EXISTS `commandedocument` (
  `id` varchar(5) NOT NULL,
  `nbExemplaire` int(11) DEFAULT NULL,
  `idLivreDvd` varchar(10) NOT NULL,
  `idSuivi` char(5) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idLivreDvd` (`idLivreDvd`),
  KEY `idSuivi` (`idSuivi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `commandedocument`
--

INSERT INTO `commandedocument` (`id`, `nbExemplaire`, `idLivreDvd`, `idSuivi`) VALUES
('00001', 6, '00001', '00003'),
('00003', 2, '20001', '00002'),
('00004', 8, '20004', '00003'),
('00005', 50, '20002', '00003'),
('00007', 5, '20002', '00002'),
('00008', 1, '20003', '00002'),
('00009', 5, '00017', '00003'),
('00010', 5, '00017', '00002'),
('00013', 15, '00016', '00001');

--
-- Triggers `commandedocument`
--
DROP TRIGGER IF EXISTS `inscommandedocument`;
DELIMITER $$
CREATE TRIGGER `inscommandedocument` BEFORE INSERT ON `commandedocument` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM commande where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "une commande doit exister pour la création de la commandedocument";
    END IF ; 
    IF EXISTS(SELECT * FROM abonnement where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un abonnement existe déjà, insertion impossible de la commandedocument";
    END IF ;
END
$$
DELIMITER ;
DROP TRIGGER IF EXISTS `updateCommande_document`;
DELIMITER $$
CREATE TRIGGER `updateCommande_document` AFTER UPDATE ON `commandedocument` FOR EACH ROW BEGIN
    DECLARE v_nbExemplaire INT;
	DECLARE v_nb INT default 0;
	DECLARE v_date DATE;
	DECLARE v_numero INT;
	DECLARE v_number INT;
	SELECT nbExemplaire INTO v_nbExemplaire	FROM commandedocument where id = NEW.id;
    SELECT dateCommande INTO v_date FROM commande where id = NEW.id;
    SELECT COUNT(*) INTO v_number FROM exemplaire where id = NEW.idLivreDvd;
    IF (v_number > 0) THEN
	    SELECT MAX(numero) into v_numero FROM exemplaire where id = NEW.idLivreDvd;
		SET v_numero = v_numero + 1;
    ELSE
		SET v_numero = 0;
    END IF;
    IF NEW.idSuivi = "00002" THEN
	    WHILE (v_nb < v_nbExemplaire) DO
	        INSERT INTO exemplaire (id, numero, dateAchat, photo, idEtat)
		    VALUES (NEW.idLivreDvd, v_numero, v_date, "", "00001");
		    SET v_nb = v_nb + 1;
		    SET v_numero = v_numero + 1;
	    END WHILE ;
	END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `document`
--

DROP TABLE IF EXISTS `document`;
CREATE TABLE IF NOT EXISTS `document` (
  `id` varchar(10) NOT NULL,
  `titre` varchar(60) DEFAULT NULL,
  `image` varchar(100) DEFAULT NULL,
  `idRayon` varchar(5) NOT NULL,
  `idPublic` varchar(5) NOT NULL,
  `idGenre` varchar(5) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idRayon` (`idRayon`),
  KEY `idPublic` (`idPublic`),
  KEY `idGenre` (`idGenre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `document`
--

INSERT INTO `document` (`id`, `titre`, `image`, `idRayon`, `idPublic`, `idGenre`) VALUES
('00001', 'Quand sort la recluse', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\bmw-m4-gc4c473057_640.jpg', 'LV003', '00002', '10014'),
('00002', 'Un pays à l\'aube', '', 'LV001', '00002', '10004'),
('00003', 'Et je danse aussi', '', 'LV002', '00003', '10013'),
('00004', 'L\'armée furieuse', '', 'LV003', '00002', '10014'),
('00005', 'Les anonymes', '', 'LV001', '00002', '10014'),
('00006', 'La marque jaune', '', 'BD001', '00003', '10001'),
('00007', 'Dans les coulisses du musée', '', 'LV001', '00003', '10006'),
('00008', 'Histoire du juif errant', '', 'LV002', '00002', '10006'),
('00009', 'Pars vite et reviens tard', '', 'LV003', '00002', '10014'),
('00010', 'Le vestibule des causes perdues', '', 'LV001', '00002', '10006'),
('00011', 'L\'île des oubliés', '', 'LV002', '00003', '10006'),
('00012', 'La souris bleue', '', 'LV002', '00003', '10006'),
('00013', 'Sacré Pêre Noël', '', 'JN001', '00001', '10001'),
('00014', 'Mauvaise étoile', '', 'LV003', '00003', '10014'),
('00015', 'La confrérie des téméraires', '', 'JN002', '00004', '10014'),
('00016', 'Le butin du requin', '', 'JN002', '00004', '10014'),
('00017', 'Catastrophes au Brésil', '', 'JN002', '00004', '10014'),
('00018', 'Le Routard - Maroc', '', 'DV005', '00003', '10011'),
('00019', 'Guide Vert - Iles Canaries', '', 'DV005', '00003', '10011'),
('00020', 'Guide Vert - Irlande', '', 'DV005', '00003', '10011'),
('00021', 'Les déferlantes', '', 'LV002', '00002', '10006'),
('00022', 'Une part de Ciel', '', 'LV002', '00002', '10006'),
('00023', 'Le secret du janissaire', '', 'BD001', '00002', '10001'),
('00024', 'Pavillon noir', '', 'BD001', '00002', '10001'),
('00025', 'L\'archipel du danger', '', 'BD001', '00002', '10001'),
('00026', 'La planète des singes', '', 'LV002', '00003', '10002'),
('00027', 'Catastrophes au Brésil', '', 'JN002', '00004', '10014'),
('10001', 'Arts Magazine', '', 'PR002', '00002', '10016'),
('10002', 'Alternatives Economiques', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\sailing-gc7b6df597_640.jpg', 'PR002', '00002', '10015'),
('10003', 'Challenges', '', 'PR002', '00002', '10015'),
('10004', 'Rock and Folk', '', 'PR002', '00002', '10016'),
('10005', 'Les Echos', '', 'PR001', '00002', '10015'),
('10006', 'Le Monde', '', 'PR001', '00002', '10018'),
('10007', 'Telerama', '', 'PR002', '00002', '10016'),
('10008', 'L\'Obs', '', 'PR002', '00002', '10018'),
('10009', 'L\'Equipe', '', 'PR001', '00002', '10017'),
('10010', 'L\'Equipe Magazine', '', 'PR002', '00002', '10017'),
('10011', 'Geo', '', 'PR002', '00003', '10016'),
('20001', 'Star Wars 5 L\'empire contre attaque', '', 'DF001', '00003', '10002'),
('20002', 'Le seigneur des anneaux : la communauté de l\'anneau', '', 'DF001', '00003', '10019'),
('20003', 'Jurassic Park', '', 'DF001', '00003', '10002'),
('20004', 'Matrix', '', 'DF001', '00003', '10002');

-- --------------------------------------------------------

--
-- Table structure for table `dvd`
--

DROP TABLE IF EXISTS `dvd`;
CREATE TABLE IF NOT EXISTS `dvd` (
  `id` varchar(10) NOT NULL,
  `synopsis` text DEFAULT NULL,
  `realisateur` varchar(20) DEFAULT NULL,
  `duree` int(6) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `dvd`
--

INSERT INTO `dvd` (`id`, `synopsis`, `realisateur`, `duree`) VALUES
('20001', 'Luc est entraîné par Yoda pendant que Han et Leia tentent de se cacher dans la cité des nuages.', 'George Lucas', 124),
('20002', 'L\'anneau unique, forgé par Sauron, est porté par Fraudon qui l\'amène à Foncombe. De là, des représentants de peuples différents vont s\'unir pour aider Fraudon à amener l\'anneau à la montagne du Destin.', 'Peter Jackson', 228),
('20003', 'Un milliardaire et des généticiens créent des dinosaures à partir de clonage.', 'Steven Spielberg', 128),
('20004', 'Un informaticien réalise que le monde dans lequel il vit est une simulation gérée par des machines.', 'Les Wachowski', 136);

--
-- Triggers `dvd`
--
DROP TRIGGER IF EXISTS `insdvd`;
DELIMITER $$
CREATE TRIGGER `insdvd` BEFORE INSERT ON `dvd` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM livres_dvd where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un livresdvd doit exister pour la création du dvd";
    END IF ; 
    IF EXISTS(SELECT * FROM livre where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un livre existe déjà, insertion impossible du dvd";
    END IF ;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `etat`
--

DROP TABLE IF EXISTS `etat`;
CREATE TABLE IF NOT EXISTS `etat` (
  `id` char(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `etat`
--

INSERT INTO `etat` (`id`, `libelle`) VALUES
('00001', 'neuf'),
('00002', 'usagé'),
('00003', 'détérioré'),
('00004', 'inutilisable');

-- --------------------------------------------------------

--
-- Table structure for table `exemplaire`
--

DROP TABLE IF EXISTS `exemplaire`;
CREATE TABLE IF NOT EXISTS `exemplaire` (
  `id` varchar(10) NOT NULL,
  `numero` int(11) NOT NULL,
  `dateAchat` date DEFAULT NULL,
  `photo` varchar(100) NOT NULL,
  `idEtat` char(5) NOT NULL,
  PRIMARY KEY (`id`,`numero`),
  KEY `idEtat` (`idEtat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `exemplaire`
--

INSERT INTO `exemplaire` (`id`, `numero`, `dateAchat`, `photo`, `idEtat`) VALUES
('00003', 56, '2022-04-01', '', '00001'),
('10001', 50, '2022-02-19', '', '00001'),
('10002', 418, '2021-12-01', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\astronaut-g4e2820e0d_640.jpg', '00002'),
('10004', 79, '2022-04-03', '', '00001'),
('10005', 1, '2022-04-01', '', '00001'),
('10007', 3237, '2021-11-23', '', '00001'),
('10007', 3238, '2021-11-30', '', '00001'),
('10007', 3239, '2021-12-07', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\entrepreneur.jpg', '00001'),
('10007', 3240, '2021-12-21', '', '00001'),
('10011', 506, '2021-04-01', '', '00001'),
('10011', 507, '2021-05-03', '', '00001'),
('10011', 508, '2021-06-05', '', '00001'),
('10011', 509, '2021-07-01', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\boy-ga6f69d470_640.png', '00001'),
('10011', 510, '2021-08-04', '', '00001'),
('10011', 511, '2021-09-01', '', '00001'),
('10011', 512, '2021-10-06', 'D:\\Maison\\Cned\\BTS2_2\\Ateliers de professionnalisation\\Atelier 3\\imgs\\sailing-gc7b6df597_640.jpg', '00001'),
('10011', 513, '2021-11-01', '', '00001'),
('10011', 514, '2021-12-01', '', '00001'),
('20002', 0, '2022-02-07', '', '00001'),
('20002', 1, '2022-02-07', '', '00002'),
('20002', 3, '2022-02-07', '', '00004'),
('20002', 4, '2022-02-07', '', '00001');

-- --------------------------------------------------------

--
-- Table structure for table `genre`
--

DROP TABLE IF EXISTS `genre`;
CREATE TABLE IF NOT EXISTS `genre` (
  `id` varchar(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `genre`
--

INSERT INTO `genre` (`id`, `libelle`) VALUES
('10000', 'Humour'),
('10001', 'Bande dessinée'),
('10002', 'Science Fiction'),
('10003', 'Biographie'),
('10004', 'Historique'),
('10006', 'Roman'),
('10007', 'Aventures'),
('10008', 'Essai'),
('10009', 'Documentaire'),
('10010', 'Technique'),
('10011', 'Voyages'),
('10012', 'Drame'),
('10013', 'Comédie'),
('10014', 'Policier'),
('10015', 'Presse Economique'),
('10016', 'Presse Culturelle'),
('10017', 'Presse sportive'),
('10018', 'Actualités'),
('10019', 'Fantazy');

-- --------------------------------------------------------

--
-- Table structure for table `livre`
--

DROP TABLE IF EXISTS `livre`;
CREATE TABLE IF NOT EXISTS `livre` (
  `id` varchar(10) NOT NULL,
  `ISBN` varchar(13) DEFAULT NULL,
  `auteur` varchar(20) DEFAULT NULL,
  `collection` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `livre`
--

INSERT INTO `livre` (`id`, `ISBN`, `auteur`, `collection`) VALUES
('00001', '1234569877896', 'Fred Vargas', 'Commissaire Adamsberg'),
('00002', '1236547896541', 'Dennis Lehanne', ''),
('00003', '6541236987410', 'Anne-Laure Bondoux', ''),
('00004', '3214569874123', 'Fred Vargas', 'Commissaire Adamsberg'),
('00005', '3214563214563', 'RJ Ellory', ''),
('00006', '3213213211232', 'Edgar P. Jacobs', 'Blake et Mortimer'),
('00007', '6541236987541', 'Kate Atkinson', ''),
('00008', '1236987456321', 'Jean d\'Ormesson', ''),
('00009', '3,21457E+12', 'Fred Vargas', 'Commissaire Adamsberg'),
('00010', '3,21457E+12', 'Manon Moreau', ''),
('00011', '3,21457E+12', 'Victoria Hislop', ''),
('00012', '3,21457E+12', 'Kate Atkinson', ''),
('00013', '3,21457E+12', 'Raymond Briggs', ''),
('00014', '3,21457E+12', 'RJ Ellory', ''),
('00015', '3,21457E+12', 'Floriane Turmeau', ''),
('00016', '3,21457E+12', 'Julian Press', ''),
('00017', '3,21457E+12', 'Philippe Masson', ''),
('00018', '3,21457E+12', '', 'Guide du Routard'),
('00019', '3,21457E+12', '', 'Guide Vert'),
('00020', '3,21457E+12', '', 'Guide Vert'),
('00021', '3,21457E+12', 'Claudie Gallay', ''),
('00022', '3,21457E+12', 'Claudie Gallay', ''),
('00023', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00024', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00025', '3,21457E+12', 'Ayrolles - Masbou', 'De cape et de crocs'),
('00026', '', 'Pierre Boulle', 'Julliard'),
('00027', '3,21457E+12', 'Philippe Masson', '');

--
-- Triggers `livre`
--
DROP TRIGGER IF EXISTS `inslivre`;
DELIMITER $$
CREATE TRIGGER `inslivre` BEFORE INSERT ON `livre` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM livres_dvd where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un livresdvd doit exister pour la création du livre";
    END IF;
    IF EXISTS(SELECT * FROM dvd where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un dvd existe déjà, insertion impossible du livre";
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `livres_dvd`
--

DROP TABLE IF EXISTS `livres_dvd`;
CREATE TABLE IF NOT EXISTS `livres_dvd` (
  `id` varchar(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `livres_dvd`
--

INSERT INTO `livres_dvd` (`id`) VALUES
('00001'),
('00002'),
('00003'),
('00004'),
('00005'),
('00006'),
('00007'),
('00008'),
('00009'),
('00010'),
('00011'),
('00012'),
('00013'),
('00014'),
('00015'),
('00016'),
('00017'),
('00018'),
('00019'),
('00020'),
('00021'),
('00022'),
('00023'),
('00024'),
('00025'),
('00026'),
('00027'),
('20001'),
('20002'),
('20003'),
('20004');

--
-- Triggers `livres_dvd`
--
DROP TRIGGER IF EXISTS `inslivres_dvd`;
DELIMITER $$
CREATE TRIGGER `inslivres_dvd` BEFORE INSERT ON `livres_dvd` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM document where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un document doit exister pour la création du livresdvd";
    END IF ; 
    IF EXISTS(SELECT * FROM revue where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "une revue existe déjà, insertion impossible du livresdvd";
    END IF ;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `public`
--

DROP TABLE IF EXISTS `public`;
CREATE TABLE IF NOT EXISTS `public` (
  `id` varchar(5) NOT NULL,
  `libelle` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `public`
--

INSERT INTO `public` (`id`, `libelle`) VALUES
('00001', 'Jeunesse'),
('00002', 'Adultes'),
('00003', 'Tous publics'),
('00004', 'Ados');

-- --------------------------------------------------------

--
-- Table structure for table `rayon`
--

DROP TABLE IF EXISTS `rayon`;
CREATE TABLE IF NOT EXISTS `rayon` (
  `id` char(5) NOT NULL,
  `libelle` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `rayon`
--

INSERT INTO `rayon` (`id`, `libelle`) VALUES
('BD001', 'BD Adultes'),
('BL001', 'Beaux Livres'),
('DF001', 'DVD films'),
('DV001', 'Sciences'),
('DV002', 'Maison'),
('DV003', 'Santé'),
('DV004', 'Littérature classique'),
('DV005', 'Voyages'),
('JN001', 'Jeunesse BD'),
('JN002', 'Jeunesse romans'),
('LV001', 'Littérature étrangère'),
('LV002', 'Littérature française'),
('LV003', 'Policiers français étrangers'),
('PR001', 'Presse quotidienne'),
('PR002', 'Magazines');

-- --------------------------------------------------------

--
-- Table structure for table `revue`
--

DROP TABLE IF EXISTS `revue`;
CREATE TABLE IF NOT EXISTS `revue` (
  `id` varchar(10) NOT NULL,
  `empruntable` tinyint(1) DEFAULT NULL,
  `periodicite` varchar(2) DEFAULT NULL,
  `delaiMiseADispo` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `revue`
--

INSERT INTO `revue` (`id`, `empruntable`, `periodicite`, `delaiMiseADispo`) VALUES
('10001', 1, 'MS', 52),
('10002', 1, 'MS', 52),
('10003', 1, 'HB', 15),
('10004', 1, 'HB', 15),
('10005', 0, 'QT', 5),
('10006', 0, 'QT', 5),
('10007', 1, 'HB', 26),
('10008', 1, 'HB', 26),
('10009', 0, 'QT', 5),
('10010', 1, 'HB', 12),
('10011', 1, 'MS', 52);

--
-- Triggers `revue`
--
DROP TRIGGER IF EXISTS `insrevue`;
DELIMITER $$
CREATE TRIGGER `insrevue` BEFORE INSERT ON `revue` FOR EACH ROW BEGIN
    IF NOT EXISTS(SELECT * FROM document where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un document doit exister pour la création de la revue";
    END IF ; 
    IF EXISTS(SELECT * FROM livres_dvd where id = NEW.id) THEN
        SIGNAL SQLSTATE "45000"
        SET MESSAGE_TEXT = "un livres_dvd existe déjà, insertion impossible de la revue";
    END IF ;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `service`
--

DROP TABLE IF EXISTS `service`;
CREATE TABLE IF NOT EXISTS `service` (
  `id` char(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `service`
--

INSERT INTO `service` (`id`, `libelle`) VALUES
('00001', 'admin'),
('00002', 'administratif'),
('00003', 'prêts'),
('00004', 'culture');

-- --------------------------------------------------------

--
-- Table structure for table `suivi`
--

DROP TABLE IF EXISTS `suivi`;
CREATE TABLE IF NOT EXISTS `suivi` (
  `id` char(5) NOT NULL,
  `libelle` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `suivi`
--

INSERT INTO `suivi` (`id`, `libelle`) VALUES
('00001', 'en cours'),
('00002', 'livrée'),
('00003', 'réglée'),
('00004', 'relancée');

-- --------------------------------------------------------

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
CREATE TABLE IF NOT EXISTS `utilisateur` (
  `id` varchar(10) NOT NULL,
  `login` varchar(20) DEFAULT NULL,
  `pwd` varchar(256) DEFAULT NULL,
  `idService` char(5) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idService` (`idService`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `utilisateur`
--

INSERT INTO `utilisateur` (`id`, `login`, `pwd`, `idService`) VALUES
('00002', 'AriesDouze13', 'ElPaMo82¤', '00001'),
('00003', 'bernard82', 'abcdefg://96X', '00004'),
('00004', 'dupont.jean', '1€M0tD3P4ss3!', '00002'),
('00005', 'BernardMo', 'BerMonpwd{78%}', '00001'),
('00006', 'adalou14', '123$aze', '00004'),
('00007', 'pret-mme-sophie', 'iÀpfiÉ4987£^§', '00003'),
('00008', 'Hekop', '3e1dc1b3fff47926a36de80bb8c760adb5a58f11369b5bce131c042d1e1b3fbc', '00001'),
('00009', 'Machop', 'c278aaa820c9f4f5c17b54c4e097092661059316e97a37197a62df09127763a6', '00003'),
('00010', 'Usopp', 'b6b41b4d9f3dcb62f553c335c52cbc8fee4139a900d082da76675a33add1ff04', '00002');

--
-- Constraints for dumped tables
--

--
-- Constraints for table `abonnement`
--
ALTER TABLE `abonnement`
  ADD CONSTRAINT `abonnement_ibfk_1` FOREIGN KEY (`id`) REFERENCES `commande` (`id`),
  ADD CONSTRAINT `abonnement_ibfk_2` FOREIGN KEY (`idRevue`) REFERENCES `revue` (`id`);

--
-- Constraints for table `commandedocument`
--
ALTER TABLE `commandedocument`
  ADD CONSTRAINT `commandedocument_ibfk_1` FOREIGN KEY (`id`) REFERENCES `commande` (`id`),
  ADD CONSTRAINT `commandedocument_ibfk_2` FOREIGN KEY (`idLivreDvd`) REFERENCES `livres_dvd` (`id`),
  ADD CONSTRAINT `commandedocument_ibfk_3` FOREIGN KEY (`idSuivi`) REFERENCES `suivi` (`id`);

--
-- Constraints for table `document`
--
ALTER TABLE `document`
  ADD CONSTRAINT `document_ibfk_1` FOREIGN KEY (`idRayon`) REFERENCES `rayon` (`id`),
  ADD CONSTRAINT `document_ibfk_2` FOREIGN KEY (`idPublic`) REFERENCES `public` (`id`),
  ADD CONSTRAINT `document_ibfk_3` FOREIGN KEY (`idGenre`) REFERENCES `genre` (`id`);

--
-- Constraints for table `dvd`
--
ALTER TABLE `dvd`
  ADD CONSTRAINT `dvd_ibfk_1` FOREIGN KEY (`id`) REFERENCES `livres_dvd` (`id`);

--
-- Constraints for table `exemplaire`
--
ALTER TABLE `exemplaire`
  ADD CONSTRAINT `exemplaire_ibfk_1` FOREIGN KEY (`id`) REFERENCES `document` (`id`),
  ADD CONSTRAINT `exemplaire_ibfk_2` FOREIGN KEY (`idEtat`) REFERENCES `etat` (`id`);

--
-- Constraints for table `livre`
--
ALTER TABLE `livre`
  ADD CONSTRAINT `livre_ibfk_1` FOREIGN KEY (`id`) REFERENCES `livres_dvd` (`id`);

--
-- Constraints for table `livres_dvd`
--
ALTER TABLE `livres_dvd`
  ADD CONSTRAINT `livres_dvd_ibfk_1` FOREIGN KEY (`id`) REFERENCES `document` (`id`);

--
-- Constraints for table `utilisateur`
--
ALTER TABLE `utilisateur`
  ADD CONSTRAINT `utilisateur_ibfk_1` FOREIGN KEY (`idService`) REFERENCES `service` (`id`);
COMMIT;


--
-- Procedures
--
DROP PROCEDURE IF EXISTS revueabonnements ;
DELIMITER //
CREATE PROCEDURE revueabonnements()
BEGIN
    SELECT titre, dateFinAbonnement  
        FROM document D INNER JOIN revue R ON D.id = R.id JOIN abonnement A ON R.id = A.idRevue 
        WHERE (0 <= DATEDIFF(dateFinAbonnement, NOW()) AND DATEDIFF(dateFinAbonnement, NOW()) < 30)
        ORDER BY dateFinAbonnement;
END ;
//
DELIMITER ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
