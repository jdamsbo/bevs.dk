-- phpMyAdmin SQL Dump
-- version 3.4.0
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 18, 2011 at 09:40 AM
-- Server version: 5.1.35
-- PHP Version: 5.2.6

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `bevs_dk_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `types`
--

CREATE TABLE IF NOT EXISTS `types` (
  `TypeId` int(16) NOT NULL AUTO_INCREMENT,
  `Type` varchar(50) DEFAULT NULL,
  `LongName` varchar(100) DEFAULT NULL,
  `Active` int(16) DEFAULT NULL,
  PRIMARY KEY (`TypeId`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=7 ;

--
-- Dumping data for table `types`
--

INSERT INTO `types` (`TypeId`, `Type`, `LongName`, `Active`) VALUES
(1, 'thyboe', 'Kurt Thyboe generator', 1),
(2, 'jj', 'Jørn Mader & Jørgen Leth generator', 1),
(3, 'chuck', 'Chuck Norris generator', 1),
(4, 'hmmm', 'Hmmmm, ting at tænke over', 1),
(5, 'inspiration', 'Inspiration og filosofiske betragtninger', 1),
(6, 'navne', 'Odiøse navne', 1);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
